using System.Linq;
using Base;
using UnityEngine;
using UnityEngine.UI;

namespace ConvertDanLu
{
    /// <summary>
    /// Displays per-Tier conversion probability based on how many furnaces of each tier
    /// are currently placed in the 4 slots.
    /// 
    /// Example: 2 Nhất Phẩm + 2 Nhị Phẩm → 50% Nhất Phẩm | 50% Nhị Phẩm
    /// </summary>
    public class ConvertInfoManager : BaseBehaviour
    {
        [SerializeField] private Text infoText;
        [SerializeField] private GameObject selfObject;

        public void Init()
        {
            SetActive(false);
        }

        public void Clear()
        {
            SetActive(false);
        }

        public void SetActive(bool active)
        {
            if (selfObject != null)
                selfObject.SetActive(active);
        }

        public void Refresh()
        {
            var slots  = ConvertDanLuPanel.Instance.Page.PutItems.GetSlots();
            var filled = slots.Where(x => x.DanData != null).ToArray();

            if (filled.Length == 0)
            {
                SetActive(false);
                return;
            }

            SetActive(true);

            if (infoText == null) return;

            float total = filled.Length; // always ≤ 4

            // Group by Tier and compute each Tier's probability
            var groups = filled
                .GroupBy(x => x.DanData.ItemData.Tier)
                .Select(g => new
                {
                    Tier    = g.Key,
                    Count   = g.Count(),
                    Rate    = g.Count() / total * 100f,
                    Color   = CoreConfig.Instance.FindQuality(g.Key).Color
                })
                .OrderBy(g => g.Tier)
                .ToArray();

            var parts = groups.Select(g =>
            {
                var tierName = GetTierName(g.Tier);
                var hex      = ColorUtility.ToHtmlStringRGB(g.Color);
                return $"<color=#{hex}>{tierName}</color> {g.Rate:F0}%";
            });

            var joined = string.Join("  |  ", parts);
            infoText.text = $"Tỉ lệ đan phương: {joined}";
        }

        private static string GetTierName(Tier tier) => tier switch
        {
            Tier.NhatPham => "Nhất Phẩm",
            Tier.NhiPham  => "Nhị Phẩm",
            Tier.TamPham  => "Tam Phẩm",
            Tier.TuPham   => "Tứ Phẩm",
            Tier.NguPham  => "Ngũ Phẩm",
            Tier.LucPham  => "Lục Phẩm",
            _             => tier.ToString(),
        };
    }
}
