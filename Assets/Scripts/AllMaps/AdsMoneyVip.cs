using System;
using CreateCharacter;
using LongDHModules;
using Network;
using Network.Callback;
using Network.Core;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    public class AdsMoneyVip : BaseBehaviour, IAdsCallback
    {
        [SerializeField] private GameObject selfObject;
        [SerializeField] private Button btn;
        private int moneyVip;
        private AdsHandler adsHandler;
        private static AdsMoneyVip instance;
        private void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                return;
            }
            selfObject.SetActive(false);
            SetEventButton(btn, Claim);
            var coreModule = Client.Instance.CoreModule;
            adsHandler = coreModule.GetHandler<AdsHandler>();
            adsHandler.SetCallback(this);
            adsHandler.GetAdsMoneyVipConfig();
        }

        private void Claim()
        {
            var charData = Client.Instance.CharacterData;
            if (!charData.IsNoAds)
            {
                var popup = Pool.Get<PopupConfirmAction>();
                popup.ShowPopConfirmAction($"Xem quảng cáo nhận {moneyVip} tiên ngọc", OnConfirm);
            }
            else
            {
                OnConfirm();
            }
        }

        private void OnConfirm()
        {
            adsHandler.ClaimMoneyVip();
        }

        public void Init(AdsMoneyVipConfig config)
        {
            moneyVip = config.MoneyVip;
            ActiveAds(config.Active);
        }

        public void ActiveAds(bool active)
        {
            selfObject.SetActive(active);
        }
    
        public void SuccessClaim(WatchAdsMoneyVipResult result)
        {
            var count = result.MoneyVip;
            MoneyVipItem.ShowNotification(count);
        }

        public void FailureClaim(string message)
        {
            NotifyGroup.Instance.Show(message);
        }
    }
}
