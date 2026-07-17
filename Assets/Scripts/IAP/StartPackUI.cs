using System;
using System.Globalization;
using System.Linq;
using Base;
using LongDHModules;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IAP
{
    public class StartPackUI : BaseBehaviour
    {
        [SerializeField] private Text moneyVipText;
        [SerializeField] private Text moneyText;
        [SerializeField] private Text congPhapText;
        [SerializeField] private Image congPhapIcon;
        [SerializeField] private Button purchaseButton;
        [SerializeField] public Popup ThisPopup => transform.GetComponent<Popup>();

        public void SetMoneyVipText(int moneyVip)
        {
            var text = $"{Utils.FormatCount(moneyVip)} Tiên ngọc";
            moneyText.text = text;
        }

        public void SetMoneyText(int money)
        {
            var text = $"{Utils.FormatCount(money)} Linh thạch";
            moneyText.text = text;
        }

        public void SetCongPhap(ushort id)
        {
            var gameManager = Pool.Get<GameManager>();
            var congPhap = gameManager.GetCongPhapWithId((int)id);
            if (congPhap == null)
            {
                Debug.Log($"Null congphap {id}");
                return;
            }
            congPhapText.text = congPhap.nameItem;
            congPhapIcon.sprite = congPhap.spriteItem;
        }

        public void SetPurchaseAction(Action action)
        {
            purchaseButton.onClick.RemoveAllListeners();
            SetEventButton(purchaseButton, action);
        }
    }
}