using BrawlhallaStreet.Core.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BrawlhallaStreet.Core
{
    // This should act more as a utility class rather than something to interface with discord
    public class StreetBot
    {
        private readonly IConfigurationRoot Configuration;
        private readonly IDataService DataService;
        public ILogger Logger;

        public StreetBot(IConfigurationRoot configuration, ILogger logger, IDataService dataService)
        {
            Configuration = configuration;
            Logger = logger;
            DataService = dataService;
        }

        public async Task LogSummaries()
        {
            var playerIds = Configuration.GetSection("PlayerIds").Get<List<int>>();
            try
            {
                foreach (var playerId in playerIds)
                {
                    var refreshed = await RefreshedPlayer(playerId);
                    if (refreshed)
                    {
                        var summaries = await CalculateStatsForPlayerGameSpan(playerId);
                        foreach (var item in summaries)
                        {
                            Logger.Information(item.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception During MainAsync: ", ex);
                throw;
            }
        }


        public async Task<bool> RefreshedPlayer(int playerId)
        {
            var player = await DataService.GetBrawlhallaPlayerFromApi(playerId);

            // Check if player has played
            Logger.Information("Checking to see if player has updated");
            var lastEntry = await DataService.GetLatestEntriesForPlayer(playerId);
            if (lastEntry.FirstOrDefault()?.Games < player.Games)
            {
                await DataService.InsertPlayer(player);
                return true;
            }
            return false;
        }

        public async Task<List<StatsSummary>> CalculateStatsForPlayerGameSpan(int playerId)
        {
            try
            {
                List<StatsSummary> statsSummaries = new List<StatsSummary>();
                var playerEntries = await DataService.GetLatestEntriesForPlayer(playerId);

                // Compare Difference
                var gameDiff = playerEntries[0].Games - playerEntries[1].Games;
                var oldLegendData = playerEntries[1].Legends.ToList();
                var newLegendData = playerEntries[0].Legends.ToList();
                var updatedLegend = newLegendData.Where(x => oldLegendData.Any(o => o.LegendNameKey == x.LegendNameKey && o.Games != x.Games)).ToList();

                // if no new games updatedLegend will be empty

                foreach (var item in updatedLegend)
                {
                    var gamesPlayed = newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Games - item.Games;
                    Logger.Information("Player played " + gamesPlayed + " game(s) With " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.LegendNameKey));

                    var statsSummary = new StatsSummary();
                    statsSummary.GamesPlayed = gamesPlayed;
                    statsSummary.PlayerName = playerEntries[0].Name;
                    statsSummary.WeaponOneDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Damageweaponone) - Convert.ToInt32(item.Damageweaponone);
                    statsSummary.WeaponTwoDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Damageweapontwo) - Convert.ToInt32(item.Damageweapontwo);
                    statsSummary.UnarmedDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Damageunarmed) - Convert.ToInt32(item.Damageunarmed);
                    statsSummary.GadgetDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Damagegadgets) - Convert.ToInt32(item.Damagegadgets);
                    statsSummary.ThrownItemDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Damagethrownitem) - Convert.ToInt32(item.Damagethrownitem);
                    statsSummary.TotalDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Damagedealt) - Convert.ToInt32(item.Damagedealt);
                    statsSummary.DamageTaken = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Damagetaken) - Convert.ToInt32(item.Damagetaken);

                    statsSummary.WeaponOneKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Koweaponone) - Convert.ToInt32(item.Koweaponone);
                    statsSummary.WeaponTwoKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Koweapontwo) - Convert.ToInt32(item.Koweapontwo);
                    statsSummary.UnarmedKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Kounarmed) - Convert.ToInt32(item.Kounarmed);
                    statsSummary.GadgetKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Kogadgets) - Convert.ToInt32(item.Kogadgets);
                    statsSummary.ThrownItemKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Kothrownitem) - Convert.ToInt32(item.Kothrownitem);
                    statsSummary.TotalKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Kos) - Convert.ToInt32(item.Kos);
                    statsSummary.Falls = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Falls) - Convert.ToInt32(item.Falls);

                    statsSummary.Suicides = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Suicides) - Convert.ToInt32(item.Suicides);
                    statsSummary.TeamKills = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Teamkos) - Convert.ToInt32(item.Teamkos);

                    Logger.Debug(statsSummary.ToString());
                    statsSummaries.Add(statsSummary);
                }

                return statsSummaries;
            }
            catch (Exception ex)
            {
                Log.Error("Exception: ", ex);
            }
            return null;
        }
    }
}