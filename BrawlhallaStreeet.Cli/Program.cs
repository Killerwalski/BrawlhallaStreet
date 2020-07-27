using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace BrawlhallaStreeet.Cli
{
    internal class Program
    {
        private static void Main(string[] args)
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

        }

        public async Task UpdatePlayer()
        {

        }
    }
}