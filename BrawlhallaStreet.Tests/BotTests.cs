using BrawlhallaStreeet.Cli;
using BrawlhallaStreet.Core;
using BrawlhallaStreet.Core.Modules;
using BrawlhallaStreet.Core.Services;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace BrawlhallaStreet.Tests
{
    public class BotTests
    {
        public ILogger Logger;
        public IConfigurationRoot Configuration;
        public IDataService DataService;

        public StreetBot StreetBot { get; set; }

        public BotTests(ITestOutputHelper output)
        {
            Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(output, Serilog.Events.LogEventLevel.Verbose)
                .CreateLogger()
                .ForContext<ApiTests>();

            Configuration = new ConfigurationBuilder()
                .AddJsonFile(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName + @"\BrawlhallaStreeet.Cli\appsettings.Development.json", false, true)
                .Build();

            DataService = new BrawlhallaDataService(Configuration, Logger);
            // DataService = new FakeDataService(Configuration, Logger);
            StreetBot = new StreetBot(Configuration, Logger, DataService);

            //var connectionString = Configuration["BrawlhallaDatabaseSettings:ConnectionString"];
        }
        [Fact]
        public async Task Scraper_Scrapes_LegendUrls()
        {
            Scraper scraper = new Scraper();
            await scraper.ScrapeLegendImages();
        }
        [Fact]
        public void CommandModule_Gets_Legend_Image()
        {
            var commandModule = new CommandModule(null, Configuration, Logger, StreetBot);
            var imgUrl = commandModule.GetImageUrlForLegend("Hattori");
            Logger.Debug(imgUrl);
            Assert.NotEmpty(imgUrl);
        }

        [Fact]
        public async Task StatsSummaries_Are_Formatted_PrettyAsync()
        {
            var commandModule = new CommandModule(null, Configuration, Logger, StreetBot);
            var recaps = await StreetBot.CalculateStatsForPlayerGameSpan(3879460);
            // 0  1  2
            // 3x  4  5
            // 6x  7  8
            // 9x  10 11
            var outputs = commandModule.FormatRecaps(recaps);
        }

        [Fact (Skip = "Updates DB")]
        public async Task StreetBot_Updates_Database()
        {
            await StreetBot.RefreshedPlayer(3879460);
        }

        [Fact]
        public async Task Calculate_Difference_Between_Last_Two_PlayersAsync()
        {
            var sut = await StreetBot.CalculateStatsForPlayerGameSpan(3879460);
            
            Assert.NotNull(sut);
            Log.Information(sut.FirstOrDefault()?.ToString());
        }


        [Fact]
        public void StreetBot_Gets_Configuration()
        {
            StreetBot streetBot = new StreetBot(Configuration, Logger, DataService);
            Assert.True(true);
        }
    }
}
