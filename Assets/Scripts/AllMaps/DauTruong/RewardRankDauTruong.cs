using CreateCharacter;
using LongDH.LongDHModules;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps.DauTruong
{
    public class RewardRankDauTruong : BaseBehaviour
    {
        [SerializeField] private Button btnReward;
        [SerializeField] private Text btnRewardText;
        [SerializeField] private Image iconRank;
        [SerializeField] private Image iconRankName;
        [SerializeField] private Text rewardTienNgocText;
        [SerializeField] private Sprite[] spriteRefRank => ArenaManager.Instance.spriteRefRank;
        [SerializeField] private Sprite[] spriteRefRankName => ArenaManager.Instance.spriteRefRankName;
        public ETypeArenaRank ETypeArenaRank => Reward.Type;
        public RankReward Reward { get; private set; }

        public void Init(RankReward rankReward)
        {
            Reward = rankReward;
            if (ArenaManager.Instance.CurrentTypeRank < ETypeArenaRank)
            {
                btnRewardText.text = $"Nhận thưởng";
                btnReward.interactable = false;
            }
            else
            {
                UpdateClaim();
            }

            InitVisual();
        }

        public void UpdateClaim()
        {
            if (!Reward.IsClaim)
            {
                btnRewardText.text = $"Nhận thưởng";
                btnReward.interactable = true;
            }
            else
            {
                btnRewardText.text = $"Đã nhận";
                btnReward.interactable = false;
            }
        }

        public void InitVisual()
        {
            SetEventButton(btnReward, () => { ArenaManager.Instance.Reward(Reward.Id); });
            iconRank.sprite = spriteRefRank[(int)ETypeArenaRank - 1];
            iconRank.SetNativeSize();
            iconRankName.sprite = spriteRefRankName[(int)ETypeArenaRank - 1];
            iconRankName.SetNativeSize();
            rewardTienNgocText.text = ArenaManager.TypeRankArenaWithReward[ETypeArenaRank].ToString();
        }
    }
}