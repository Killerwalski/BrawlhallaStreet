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
						await RefreshPlayer(playerId);
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
		}

		public async Task RefreshPlayer(int playerId)
        {
			var player = await DataService.GetBrawlhallaPlayer(playerId);
			await DataService.InsertPlayer(player);
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

        public async Task CalculateStatsForPlayerGameSpan(int playerId)
        {
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
                var weaponOneDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Damageweaponone) - Convert.ToInt32(item.Damageweaponone);
                var weaponTwoDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Damageweapontwo) - Convert.ToInt32(item.Damageweapontwo);
                var unarmedDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Damageunarmed) - Convert.ToInt32(item.Damageunarmed);
                var gadgetDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Damagegadgets) - Convert.ToInt32(item.Damagegadgets);
                var thrownItemDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Damagethrownitem) - Convert.ToInt32(item.Damagethrownitem);
                var totalDamage = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Damagedealt) - Convert.ToInt32(item.Damagedealt);
                var damageTaken = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Damagetaken) - Convert.ToInt32(item.Damagetaken);

                var weaponOneKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Koweaponone) - Convert.ToInt32(item.Koweaponone);
                var weaponTwoKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Koweapontwo) - Convert.ToInt32(item.Koweapontwo);
                var unarmedKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Kounarmed) - Convert.ToInt32(item.Kounarmed);
                var gadgetKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Kogadgets) - Convert.ToInt32(item.Kogadgets);
                var thrownItemKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Kothrownitem) - Convert.ToInt32(item.Kothrownitem);
                var totalKo = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Kos) - Convert.ToInt32(item.Kos);
                var falls = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Falls) - Convert.ToInt32(item.Falls);

                var suicides = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Suicides) - Convert.ToInt32(item.Suicides);
                var teamKills = Convert.ToInt32(newLegendData.Where(x => x.LegendNameKey == item.LegendNameKey).FirstOrDefault().Teamkos) - Convert.ToInt32(item.Teamkos);

                Logger.Information($"*** Damage Summary *** \n WeaponOne: {weaponOneDamage} \t WeaponTwo: {weaponTwoDamage} \t Unarmed: {unarmedDamage} \n" +
                    $"Gadgets: {gadgetDamage} \t Thrown Item: {thrownItemDamage} \t Total Damage: {totalDamage} \t Damage Taken: {damageTaken} ");
                Logger.Information($"*** KO Summary *** \n WeaponOne: {weaponOneKo} \t WeaponTwo: {weaponTwoKo} \t Unarmed: {unarmedKo} \n" +
                    $"Gadgets: {gadgetKo} \t Thrown Item: {thrownItemKo} \t Total KO: {totalKo} \t Falls: {falls} ");
                Logger.Information($"*** LULS *** \n Suicides: {suicides} \t Team KO: {teamKills} \n");
            }
        }
    }
}
