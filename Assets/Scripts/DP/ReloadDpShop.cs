using System;
using Network;
using UnityEngine;
using UnityEngine.UI;
using Network.Extensions;

namespace DP
{
    public class ReloadDpShop : BaseBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private PopupConfirmAction popup;

        private void Start()
        {
            SetEventButton(button, OnClick);
        }

        private void OnClick()
        {
            var client = Client.Instance;
            if (client.CharacterData.IsNoAds)
            {
                Send();
            }
            else
            {
                popup.ShowPopConfirmAction("Xác nhận xem quảng cáo làm mới cửa hàng?", Send);
            }
        }

        private void Send()
        {
            Client.Instance.Send(MessageTag.ReloadDpShop);
        }
    }
}