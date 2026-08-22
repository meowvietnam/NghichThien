using UnityEngine;
using UnityEngine.UI;

namespace Rank
{
    public class MonsterRankRewardUIContext
    {
        public int Points { get; init; }
        public Text RequirePointsText { get; init; }
        public Button ClaimButton { get; init; }
        public RectTransform Container { get; init; }
        public ItemRewardUI MoneyVipPrefab { get; init; }
        public ItemRewardUI SkillItemPrefab { get; init; }
    }
}