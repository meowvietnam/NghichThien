using Network.Models;

namespace Network.Callback
{
    public interface ILoadRank
    {
        void LoadRankSuccess(LoadRankResult result);
        void LoadRankFailure(string message);
    }
}