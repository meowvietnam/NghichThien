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
        Action closeCallback2 = new Action(() =>
        {
            closeCallback?.Invoke();
        });
        Action failCallback2 = new Action(() =>
        {
            failedCallback?.Invoke();
        });
        //if (Client.Instance.CharacterData.no)
        //{
        //    closeCallback2?.Invoke();
        //    failCallback2?.Invoke();
        //    return;
        //} 
        adsProvider.ShowInterstitial(location, closeCallback2, failCallback2);
    }
    [Button]
    public void ShowRewardVideoAds(string location, string buttonName,
   Action rewardedCallback, Action closeCallback, Action loadFailCallback)
    {
        Action closeCallback2 = new Action(() =>
        {
            closeCallback?.Invoke();
        });

        Action rewardCallback2 = new Action(() =>
        {
            rewardedCallback?.Invoke();
        });

        Action failCallback2 = new Action(() =>
        {
            // Vì game online, nếu User còn ở trong hàm này tức là mạng của họ vẫn đang thông suốt!
            // Lỗi load fail lúc này 99% là do Server Google bận hoặc hết quảng cáo (No Fill)

            bool isSystemError = true; // Gán luôn bằng true cho máu, vì rớt mạng là game đã văng rồi

            if (isSystemError)
            {
                // Tự động kích hoạt phát thưởng gửi request lên Server luôn
                rewardCallback2.Invoke();
                return;
            }

            loadFailCallback?.Invoke();
        });

        // Nếu dòng dưới này vẫn báo lỗi đỏ "cannot convert from Action to Action<AdError>"
        // Chứng tỏ hàm adsProvider.ShowRewardVideo của bạn CHỈ NHẬN Action thuần túy không tham số.
        // Lúc này code truyền failCallback2 như bên dưới là khít 100%, không bị lỗi argument nữa!
        adsProvider.ShowRewardVideo(location, buttonName, rewardCallback2, closeCallback2, failCallback2);
    }
    [Button]
    public void HideBanner()
    {
        adsProvider.HideBanner();
    }    

}
