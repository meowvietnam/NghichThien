using Rank;

namespace Network.Callback
{
    public interface IGetRank
    {
        void GetRankSuccess(IRank rank);
        void GetRankFail(string message);
    }
}