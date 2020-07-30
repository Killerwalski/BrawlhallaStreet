using BrawlhallaStreeet.Cli;
using BrawlhallaStreet.Core;
using BrawlhallaStreet.Core.Services;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace BrawlhallaStreet.Tests
{
    public class ApiTests
    {
        public ILogger Logger;
        public IConfiguration Configuration;
        public IDataService DataService;

        public IMongoCollection<BrawlhallaPlayer> MongoCollection { get; }

        public ApiTests(ITestOutputHelper output)
        {
            Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(output, Serilog.Events.LogEventLevel.Verbose)
                .CreateLogger()
                .ForContext<ApiTests>();

            Configuration = new ConfigurationBuilder()
                // .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName +  @"\BrawlhallaStreeet.Cli\appsettings.Development.json", false, true)
                .Build();

            DataService = new BrawlhallaDataService(Configuration);

            //var connectionString = Configuration["BrawlhallaDatabaseSettings:ConnectionString"];
            //var databaseName = Configuration["BrawlhallaDatabaseSettings:DatabaseName"];
            //var collectionName = Configuration["BrawlhallaDatabaseSettings:CollectionName"];

            //var client = new MongoClient(connectionString);
            //var database = client.GetDatabase(databaseName);
            //MongoCollection = database.GetCollection<BrawlhallaPlayer>(collectionName);
        }

        [Fact]
        public async Task DataService_Gets_Updated_Player()
        {
            var result = await DataService.GetBrawlhallaPlayer(3879460);
            Logger.Information("Result: " + result);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Calculate_Difference_Between_Last_Two_PlayersAsync()
        {
            var playerEntries = new List<BrawlhallaPlayer>();
            var filter = new BsonDocument();
            using (var cursor = await MongoCollection.Find(filter).ToCursorAsync())
            {
                while (await cursor.MoveNextAsync())
                {
                    foreach (var doc in cursor.Current)
                    {
                        playerEntries.Add(doc);
                    }
                }
            }
            Logger.Information("Collection contains " + playerEntries.Count + " Players.");
            playerEntries = playerEntries.OrderByDescending(p => p.Games).Take(2).ToList();
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