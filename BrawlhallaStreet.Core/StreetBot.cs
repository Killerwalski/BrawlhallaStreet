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

        public async Task<List<List<StatsSummary>>> GetPlayerSummaries()
        {
            List<List<StatsSummary>> allSummaries = new List<List<StatsSummary>>();
            List<StatsSummary> summaries = new List<StatsSummary>();
            // This is going to mess us up, probably should be a List<List<StatsSummary> for each player
            var playerIds = Configuration.GetSection("PlayerIds").Get<List<int>>();
            try
            {
                foreach (var playerId in playerIds)
                {
                    var refreshed = await RefreshedPlayer(playerId);
                    if (refreshed)
                    {
                        summaries = await CalculateStatsForPlayerGameSpan(playerId);
                        // Should add to the collection of players here
                        allSummaries.Add(summaries);
                    }
                    else
                        Logger.Debug("PlayerId " + playerId + " Has no new games.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception During MainAsync: ", ex);
                throw;
            }
            return allSummaries;
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
                var oldLegendIntersect = oldLegendData.Where(x => newLegendData.Any(o => o.LegendNameKey == x.LegendNameKey && o.Games != x.Games)).ToList();

                // if no new games updatedLegend will be empty

                foreach (var oldLegend in oldLegendIntersect)
                {
                    var gamesPlayed = newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Games - oldLegend.Games;
                    Logger.Debug("Player played " + gamesPlayed + " game(s) With " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(oldLegend.LegendNameKey));

                    var statsSummary = new StatsSummary();
                    statsSummary.GamesPlayed = gamesPlayed;
                    statsSummary.PlayerName = playerEntries[0].Name;
                    statsSummary.LegendName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(oldLegend.LegendNameKey);
                    statsSummary.WeaponOneDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Damageweaponone) - Convert.ToInt32(oldLegend.Damageweaponone);
                    statsSummary.WeaponTwoDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Damageweapontwo) - Convert.ToInt32(oldLegend.Damageweapontwo);
                    statsSummary.UnarmedDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Damageunarmed) - Convert.ToInt32(oldLegend.Damageunarmed);
                    statsSummary.GadgetDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Damagegadgets) - Convert.ToInt32(oldLegend.Damagegadgets);
                    statsSummary.ThrownItemDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Damagethrownitem) - Convert.ToInt32(oldLegend.Damagethrownitem);
                    statsSummary.TotalDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Damagedealt) - Convert.ToInt32(oldLegend.Damagedealt);
                    statsSummary.DamageTaken = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Damagetaken) - Convert.ToInt32(oldLegend.Damagetaken);

                    statsSummary.WeaponOneKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Koweaponone) - Convert.ToInt32(oldLegend.Koweaponone);
                    statsSummary.WeaponTwoKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Koweapontwo) - Convert.ToInt32(oldLegend.Koweapontwo);
                    statsSummary.UnarmedKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Kounarmed) - Convert.ToInt32(oldLegend.Kounarmed);
                    statsSummary.GadgetKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Kogadgets) - Convert.ToInt32(oldLegend.Kogadgets);
                    statsSummary.ThrownItemKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Kothrownitem) - Convert.ToInt32(oldLegend.Kothrownitem);
                    statsSummary.TotalKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Kos) - Convert.ToInt32(oldLegend.Kos);
                    statsSummary.Falls = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Falls) - Convert.ToInt32(oldLegend.Falls);

                    statsSummary.Suicides = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Suicides) - Convert.ToInt32(oldLegend.Suicides);
                    statsSummary.TeamKills = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == oldLegend.LegendNameKey).FirstOrDefault().Teamkos) - Convert.ToInt32(oldLegend.Teamkos);

                    // Logger.Debug(statsSummary.ToString());
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