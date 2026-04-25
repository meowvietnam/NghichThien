using TMPro;
using UnityEngine;

namespace LuyenKhi
{
    public class EffectDetail
    {
        public string Name { get; set; }
        public int Sum { get; set; }
        public int Value { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class EffectDetailManager : BaseBehaviour
    {
        [SerializeField] private EffectDetailUI[] effectDetails;

        public void SetDetails(EffectDetail[] details)
        {
            foreach (var effectDetailUI in effectDetails)
            {
                effectDetailUI.gameObject.SetActive(false);
            }
            for (int i = 0; i < details.Length; i++)
            {
                var effectDetailUI = effectDetails[i];
                var detail = details[i];

                float ratio = detail.Value > 0 ? (float)detail.Sum / detail.Value : 0f;
                var progressColor =
                    ratio >= 1f ? "#22C55E" :
                    ratio > 0.5f ? "#F59E0B" :
                    "#EF4444"; 
                var rangeColor = "#9CA3AF";
                var text =
                    $"<b>{detail.Name}</b> " +
                    $"(<color={progressColor}>{detail.Sum}</color>/" +
                    $"<color={progressColor}>{detail.Value}</color>)" +
                    ": "+
                    $"<color={rangeColor}>[{detail.Min}-{detail.Max}]</color>";

                effectDetailUI.gameObject.SetActive(true);
                effectDetailUI.SetText(text);
            }
        }
    }
}