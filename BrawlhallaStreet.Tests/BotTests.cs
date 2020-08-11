﻿using BrawlhallaStreet.Core;
using BrawlhallaStreet.Core.Modules;
using BrawlhallaStreet.Core.Services;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
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
            StreetBot = new StreetBot(Configuration, Logger, DataService);

            //var connectionString = Configuration["BrawlhallaDatabaseSettings:ConnectionString"];
        }

        [Fact]
        public async Task Command_Module_Outputs_Last_Recap()
        {
            var service = new Discord.Commands.CommandService();
            var commandModule = new CommandModule(service, Configuration);
            await commandModule.Recap();
        }

        [Fact]
        public async Task Calculate_Difference_Between_Last_Two_PlayersAsync()
        {
            await StreetBot.CalculateStatsForPlayerGameSpan(3879460);

        }

        [Fact]
        public async Task StreetBot_Saves_Updated_Player_Data()
        {
            await StreetBot.TestMethod();

        }

        [Fact]
        public void StreetBot_Gets_Configuration()
        {
            StreetBot streetBot = new StreetBot(Configuration, Logger, DataService);
            Assert.True(true);
        }
    }
}
