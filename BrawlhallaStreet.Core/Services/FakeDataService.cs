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
        public object Configuration { get; }
        public object Logger { get; }
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

        public Task<List<BrawlhallaPlayer>> GetLatestEntriesForPlayer(int playerId)
        {
            throw new NotImplementedException();
        }

        public Task InsertPlayer(BrawlhallaPlayer player)
        {
            throw new NotImplementedException();
        }
    }
}
