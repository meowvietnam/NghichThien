using System;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

namespace Rank
{
    public class MonsterKillButton : BaseBehaviour
    {
        [SerializeField] private Button showButton;
        [SerializeField] private MonsterKillPanel panel;
        private void Start()
        {
            SetEventButton(showButton,ShowPanel);
        }

        private void ShowPanel()
        {
            panel.Show();
        }
    }
}
