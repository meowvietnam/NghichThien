using Base;
using CreateCharacter;
using Network;
using UnityEngine;
using UnityEngine.UI;
using Network.Extensions;
namespace DP
{
    public class UpgradeButton : BaseBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private PopupConfirmAction popup;
        private void Start()
        {
            SetEventButton(button, OnClick);
        }
        private void OnClick()
        {
            var nextLevel = DongPhuPanel.Instance.GetMyNextLevel();
            if(nextLevel == null)
            {
                NotifyGroup.Instance.Show("Động phủ của bạn đã đạt cấp tối đa");
                return;
            }
            var text = $"Sử dụng {Utils.FormatCount(nextLevel.RequireMoneyVip)} Tiên ngọc để nâng cấp Động phủ <color=#{ColorUtility.ToHtmlStringRGB(nextLevel.Color)}>{nextLevel.Name}</color>";
            popup.ShowPopConfirmAction(text, () =>
            {
                Client.Instance.Send(MessageTag.UpgradeDongPhu);
            });
        }
    }
}