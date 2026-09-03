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
        [SerializeField] private UpgradeDongPhuPanel panel;
        private void Start()
        {
            SetEventButton(button, OnClick);
        }
        private void OnClick()
        {
            var nextLevel = DongPhuPanel.Instance.GetMyNextLevel();
            if(nextLevel == null)
            {
                DongPhuPanel.Instance.NotifyGroup.Show("Động phủ của bạn đã đạt cấp tối đa");
                return;
            }
            panel.ShowPopup(nextLevel.Level);
        }
    }
}