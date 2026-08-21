using UnityEngine;
using UnityEngine.UI;

namespace Rank
{
    public class MonsterRankRewardUIContext
    {
        public Text RequirePointsText { get; init; }
        public Button ClaimButton { get; init; }
        public RectTransform Container { get; init; }
        public LuyenDan.BaseItemUI ItemUIPrefab { get; init; }
    }
}