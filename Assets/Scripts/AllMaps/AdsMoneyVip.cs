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
        private void Start()
        {
            selfObject.SetActive(false);
            SetEventButton(btn, Claim);
            adsHandler = new AdsHandler(this);
            var coreModule = Client.Instance.CoreModule;
            coreModule.RemoveHandler<AdsHandler>();
            coreModule.AddHandler(adsHandler);
            adsHandler.GetAdsMoneyVipConfig();
        }

        private void Claim()
        {
            var popup = Pool.Get<PopupConfirmAction>();
            popup.ShowPopConfirmAction($"Xem quảng cáo nhận {moneyVip} tiên ngọc", OnConfirm);
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
