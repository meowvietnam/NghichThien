using System;
using Base;
using UnityEngine;
using UnityEngine.UI;

namespace IAP
{
    public class MoneyVipPackUI : BaseBehaviour
    {
        [SerializeField] private Text moneyVipText;
        [SerializeField] private Text priceText;
        [SerializeField] private Image icon;
        [SerializeField] private Button purchaseButton;
        public void SetMoneyVipText(int moneyVip)
        {
            var text = $"{Utils.FormatMoney(moneyVip)} Tiên ngọc";
            moneyVipText.text = text;
        }
        public void SetPriceText(long price)
        {
            var text = $"{Utils.FormatMoney(price)} VNĐ";
            priceText.text = text;
        }

        public void SetIcon(Sprite icon)
        {
            this.icon.sprite = icon;
        }
        public void SetPurchaseAction(Action action)
        {
            purchaseButton.onClick.RemoveAllListeners();
            SetEventButton(purchaseButton, action);
        }
    }
}