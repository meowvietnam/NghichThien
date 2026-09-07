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
    private bool isLoadingInterstitial;
    private bool isLoadingRewarded;

    #region AdUnit IDs

    private string GetBannerId()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || !BUILD_RELEASE
        // BANNER TEST ID
#if UNITY_ANDROID
        return "ca-app-pub-3940256099942544/6300978111"; // Test Android
#elif UNITY_IOS
        return "ca-app-pub-3940256099942544/2934735716"; // Test iOS
#else
        return "ca-app-pub-3940256099942544/6300978111";
#endif
#else
    // BANNER REAL ID
#if UNITY_ANDROID
        return "ca-app-pub-6144347258558138/5003556107"; // Real Android
#elif UNITY_IOS
        return ""; // Điền ID Banner Real iOS vào đây nếu có ghen babe
#else
        return "";
#endif
#endif
    }

    private string GetInterstitialId()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || !BUILD_RELEASE
        // INTERSTITIAL TEST ID
#if UNITY_ANDROID
        return "ca-app-pub-3940256099942544/1033173712"; // Test Android
#elif UNITY_IOS
        return "ca-app-pub-3940256099942544/4411468910"; // Test iOS
#else
        return "ca-app-pub-3940256099942544/1033173712";
#endif
#else
    // INTERSTITIAL REAL ID
#if UNITY_ANDROID
        return "ca-app-pub-6144347258558138/5728248630"; // Real Android
#elif UNITY_IOS
        return "ca-app-pub-6144347258558138/1789003629"; // Real iOS chuẩn đét
#else
        return "";
#endif
#endif
    }

    private string GetRewardedId()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || !BUILD_RELEASE
        // REWARDED TEST ID
#if UNITY_ANDROID
        return "ca-app-pub-3940256099942544/5224354917"; // Test Android
#elif UNITY_IOS
        return "ca-app-pub-3940256099942544/1712485313"; // Test iOS
#else
        return "ca-app-pub-3940256099942544/5224354917";
#endif
#else
    // REWARDED REAL ID
#if UNITY_ANDROID
        return "ca-app-pub-6144347258558138/6749349559"; // Real Android
#elif UNITY_IOS
        return "ca-app-pub-6144347258558138/8162840282"; // Real iOS chuẩn đét
#else
        return "";
#endif
#endif
    }

    #endregion

    #region Init

    public void InitAds()
    {
        if (isInitialize == 1) return;

        MobileAds.Initialize(initStatus => { isInitialize = 1; });
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
            RequestInterstitial(() =>
            {
                if (interstitialAd != null && interstitialAd.CanShowAd())
                {
                    interstitialAd.Show();
                }
                else
                {
                    interFailedCallback?.Invoke();
                }
            }, interFailedCallback);
        }
    }

    private void RequestInterstitial(Action onLoaded = null, Action onFailed = null)
    {
        if (isLoadingInterstitial) return;
        isLoadingInterstitial = true;

        string adUnitId = GetInterstitialId();
        AdRequest request = new AdRequest();

        InterstitialAd.Load(adUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            isLoadingInterstitial = false;
            if (error != null || ad == null)
            {
                Debug.LogError("Failed to load interstitial: " + error);
                onFailed?.Invoke();
                return;
            }

            interstitialAd?.Destroy();
            interstitialAd = ad;

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                NetworkManager.Instance.Invoke(() =>
                {
                    Debug.Log("Interstitial closed");
                    interCloserCallback?.Invoke();
                });
            };

            interstitialAd.OnAdFullScreenContentFailed += (AdError err) =>
            {
                NetworkManager.Instance.Invoke(() =>
                {
                    Debug.LogError("Interstitial failed: " + err);
                    interFailedCallback?.Invoke();
                });
            };

            onLoaded?.Invoke();
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
            RequestRewarded(() =>
            {
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
                    rewardAdsFailedCallback?.Invoke();
                }
            }, rewardAdsFailedCallback);
        }
    }

    private void RequestRewarded(Action onLoaded = null, Action onFailed = null)
    {
        if (isLoadingRewarded) return;
        isLoadingRewarded = true;

        string adUnitId = GetRewardedId();
        AdRequest request = new AdRequest();

        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            isLoadingRewarded = false;
            if (error != null || ad == null)
            {
                Debug.LogError("Failed to load rewarded: " + error);
                onFailed?.Invoke();
                return;
            }

            rewardedAd?.Destroy();
            rewardedAd = ad;
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                NetworkManager.Instance.Invoke(() =>
                {
                    Debug.Log("Rewarded closed");
                    rewardAdsCloseCallback?.Invoke();
                });
            };
            rewardedAd.OnAdFullScreenContentFailed += (AdError err) =>
            {
                NetworkManager.Instance.Invoke(() =>
                {
                    Debug.LogError("Rewarded failed: " + err);
                    rewardAdsFailedCallback?.Invoke();
                });
            };

            onLoaded?.Invoke();
        });
    }

    #endregion
}