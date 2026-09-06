using UnityEngine;
using UnityEngine.UI;
namespace Rank
{
    public class ElementReward : BaseBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private ItemRewardUI moneyVipPrefab;
        [SerializeField] private ItemRewardUI skillPrefab;
        [SerializeField] private ConvertElementalSkill.Prefabs.ItemUI itemUIPrefab;
        [SerializeField] private Button rewardButton;
        [SerializeField] private RectTransform content;

        public void Init(int points, MonsterRankReward reward)
        {
            var context = CreateContext(points);
            reward.ShowUI(context);
        }
        
        private RewardUIContext CreateContext(int points)
        {
            return new RewardUIContext()
            {
                Points = points,
                MoneyVipPrefab = moneyVipPrefab,
                RandomSkillItemPrefab =  skillPrefab,
                RequirePointsText = text,
                Container =  content,
                ClaimButton = rewardButton,
                ItemUIPrefab = itemUIPrefab
            };
        }
    }
}