using Network.Models;

namespace Network.Callback
{
    public interface IAdsCallback
    {
        void Init(AdsMoneyVipConfig config);
        void ActiveAds(bool active);
        void SuccessClaim(WatchAdsMoneyVipResult result);
        void FailureClaim(string message);
    }
}