using System.Linq;
using Base;
using Network.Models;
using Rank;
using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    /// <summary>
    /// Một thành tựu Động phủ trong bảng (dùng lại giao diện hàng mốc sự kiện <c>ElementMilestone</c>):
    /// tên + mô tả mốc hiện tại + tiến độ, phần thưởng của mốc, nút Nhận thưởng.
    /// Tạo prefab: Tools → Động phủ → Tạo prefab Thành tựu.
    /// </summary>
    public class DongPhuAchievementElement : BaseBehaviour
    {
        [SerializeField] private ItemRewardUI moneyVipPrefab;
        [SerializeField] private ItemRewardUI skillItemPrefab;
        [SerializeField] private ConvertElementalSkill.Prefabs.ItemUI itemUIPrefab;
        [Tooltip("Tên thành tựu, mô tả mốc hiện tại và tiến độ.")]
        [SerializeField] private Text titleText;
        [SerializeField] private RectTransform content;
        [SerializeField] private Button button;
        [SerializeField] private Text buttonText;
        [SerializeField] private Color completedColor = new Color(0.49f, 0.88f, 0.49f);
        [SerializeField] private Color incompleteColor = new Color(1f, 0.66f, 0.3f);

        [Header("Nền hàng theo trạng thái (tuỳ chọn)")]
        [SerializeField] private Image rowBackground;
        [Tooltip("Đang làm (chưa đạt mốc).")]
        [SerializeField] private Sprite inProgressSprite;
        [Tooltip("Đạt mốc, chờ nhận thưởng.")]
        [SerializeField] private Sprite claimableSprite;
        [Tooltip("Đã nhận hết các mốc.")]
        [SerializeField] private Sprite completedSprite;
        [Tooltip("Dấu \"mới\" hiện khi có mốc nhận được.")]
        [SerializeField] private GameObject claimableBadge;
        [Tooltip("Dấu hoàn thành hiện khi đã nhận hết các mốc (thay nút).")]
        [SerializeField] private GameObject completedMark;

        private DongPhuAchievementPanel panel;
        public DongPhuAchievementView Achievement { get; private set; }
        public DongPhuAchievementTierView Tier { get; private set; }

        private void Start()
        {
            SetEventButton(button, () => panel.OnClaim(this));
        }

        /// <summary>Mốc đang hiển thị: mốc đầu tiên chưa nhận; nhận hết thì hiện mốc cuối (đã hoàn thành).</summary>
        public static DongPhuAchievementTierView CurrentTier(DongPhuAchievementView achievement) =>
            achievement.Tiers.FirstOrDefault(x => !x.IsClaimed) ?? achievement.Tiers.LastOrDefault();

        public static bool CanClaim(DongPhuAchievementView achievement)
        {
            var tier = CurrentTier(achievement);
            return tier != null && !tier.IsClaimed && achievement.Value >= tier.Target;
        }

        public void Init(DongPhuAchievementPanel owner, DongPhuAchievementView achievement)
        {
            panel = owner;
            Achievement = achievement;
            Tier = CurrentTier(achievement);
            foreach (Transform child in content) Destroy(child.gameObject);
            if (Tier == null)
            {
                if (titleText != null) titleText.text = achievement.Name;
                button.gameObject.SetActive(false);
                return;
            }

            var index = achievement.Tiers.IndexOf(Tier) + 1;
            var done = achievement.Value >= Tier.Target;
            var color = ColorUtility.ToHtmlStringRGB(done ? completedColor : incompleteColor);
            var description = achievement.Description.Replace("{target}", TargetText(achievement, Tier.Target));
            if (titleText != null)
            {
                titleText.supportRichText = true;
                titleText.text =
                    $"<b>{achievement.Name}</b>  <size=22>(mốc {index}/{achievement.Tiers.Count})</size>\n" +
                    $"<size=22>{description}</size>  " +
                    $"<color=#{color}>{ProgressText(achievement, Tier.Target)}</color>";
            }

            var context = new RewardUIContext
            {
                Container = content,
                MoneyVipPrefab = moneyVipPrefab,
                RandomSkillItemPrefab = skillItemPrefab,
                ItemUIPrefab = itemUIPrefab
            };
            foreach (var reward in Tier.Rewards) reward.ShowUI(context);
            UpdateButton();
        }

        public void UpdateButton()
        {
            var allDone = Tier.IsClaimed;
            var claimable = !Tier.IsClaimed && Achievement.Value >= Tier.Target;
            ApplyRowState(allDone, claimable);
            button.gameObject.SetActive(!(allDone && completedMark != null));
            if (Tier.IsClaimed)
            {
                buttonText.text = "Hoàn thành";
                button.interactable = false;
            }
            else if (Achievement.Value >= Tier.Target)
            {
                buttonText.text = "Nhận thưởng";
                button.interactable = true;
            }
            else
            {
                buttonText.text = "Chưa đạt";
                button.interactable = false;
            }
        }

        private void ApplyRowState(bool allDone, bool claimable)
        {
            if (rowBackground != null)
            {
                var sprite = allDone ? completedSprite : claimable ? claimableSprite : inProgressSprite;
                if (sprite != null) rowBackground.sprite = sprite;
            }

            if (claimableBadge != null) claimableBadge.SetActive(claimable);
            if (completedMark != null) completedMark.SetActive(allDone);
        }

        /// <summary>Chỉ số 6 (cấp động phủ): hiện tên cấp thay cho con số.</summary>
        private static bool IsLevelStat(DongPhuAchievementView a) => a.Stat == 6;

        private static string TargetText(DongPhuAchievementView a, long target) =>
            IsLevelStat(a) ? LevelName(target) : Utils.FormatCount(target);

        private static string ProgressText(DongPhuAchievementView a, long target) =>
            IsLevelStat(a)
                ? $"{LevelName(a.Value)}"
                : $"{Utils.FormatCount(System.Math.Min(a.Value, target))}/{Utils.FormatCount(target)}";

        private static string LevelName(long level)
        {
            var levels = DongPhuPanel.Instance?.DongPhuInfo?.Config?.Levels;
            var config = levels?.FirstOrDefault(x => (long)x.Level == level);
            return config?.Name ?? $"cấp {level}";
        }
    }
}
