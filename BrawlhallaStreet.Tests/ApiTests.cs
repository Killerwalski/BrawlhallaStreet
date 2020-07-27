using BrawlhallaStreeet.Cli;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace BrawlhallaStreet.Tests
{
    public class ApiTests
    {
        public ILogger Logger;
        public IConfiguration Configuration;

        public ApiTests(ITestOutputHelper output)
        {
            Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(output, Serilog.Events.LogEventLevel.Verbose)
                .CreateLogger()
                .ForContext<ApiTests>();

            Configuration = new ConfigurationBuilder()
                // .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile(@"C:\Code\Repositories\BrawlhallaStreet\BrawlhallaStreeet.Cli\appsettings.Development.json", true, true)
                .Build();
        }

        [Fact]
        public async Task Insert_Updated_Player_ToMongoAsync()
        {
            Program prog = new Program();
            await prog.Setup();

            var result = await prog.GetBrawlhallaPlayer(3879460, Configuration);
            Logger.Information("Result: " + result);

            Assert.NotNull(result);
            await prog.InsertPlayer(result);
        }

        [Fact]
        public void Calculate_Difference_Between_Last_Two_Players()
        {

        }
    }
}