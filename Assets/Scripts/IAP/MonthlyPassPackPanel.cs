using System;
using Auction;
using Base;
using UnityEngine;
using UnityEngine.UI;

namespace IAP
{
    public class MonthlyPassPackPanel : Popup
    {
        [SerializeField] private Button payButton;
        [SerializeField] private Text moneyText;
        private Action payAction;

        private void Start()
        {
            SetEventButton(payButton, () => payAction?.Invoke());
        }

        public void SetMoney(int money)
        {
            moneyText.text = $"{Utils.FormatCount(money)} Tiên ngọc";
        }

        public void SetPurchaseEvent(Action action)
        {
            payAction = action;
        }
    }
}