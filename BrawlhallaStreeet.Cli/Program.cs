using BrawlhallaStreet.Cli;
using BrawlhallaStreet.Core;
using BrawlhallaStreet.Core.Services;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace BrawlhallaStreeet.Cli
{
    public class Program
    {
        private IConfiguration Configuration;
        private IDataService DataService;

        private static async Task Main(string[] args)
        {
            await Startup.RunAsync(args);

            Console.WriteLine("Hello World!");

            // Program prog = new Program();
            // await prog.RunStreetBot();
        }

        public Program()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings.Development.json", true, true)
                .Build();
            DataService = new BrawlhallaDataService(Configuration, Log.Logger);
        }

        private async Task RunStreetBot()
        {
            var streetBot = new StreetBot(Configuration, Log.Logger, DataService);
            await streetBot.MainAsync();
        }
    }
}