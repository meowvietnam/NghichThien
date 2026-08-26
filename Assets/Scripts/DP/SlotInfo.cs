using System;
using Base;
using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    public class SlotInfo : BaseBehaviour
    {
        [SerializeField] private Text textInfo;
        [SerializeField] private Slider slider;
        private PlantSeed seed;
        private bool isStop;
        public void SetSeed(PlantSeed seed)
        {
            isStop = false;
            this.seed = seed;
            if (!HasSeed())
            {
                slider.value = 0f;
            }
        }
        private bool HasSeed()
        {
            return seed != null;
        }
        private bool CanHarvest()
        {
            return DateTime.UtcNow >= seed.EstimateHarvestAt;
        }
        private void FixedUpdate()
        {
            if (!HasSeed() || isStop)
            {
                return;
            }
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            var estimateHarvestAt = seed.EstimateHarvestAt;
            var plantAt = seed.PlantAt;
            var harvestTime = estimateHarvestAt - DateTime.UtcNow;
            harvestTime = harvestTime > TimeSpan.Zero ? harvestTime : TimeSpan.Zero;
            var totalTime = estimateHarvestAt - plantAt;
            var progress = 1f;
            if (totalTime > TimeSpan.Zero)
            {
                progress = 1f - (float)(totalTime / harvestTime);
            }
            slider.value = progress;
            var canHarvest = CanHarvest();
            var text = canHarvest
                ? "<color=#5BE37D>Có thể thu hoạch</color>"
                : $"<color=#FFD43B>Còn {Utils.GetTime(harvestTime)}</color>";
            SetInfoText(text);
            isStop = canHarvest;
        }
        public void SetInfoText(string text)
        {
            textInfo.text = text;
        }
    }
}