using System;
using Base;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps.ui_rank
{
    public class RewardInfo : MonoBehaviour
    {
        [SerializeField] public Text endTimeText;

        public void SetEndTime(TimeSpan time)
        {
            var text = $"Trả thưởng sau: {Utils.GetTime(time)}";
            endTimeText.text = text;
        }
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public bool GetActive()
        {
            return gameObject.activeSelf;
        }
    }
}
