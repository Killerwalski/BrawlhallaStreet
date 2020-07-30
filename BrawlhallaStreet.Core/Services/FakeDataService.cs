using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace BrawlhallaStreet.Core.Services
{
    class FakeDataService : IDataService
    {
        public Task<BrawlhallaPlayer> GetBrawlhallaPlayer(int playerId)
        {
            throw new NotImplementedException();
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
