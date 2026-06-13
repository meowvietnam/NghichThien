using System;

namespace SdkService.Components
{
    public interface IAdsProvider
    {
        public void InitAds();

        public void ShowRewardVideo(string location, string buttonName, Action rewardedCallback,
            Action closeCallback, Action loadFailCallback);

        public void ShowInterstitial(string location, Action cloaseCallback, Action failedCallback);

        public void ShowBanner();

        public void HideBanner();
    }
}