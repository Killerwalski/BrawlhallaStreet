using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrawlhallaStreet.Core.Services
{
    public class FakeDataService : IDataService
    {
        public IConfiguration Configuration { get; }
        public ILogger Logger { get; }
        public FakeDataService(IConfiguration configuration, ILogger logger)
        {
            Configuration = configuration;
            Logger = logger;

        }


        public Task<BrawlhallaPlayer> GetBrawlhallaPlayerFromApi(int playerId)
        {
            BrawlhallaPlayer fakePlayer;
            var test = Directory.GetFiles(Directory.GetCurrentDirectory());

            var fileName = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Samples\").Where(x => x.Contains("player")).FirstOrDefault();
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                fakePlayer = JsonConvert.DeserializeObject<BrawlhallaPlayer>(json);
            }
            return Task.FromResult(fakePlayer);
        }

        public async Task<List<BrawlhallaPlayer>> GetLatestEntriesForPlayer(int playerId)
        {
            List<BrawlhallaPlayer> fakePlayerEntries;
            var fileName = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Samples\").Where(x => x.Contains("latestEntries")).FirstOrDefault();
            using (StreamReader r = new StreamReader(fileName))
            {
                string json = r.ReadToEnd();
                fakePlayerEntries = JsonConvert.DeserializeObject<List<BrawlhallaPlayer>>(json);
            }

            return await Task.FromResult(fakePlayerEntries);
        }

        public Task InsertPlayer(BrawlhallaPlayer player)
        {
            return Task.CompletedTask;
        }
    }
}
