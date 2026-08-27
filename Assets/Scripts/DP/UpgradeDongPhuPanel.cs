using System.Collections.Generic;
using UnityEngine;

namespace DP
{
    public class UpgradeDongPhuPanel : BaseBehaviour
    {
        [SerializeField] private List<UpgradePopup> popups;
        public void ShowPopup(DongPhuLevel level)
        {
            gameObject.SetActive(true);
            foreach (var upgradePopup in popups)
            {
                var active = upgradePopup.Level == level;
                upgradePopup.gameObject.SetActive(active);
                if (active)
                {
                    upgradePopup.Show();
                }
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}