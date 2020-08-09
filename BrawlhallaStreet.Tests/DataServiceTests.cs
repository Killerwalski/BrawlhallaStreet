using BrawlhallaStreet.Core;
using BrawlhallaStreet.Core.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            Logger.Information("Returned Player:\n " +  playerJson);

        }
    }
}
