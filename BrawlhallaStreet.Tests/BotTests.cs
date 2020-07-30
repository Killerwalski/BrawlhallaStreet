using BrawlhallaStreet.Core;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace BrawlhallaStreet.Tests
{
    public class BotTests
    {
        public ILogger Logger;
        public IConfiguration Configuration;


        public BotTests(ITestOutputHelper output)
        {
            Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(output, Serilog.Events.LogEventLevel.Verbose)
                .CreateLogger()
                .ForContext<ApiTests>();

            Configuration = new ConfigurationBuilder()
                .AddJsonFile(@"C:\Code\Repositories\BrawlhallaStreet\BrawlhallaStreeet.Cli\appsettings.Development.json", true, true)
                .Build();

            //var connectionString = Configuration["BrawlhallaDatabaseSettings:ConnectionString"];
        }

        [Fact]
        public async Task StreetBot_Calls_Brawlhalla_Api()
        {
            var streetBot = new StreetBot(Configuration);
            // await streetBot.GetPlayerData();
        }

        [Fact]
        public async Task StreetBot_Saves_Updated_Player_Data()
        {

        }

        [Fact]
        public void StreetBot_Gets_Configuration()
        {
            StreetBot streetBot = new StreetBot(Configuration);
            Assert.True(true);
        }
    }
}
