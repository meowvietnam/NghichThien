using System;
using DG.Tweening;
using Network;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    public class UpgradePopup : BaseBehaviour
    {
        [field: SerializeField] public DongPhuLevel Level { get; private set; }
        [SerializeField] private Button buyWithMoneyVip;
        [SerializeField] private Button buyWithIap;
        [SerializeField] private Button close;
        [SerializeField] private UpgradeDongPhuPanel panel;
        private Tween tween;

        private void Start()
        {
            SetEventButton(buyWithIap, BuyWithIap);
            SetEventButton(buyWithMoneyVip, BuyWithMoneyVip);
            SetEventButton(close, Hide);
        }

        private void BuyWithMoneyVip()
        {
            BuyWith(UpgradeDongPhuBy.MoneyVip);
            Hide();
        }

        private void BuyWith(UpgradeDongPhuBy by)
        {
            var request = new UpgradeDongPhu()
            {
                By = by
            };
            Client.Instance.MemSend(MessageTag.UpgradeDongPhu, request);
        }

        private void BuyWithIap()
        {
            BuyWith(UpgradeDongPhuBy.Iap);
            Hide();
        }

        public void Show()
        {
            tween?.Kill();
            tween = transform.DOScale(Vector3.one, 0.25f);
        }

        private void Hide()
        {
            tween?.Kill();
            tween = transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => { panel.Hide(); });
        }
    }
}