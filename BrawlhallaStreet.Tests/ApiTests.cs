using BrawlhallaStreeet.Cli;
using BrawlhallaStreet.Core;
using BrawlhallaStreet.Core.Services;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
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

            DataService = new BrawlhallaDataService(Configuration, Logger);
        }

        [Fact]
        public async Task DataService_Gets_Updated_Player()
        {
            var result = await DataService.GetBrawlhallaPlayerFromApi(3879460);
            Logger.Information("Result: " + result);
            var serialized = JsonConvert.SerializeObject(result, Formatting.Indented);
            System.IO.File.WriteAllText(Directory.GetCurrentDirectory() + @$"\Samples\playerSample.json", serialized);

            Assert.NotNull(result);
        }
    }
}