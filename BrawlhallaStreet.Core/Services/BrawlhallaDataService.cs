using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrawlhallaStreet.Core.Services
{
    public class BrawlhallaDataService : IDataService
    {
        private IConfiguration Configuration;
        private IMongoCollection<BrawlhallaPlayer> MongoCollection;

        public BrawlhallaDataService(IConfiguration configuration)
        {
            Configuration = configuration;

            var connectionString = Configuration["BrawlhallaDatabaseSettings:ConnectionString"];
            var databaseName = Configuration["BrawlhallaDatabaseSettings:DatabaseName"];
            var collectionName = Configuration["BrawlhallaDatabaseSettings:CollectionName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            MongoCollection = database.GetCollection<BrawlhallaPlayer>(collectionName);
        }

        public async Task<BrawlhallaPlayer> GetBrawlhallaPlayer(int playerId)
        {
            var url = $"https://api.brawlhalla.com/player/{playerId}/stats?api_key=" + Configuration["BrawlhallaApiKey"];
            HttpClient client = new HttpClient();
            string responseBody = await client.GetStringAsync(url);
            var player = JsonConvert.DeserializeObject<BrawlhallaPlayer>(responseBody);
            return player;
        }

        public async Task InsertPlayer(BrawlhallaPlayer player)
        {
            await MongoCollection.InsertOneAsync(player);
        }

        private async Task SeedDatabase()
        {
            var document = JsonConvert.DeserializeObject<BrawlhallaPlayer>(File.ReadAllText(@"Killerwalski_Seed.json"));
            await MongoCollection.InsertOneAsync(document);
        }
    }
}