using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrawlhallaStreet.Core.Services
{
    public class BrawlhallaDataService : IDataService
    {
        private IConfigurationRoot Configuration;
        private IMongoCollection<BrawlhallaPlayer> MongoCollection;
        public ILogger Logger;

        public BrawlhallaDataService(IConfigurationRoot configuration, ILogger logger)
        {
            Configuration = configuration;
            Logger = logger;

            var connectionString = Configuration["BrawlhallaDatabaseSettings:ConnectionString"];
            var databaseName = Configuration["BrawlhallaDatabaseSettings:DatabaseName"];
            var collectionName = Configuration["BrawlhallaDatabaseSettings:CollectionName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            MongoCollection = database.GetCollection<BrawlhallaPlayer>(collectionName);
        }

        public async Task<BrawlhallaPlayer> GetBrawlhallaPlayerFromApi(int playerId)
        {
            var url = $"https://api.brawlhalla.com/player/{playerId}/stats?api_key=" + Configuration["BrawlhallaApiKey"];
            HttpClient client = new HttpClient();
            string responseBody = await client.GetStringAsync(url);
            var player = JsonConvert.DeserializeObject<BrawlhallaPlayer>(responseBody);
            return player;
        }

        public async Task InsertPlayer(BrawlhallaPlayer player)
        {
            if (player.CreatedDate == null)
            {
                player.CreatedDate = DateTime.Now;
            }

            await MongoCollection.InsertOneAsync(player);
        }

        private async Task SeedDatabase()
        {
            var document = JsonConvert.DeserializeObject<BrawlhallaPlayer>(File.ReadAllText(@"Killerwalski_Seed.json"));
            await MongoCollection.InsertOneAsync(document);
        }

        public async Task<List<BrawlhallaPlayer>> GetLatestEntriesForPlayer(int playerId)
        {
            var playerEntries = new List<BrawlhallaPlayer>();
            var filter = Builders<BrawlhallaPlayer>.Filter.Eq(p => p.BrawlhallaId, playerId);
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
            return playerEntries;
        }
    }
}