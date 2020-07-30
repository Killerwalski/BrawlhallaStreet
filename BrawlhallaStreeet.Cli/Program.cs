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

            var streetBot = new StreetBot(prog.Configuration);
            await streetBot.MainAsync();
        }
    }
}