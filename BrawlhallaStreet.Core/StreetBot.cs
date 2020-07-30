using BrawlhallaStreet.Core.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrawlhallaStreet.Core
{
    public class StreetBot
    {
        private DiscordSocketClient Client;
		private readonly IConfiguration Configuration;
		private IDataService DataService;
		public ILogger Logger;

		public StreetBot(IConfiguration configuration, ILogger logger, IDataService dataService)
		{
			Configuration = configuration;
            Logger = logger;
			DataService = dataService;
		}
		public async Task MainAsync()
		{
			await SetupLogger();
			Client = new DiscordSocketClient();
			Client.Log += Log;

			// Remember to keep token private or to read it from an 
			// external source! In this case, we are reading the token 
			// from an environment variable. If you do not know how to set-up
			// environment variables, you may find more information on the 
			// Internet or by using other methods such as reading from 
			// a configuration.
			await Client.LoginAsync(TokenType.Bot,
				Configuration["DiscordToken"]);
				// Environment.GetEnvironmentVariable("DiscordToken"));
				// "NzM3MTU4OTY5MTg1MDA5NzQ2.Xx5Syg.vn617f-LfhlHAIHAFzbf40Kr1yk");
			await Client.StartAsync();

			var playerIds = Configuration.GetSection("PlayerIds").Get<List<int>>();
			try
            {
                while (true)
                {
                    foreach (var playerId in playerIds)
                    {
						var refreshed = await RefreshedPlayer(playerId);
                        if (refreshed)
                        {
                            var summaries = await CalculateStatsForPlayerGameSpan(playerId);
                            foreach (var item in summaries)
                            {
                                var test = Client;
                            }
                        }
                    }

					await Task.Delay(120 * 1000);
				}
            }
            catch (Exception ex)
            {
				Logger.Error("Exception During MainAsync: ", ex);
				await Client.StopAsync();
				Client.Dispose();
				throw;
            }

			await Client.StopAsync();
			Client.Dispose();
		}

		public async Task TestMethod()
        {
			var playerIds = Configuration.GetSection("PlayerIds").Get<List<int>>();
            await Task.CompletedTask;
		}

		public async Task<bool> RefreshedPlayer(int playerId)
        {
			var player = await DataService.GetBrawlhallaPlayer(playerId);
            // Check if player has played
            var latestEntry = await DataService.GetLatestEntriesForPlayer(playerId);
            if (latestEntry.FirstOrDefault()?.Games < player.Games)
            {
                await DataService.InsertPlayer(player);
                return true;
            }
            return false;
        }

		private Task SetupLogger()
        {
			Logger = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.CreateLogger()
				.ForContext<StreetBot>();

			return Task.CompletedTask;
		}

		private Task Log(LogMessage msg)
        {
			Logger.Information(msg.ToString());
            // Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task<List<StatsSummary>> CalculateStatsForPlayerGameSpan(int playerId)
        {
            List<StatsSummary> statsSummaries = new List<StatsSummary>();
            var playerEntries = await DataService.GetLatestEntriesForPlayer(playerId);
            
            // Compare Difference
            var difference = string.Empty;
            var gameDiff = playerEntries[0].Games - playerEntries[1].Games;
            var oldLegendData = playerEntries[1].Legends.ToList();
            var newLegendData = playerEntries[0].Legends.ToList();
            var updatedLegend = oldLegendData.Where(x => newLegendData.Any(o => o.LegendNameKey == x.LegendNameKey && o.Games != x.Games)).ToList();
            foreach (var item in updatedLegend)
            {
                Logger.Information("Player played " + (newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Games - item.Games) + " game(s) With " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.LegendNameKey));

                // Could probably just make this a class
                var statsSummary = new StatsSummary();

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
    }
}
