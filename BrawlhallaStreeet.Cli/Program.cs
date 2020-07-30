using BrawlhallaStreet.Core;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BrawlhallaStreeet.Cli
{
    public class Program
    {
        private IConfiguration Configuration;
        private IMongoCollection<BrawlhallaPlayer> MongoCollection;
        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.File("BrawlhallaStreet-Cli.log", fileSizeLimitBytes: 10000000)
           .CreateLogger();

            Console.WriteLine("Hello World!");
            Log.Information("Application Starting");

            
            Program prog = new Program();
            await prog.Setup();

            var streetBot = new StreetBot(prog.Configuration);
            await streetBot.MainAsync();

        }

        public async Task Setup()
        {
            Configuration = new ConfigurationBuilder()
                // .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("appsettings.Development.json", true, true)
                .Build();

            var connectionString = Configuration["BrawlhallaDatabaseSettings:ConnectionString"];
            var databaseName = Configuration["BrawlhallaDatabaseSettings:DatabaseName"];
            var collectionName = Configuration["BrawlhallaDatabaseSettings:CollectionName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            MongoCollection = database.GetCollection<BrawlhallaPlayer>(collectionName);

            var count = await MongoCollection.CountDocumentsAsync(new BsonDocument());
            if (count == 0)
                await SeedDatabase();

        }

        public async Task<BrawlhallaPlayer> GetBrawlhallaPlayer(int playerId, IConfiguration config)
        {
            var url = $"https://api.brawlhalla.com/player/{playerId}/stats?api_key=" + config["BrawlhallaApiKey"];
            HttpClient client = new HttpClient();
            string responseBody = await client.GetStringAsync(url);
            var player = JsonConvert.DeserializeObject<BrawlhallaPlayer>(responseBody);
            return player;
        }

        private async Task SeedDatabase()
        {
            var document = JsonConvert.DeserializeObject<BrawlhallaPlayer>(File.ReadAllText(@"Killerwalski_Seed.json"));
            await MongoCollection.InsertOneAsync(document);
        }

        public async Task InsertPlayer(BrawlhallaPlayer player)
        {
            await MongoCollection.InsertOneAsync(player);
        }
    }
}