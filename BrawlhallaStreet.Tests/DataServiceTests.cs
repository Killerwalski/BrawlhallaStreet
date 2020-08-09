using BrawlhallaStreet.Core;
using BrawlhallaStreet.Core.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace BrawlhallaStreet.Tests
{
    public class DataServiceTests
    {
        public ILogger Logger;
        public IConfiguration Configuration;
        public IDataService DataService;

        public DataServiceTests(ITestOutputHelper output)
        {
            Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(output, Serilog.Events.LogEventLevel.Verbose)
                .CreateLogger()
                .ForContext<ApiTests>();

            Configuration = new ConfigurationBuilder()
                // .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName + @"\BrawlhallaStreeet.Cli\appsettings.Development.json", false, true)
                .Build();

            DataService = new FakeDataService(Configuration, Logger);
        }

        [Fact]
        public async Task DataService_Gets_Player()
        {
            var player = await DataService.GetBrawlhallaPlayerFromApi(1);
            var playerJson = JsonConvert.SerializeObject(player, Formatting.Indented);
            Assert.NotNull(player);
            Logger.Information("Returned Player:\n " + playerJson);
        }

        [Fact]
        public async Task DataService_Gets_Latest_Entries()
        {
            var latestEntires = (await DataService.GetLatestEntriesForPlayer(1)).Take(2).ToList();
            var entriesJson = JsonConvert.SerializeObject(latestEntires, Formatting.Indented);
            Assert.True(latestEntires.Count == 2);
            Logger.Information("Returned Player:\n " + entriesJson);
        }

        [Fact]
        public async Task DataService_Inserts_New_Player()
        {
            var player = new BrawlhallaPlayer() { Name = "FakePlayer" };
            await DataService.InsertPlayer(player);

        }
    }
}