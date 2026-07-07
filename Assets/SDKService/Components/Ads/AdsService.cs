//using GamePlay.Script;
using GoogleMobileAds.Api;
using Network;
using SdkService.Components;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AdsService : SingletonMonoDontDestroy<AdsService>
{
    private IAdsProvider adsProvider;

    private DateTime lastPauseTime;
    private DateTime expireTime;

    private DateTime lastTimeCloseInter;
    private bool isShowingOther = false;

    private void Awake()
    {
        Init();
    }
    public void Init()
    {
        if (adsProvider == null)
        {
            adsProvider = new AdmobAds();
            adsProvider.InitAds();
        }

        lastTimeCloseInter = DateTime.Now.AddDays(-1);
        HideBanner();
    }
    [Button]
    public void ShowInterstitialAds(string location, Action closeCallback, Action failedCallback)
    {
        Action closeCallback2 = () =>
        {
            closeCallback?.Invoke();
        };
        Action failCallback2 = () =>
        {
            failedCallback?.Invoke();
        };
        adsProvider.ShowInterstitial(location, closeCallback2, failCallback2);
    }
    public void ShowRewardVideoAds(string location, string buttonName,
    Action rewardedCallback, Action closeCallback, Action loadFailCallback)
    {
        float startTime = Time.realtimeSinceStartup;
        Action closeCallback2 = () =>
        {
            float duration = Time.realtimeSinceStartup - startTime;
            if (duration < 5f)
            {
                NetworkManager.Instance.Invoke(() => {
                    loadFailCallback?.Invoke();
                    Debug.LogWarning("Xem quảng cáo quá nhanh, nghi vấn hack/skip!");
                });
                return;
            }
            closeCallback?.Invoke();
        };

        Action rewardCallback2 = () =>
        {
            rewardedCallback?.Invoke();
        };

        Action failCallback2 = () =>
        {
            NetworkManager.Instance.Invoke(() =>
            {
                // Tính thời gian từ lúc bấm đến lúc báo Fail
                float duration = Time.realtimeSinceStartup - startTime;

                // NẾU BỊ FAIL QUÁ NHANH (Dưới 3 giây): Chắc chắn do AdBlocker chặn DNS hoặc mất mạng hẳn
                if (duration < 3f)
                {
                    // Cứ báo lỗi mạng/hệ thống bận, KHÔNG phát thưởng đền bù gì hết!
                    loadFailCallback?.Invoke();
                    Debug.Log("Quảng cáo bị chặn đứng ngay lập tức -> Nghi vấn dùng AdBlocker");
                    return;
                }

                // NẾU FAIL CHẬM (Quá 3 giây): Có thể do mạng quay xoay xoay một lúc rồi mới fail, 
                // hoặc lỗi Server bận thật, lúc này mới kích hoạt đền bù phát quà cho user ngoan ngoãn
                bool isRealSystemError = true;

                if (isRealSystemError)
                {
                    rewardCallback2.Invoke();
                    return;
                }

                loadFailCallback?.Invoke();
            });
        };

        adsProvider.ShowRewardVideo(location, buttonName, rewardCallback2, closeCallback2, failCallback2);
    }
    [Button]
    public void HideBanner()
    {
        adsProvider.HideBanner();
    }    

}
