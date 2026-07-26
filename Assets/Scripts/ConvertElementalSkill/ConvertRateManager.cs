using System.Linq;
using Network;
using UnityEngine;

namespace ConvertElementalSkill
{
    public class ConvertRateManager : BaseBehaviour
    {
        [SerializeField] private ConvertRateText[] rateTexts;

        public void Init()
        {
            foreach (var convertRateText in rateTexts)
            {
                convertRateText.SetActive(false);
            }
        }

        public void Clear()
        {
            foreach (var convertRateText in rateTexts)
            {
                convertRateText.SetActive(false);
            }
        }

        public void RefreshRateText()
        {
            var instance = ConvertElementalSkillPanel.Instance;
            var slots = instance.Page.PutItems.GetSlots();
            var oneRate = 100f / slots.Length;
            var tierCount = slots
                .Where(x => x.SkillData != null)
                .GroupBy(x => x.SkillData.ItemData.TierSkill)
                .Select(g => new
                {
                    Tier = g.Key,
                    Count = g.Count(),
                    Color = g.First().SkillData.ItemData.VisualQuality.Color
                }).ToArray();
            var currentTypeSkillName = instance.CurrentSelectedTypeSkill.Name;
            for (int i = 0; i < tierCount.Length; i++)
            {
                var tier = tierCount[i].Tier;
                var count = tierCount[i].Count;
                var color = tierCount[i].Color;
                var tierText = BaseSkill.GetTextTierSkill(tier);
                var rate = count * oneRate;
                var text =
                    $"Có {rate:F2}% chuyển đổi hệ <i><color=#4A90E2>{currentTypeSkillName}</color></i> - <color=#{ColorUtility.ToHtmlStringRGB(color)}>{tierText}</color>";
                rateTexts[i].SetText(text);
                rateTexts[i].SetActive(true);
            }

            for (int i = tierCount.Length; i < rateTexts.Length; i++)
            {
                rateTexts[i].SetActive(false);
            }
        }
    }
}