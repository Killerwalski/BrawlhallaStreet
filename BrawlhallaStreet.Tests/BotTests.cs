using BrawlhallaStreet.Core;
using BrawlhallaStreet.Core.Services;
using Microsoft.Extensions.Configuration;
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
    public class BotTests
    {
        public ILogger Logger;
        public IConfiguration Configuration;
        public IDataService DataService;

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

            DataService = new BrawlhallaDataService(Configuration);

            //var connectionString = Configuration["BrawlhallaDatabaseSettings:ConnectionString"];
        }

        [Fact]
        public async Task StreetBot_Saves_Updated_Player_Data()
        {
            StreetBot streetBot = new StreetBot(Configuration, DataService);

        }

        [Fact]
        public void StreetBot_Gets_Configuration()
        {
            StreetBot streetBot = new StreetBot(Configuration, DataService);
            Assert.True(true);
        }
    }
}
