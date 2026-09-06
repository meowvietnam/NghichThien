
using UnityEngine;
using UnityEngine.UI;

namespace Rank
{
    public class RewardUIContext
    {
        public int Points { get; init; }
        public Text RequirePointsText { get; init; }
        public Button ClaimButton { get; init; }
        public RectTransform Container { get; init; }
        public ItemRewardUI MoneyVipPrefab { get; init; }
        public ItemRewardUI RandomSkillItemPrefab { get; init; }
        public ConvertElementalSkill.Prefabs.ItemUI ItemUIPrefab { get; init; }
        public ConvertElementalSkill.Prefabs.SkillItemUI SkillItemUIPrefab { get; init; }
    }
}