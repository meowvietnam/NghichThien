using System.Linq;
using Unity.Mathematics.Geometry;
using UnityEngine;

namespace LuyenKhi
{
    public class PutMaterials : MonoBehaviour
    {
        [SerializeField] public ItemSlot[] slots;
        [SerializeField] private EffectDetailManager effectDetailManager;

        public void Init()
        {
            foreach (var itemSlot in slots)
            {
                itemSlot.Clear();
            }
            effectDetailManager.Init();
        }
        public Material[] GetSelectedMaterials()
        {
            return slots.Where(x => x.CurrentSelect != null).Select(x => x.CurrentSelect.Material).ToArray();
        }
        public void Recalculate()
        {
            var matConfigs = LuyenKhiPanel.Instance.Resources.MaterialGradeConfigs;
            var materials = GetSelectedMaterials();
            var grade = 0;
            foreach (var material in materials)
            {
                var tier = material.Tier;
                var matConfig = matConfigs.First(x => x.Tier == tier);
                grade += matConfig.Grade;
            }

            var luyenKhiPage = LuyenKhiPanel.Instance.LuyenKhiPage;
            var selectedType = luyenKhiPage.CurrentSelectedType;
            var lianQiEffects = LuyenKhiPanel.Instance.Resources.LianQiEffects;
            var sumGroup = materials
                .GroupBy(x => new
                {
                    x.CultivationElement,
                    x.YinYangType,
                })
                .Select(k => new
                {
                    Materials = k.ToList(),
                    Effect = lianQiEffects.First(i =>
                        i.CultivationElement == k.Key.CultivationElement && i.YinYangType == k.Key.YinYangType)
                        .Effects.First(i => i.ArtifactType == selectedType),
                })
                .GroupBy(x => x.Effect.EffectType)
                .Select(group => new
                {
                    Sum = group.SelectMany(x => x.Materials).Sum(x => matConfigs.First(i => i.Tier == x.Tier).Grade),
                    group.First().Effect,
                })
                .ToArray();
            var effectGroups = sumGroup
                .Select(x =>
                {
                    var sum = x.Sum;
                    var effect = x.Effect;
                    var max = Mathf.RoundToInt(1.0f * sum / effect.Value);
                    var min = Mathf.RoundToInt(0.5f * max);
                    return new EffectDetail
                    {
                        Name = effect.Name,
                        EffectType = effect.EffectType,
                        Value = effect.Value,
                        Sum = sum,
                        Max = max,
                        Min = min,
                    };
                })
                .ToArray();
            effectDetailManager.SetDetails(effectGroups);
            luyenKhiPage.SetGrade(grade);
        }
    }
}