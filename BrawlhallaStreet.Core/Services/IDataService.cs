using System.Threading.Tasks;

namespace BrawlhallaStreet.Core.Services
{
    public interface IDataService
    {
        public Task InsertPlayer(BrawlhallaPlayer player);
        public Task<BrawlhallaPlayer> GetBrawlhallaPlayer(int playerId);
    }
}