using System.Linq;
using Unity.Mathematics.Geometry;
using UnityEngine;

namespace LuyenKhi
{
    public class PutMaterials : MonoBehaviour
    {
        [SerializeField] public ItemSlot[] slots;
        [SerializeField] private EffectDetailManager effectDetailManager;
        private void OnValidate()
        {
            slots = GetComponentsInChildren<ItemSlot>();
        }

        public void Recalculate()
        {
            var matConfigs = LuyenKhiPanel.Instance.Resources.MaterialGradeConfigs;
            var slots = this.slots.Where(x => x.CurrentSelect != null).ToArray();
            var grade = 0;

            foreach (var slot in slots)
            {
                var tier = slot.CurrentSelect.Material.Tier;
                var matConfig = matConfigs.First(x => x.Tier == tier);
                grade += matConfig.Grade;
            }

            var luyenKhiPage = LuyenKhiPanel.Instance.LuyenKhiPage;
            var currentSelectedType = luyenKhiPage.CurrentSelectedType;
            var lianQiEffects = LuyenKhiPanel.Instance.Resources.LianQiEffects;
            var sumGroup = slots
                .Select(x => x.CurrentSelect.Material)
                .GroupBy(x => new
                {
                    x.CultivationElement,
                    x.YinYangType,
                })
                .Select(group => new
                {
                    Key = group.Key,
                    Sum = group.Sum(x => matConfigs.First(i => i.Tier == x.Tier).Grade),
                })
                .ToArray();
            var effectGroups = sumGroup
                .Select(x =>
                {
                    var k = x.Key;
                    var sum = x.Sum;
                    var effect = lianQiEffects.First(i =>
                            i.CultivationElement == k.CultivationElement && i.YinYangType == k.YinYangType)
                        .Effects
                        .First(i => i.ArtifactType == currentSelectedType);
                    var max = Mathf.RoundToInt(1.0f * sum / effect.Effect.Value);
                    var min = Mathf.RoundToInt(0.5f * max);
                    return new EffectDetail
                    {
                        Name = effect.Effect.Name,
                        Value = effect.Effect.Value,
                        Sum = sum,
                        Max = max,
                        Min = min,
                    };
                }).ToArray();
            effectDetailManager.SetDetails(effectGroups);
            luyenKhiPage.SetGrade(grade);
        }
    }
}