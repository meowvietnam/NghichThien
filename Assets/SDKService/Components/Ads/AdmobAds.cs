using GoogleMobileAds.Api;
using Network;
using SdkService.Components;
using System;
using UnityEngine;

public class AdmobAds : IAdsProvider
{
    [HideInInspector] public int isInitialize = 0;

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private Action rewardAdsCloseCallback;
    private Action rewardAdsRewardedCallback;
    private Action rewardAdsFailedCallback;
    private Action interCloserCallback;
    private Action interFailedCallback;

    private bool showBanner = false;

    #region AdUnit IDs
    private string GetBannerId()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || !BUILD_RELEASE
        return "ca-app-pub-3940256099942544/6300978111"; // TEST ID
#else
        return "ca-app-pub-6144347258558138/5003556107"; // REAL BANNER ID
#endif
    }

    private string GetInterstitialId()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || !BUILD_RELEASE
        return "ca-app-pub-3940256099942544/1033173712"; // TEST ID
#else
        return "ca-app-pub-6144347258558138/4467554150"; // REAL INTERSTITIAL ID
#endif
    }

    private string GetRewardedId()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD|| !BUILD_RELEASE
        return "ca-app-pub-3940256099942544/5224354917"; // TEST ID
#else
        return "ca-app-pub-6144347258558138/3154472482"; // REAL REWARDED ID
#endif
    }
    #endregion

    #region Init
    public void InitAds()
    {
        if (isInitialize == 1) return;

        MobileAds.Initialize(initStatus =>
        {
            isInitialize = 1;

            //RequestBanner();
            RequestInterstitial();
            RequestRewarded();
        });
    }
    #endregion

    #region Banner
    public void ShowBanner()
    {
        if (bannerView == null)
            RequestBanner();

        showBanner = true;
        bannerView?.Show();
    }

    public void HideBanner()
    {
        showBanner = false;
        bannerView?.Hide();
    }

    private void RequestBanner()
    {
        string adUnitId = GetBannerId();
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);

        bannerView.OnBannerAdLoaded += () => Debug.Log("Banner loaded");
        bannerView.OnBannerAdLoadFailed += (error) => Debug.Log("Banner failed: " + error);
    }
    #endregion

    #region Interstitial
    public void ShowInterstitial(string location, Action closeCallback, Action failedCallback)
    {
        interCloserCallback = closeCallback;
        interFailedCallback = failedCallback;

        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial not ready");
            interFailedCallback?.Invoke();
            RequestInterstitial();
        }
    }
    private void RequestInterstitial()
    {
        string adUnitId = GetInterstitialId();
        AdRequest request = new AdRequest();

        InterstitialAd.Load(adUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Failed to load interstitial: " + error);
                return;
            }

            interstitialAd = ad;

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                NetworkManager.Instance.Invoke(() =>
                {
                    Debug.Log("Interstitial closed");
                    interCloserCallback?.Invoke();
                    RequestInterstitial();
                });
            };

            interstitialAd.OnAdFullScreenContentFailed += (AdError err) =>
            {
                NetworkManager.Instance.Invoke(() =>
                {
                    Debug.LogError("Interstitial failed: " + err);
                    interFailedCallback?.Invoke();
                    RequestInterstitial();
                });
            };
        });
    }
    #endregion

    #region Rewarded
    public void ShowRewardVideo(string location, string buttonName,
        Action rewardedCallback, Action closeCallback, Action loadFailCallback)
    {
        rewardAdsRewardedCallback = rewardedCallback;
        rewardAdsCloseCallback = closeCallback;
        rewardAdsFailedCallback = loadFailCallback;

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                NetworkManager.Instance.Invoke(() =>
                {
                    Debug.Log($"User rewarded: {reward.Type}, amount: {reward.Amount}");
                    rewardAdsRewardedCallback?.Invoke();
                });
            });
        }
        else
        {
            Debug.Log("Rewarded not ready");
            rewardAdsFailedCallback?.Invoke();
            RequestRewarded();
        }
    }

    private void RequestRewarded()
    {
        string adUnitId = GetRewardedId();
        AdRequest request = new AdRequest();

        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Failed to load rewarded: " + error);
                return;
            }
            rewardedAd = ad;
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                NetworkManager.Instance.Invoke(() =>
                {
                    Debug.Log("Rewarded closed");
                    rewardAdsCloseCallback?.Invoke();
                    RequestRewarded();
                });
            };
            rewardedAd.OnAdFullScreenContentFailed += (AdError err) =>
            {
                NetworkManager.Instance.Invoke(() =>
                {
                    Debug.LogError("Rewarded failed: " + err);
                    rewardAdsFailedCallback?.Invoke();
                    RequestRewarded();
                });
            };
        });
    }
    #endregion
}
