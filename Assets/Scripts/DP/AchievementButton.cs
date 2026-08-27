using System;
using CreateCharacter;
using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    public class AchievementButton : BaseBehaviour
    {
        [SerializeField] private Button button;
        private void Start()
        {
            SetEventButton(button, OnClick);
        }
        private void OnClick()
        {
            NotifyGroup.Instance.Show("Tính năng đang phát triển");
        }
    }
}