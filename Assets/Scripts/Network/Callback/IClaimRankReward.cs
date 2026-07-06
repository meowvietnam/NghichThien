using Network.Models;

namespace Network.Callback
{
    public interface IClaimRankReward
    {
        void RewardSuccess(ClaimRankRewardResult result);
        void FailClaimReward(string message);
    }
}