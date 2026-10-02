using System;
using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using DG.Tweening;
using Network;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    /// <summary>
    /// Bảng thành tựu Động phủ (dùng lại giao diện bảng mốc sự kiện <c>MilestoneEventPanel</c>).
    /// Danh sách thành tựu, mốc và phần thưởng do server cấu hình (Data/dong-phu-achievements.json);
    /// client chỉ hiển thị và gửi yêu cầu nhận thưởng.
    /// </summary>
    public class DongPhuAchievementPanel : BaseBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Text titleText;
        [SerializeField] private Text detailText;
        [SerializeField] private RectTransform content;
        [SerializeField] private DongPhuAchievementElement elementPrefab;

        private readonly List<DongPhuAchievementElement> elements = new();
        private List<DongPhuAchievementView> achievements = new();
        private Tween tween;
        private bool loading;
        private bool claiming;

        // Mặc định là thành tựu Động phủ; nơi khác (vd. Tông môn chiến) gọi Configure để dùng lại bảng này.
        private MessageTag getTag = MessageTag.GetDongPhuAchievements;
        private MessageTag claimTag = MessageTag.ClaimDongPhuAchievement;
        private string title = "Thành Tựu Động Phủ";
        private Action<int> claimableChanged = AchievementButton.SetClaimableCount;

        /// <summary>
        /// Dùng lại bảng cho bộ thành tựu khác: tag lấy danh sách, tag nhận thưởng (cùng kiểu message với Động phủ),
        /// tiêu đề và hàm nhận số mốc đang nhận được (chấm đỏ).
        /// </summary>
        public void Configure(MessageTag get, MessageTag claim, string panelTitle, Action<int> onClaimableChanged)
        {
            getTag = get;
            claimTag = claim;
            title = panelTitle;
            claimableChanged = onClaimableChanged;
        }

        private void Start()
        {
            SetEventButton(closeButton, Hide);
        }

        public void Show()
        {
            tween?.Kill();
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            tween = transform.DOScale(Vector3.one, 0.25f);
            Load();
        }

        public void Hide()
        {
            tween?.Kill();
            tween = transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => gameObject.SetActive(false));
        }

        private static void Notify(string text)
        {
            var notify = DongPhuPanel.Instance != null ? DongPhuPanel.Instance.NotifyGroup : NotifyGroup.Instance;
            notify?.Show(text);
        }

        private async void Load()
        {
            if (loading) return;
            loading = true;
            try
            {
                var reader = await Client.Instance.RequestWait(getTag);
                var result = reader.MemRead<GetDongPhuAchievementsResult>();
                if (this == null) return;
                if (!result.Success)
                {
                    Notify(result.Message);
                    return;
                }

                achievements = result.Achievements ?? new List<DongPhuAchievementView>();
                Render();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                Notify("Không tải được thành tựu, vui lòng thử lại");
            }
            finally
            {
                loading = false;
            }
        }

        private void Render()
        {
            foreach (var e in elements) Destroy(e.gameObject);
            elements.Clear();
            if (titleText != null) titleText.text = title;
            var claimable = achievements.Count(DongPhuAchievementElement.CanClaim);
            var completed = achievements.Count(a => a.Tiers.Count > 0 && a.Tiers.All(t => t.IsClaimed));
            if (detailText != null)
                detailText.text = $"Đạt mốc để nhận thưởng. Đã hoàn thành {completed}/{achievements.Count} thành tựu" +
                                  (claimable > 0 ? $" - có {claimable} mốc chờ nhận" : string.Empty);

            // Mốc nhận được lên đầu, thành tựu đã hoàn thành xuống cuối.
            var ordered = achievements
                .OrderByDescending(DongPhuAchievementElement.CanClaim)
                .ThenBy(a => a.Tiers.Count > 0 && a.Tiers.All(t => t.IsClaimed));
            foreach (var achievement in ordered)
            {
                var element = Instantiate(elementPrefab, content);
                element.Init(this, achievement);
                elements.Add(element);
            }

            claimableChanged?.Invoke(claimable);
        }

        public async void OnClaim(DongPhuAchievementElement element)
        {
            if (claiming || element.Tier == null) return;
            claiming = true;
            try
            {
                var request = new ClaimDongPhuAchievementRequest
                {
                    AchievementId = element.Achievement.Id,
                    TierId = element.Tier.Id
                };
                var reader = await Client.Instance.MemRequestWait(claimTag, request);
                var result = reader.MemRead<ClaimDongPhuAchievementResult>();
                if (this == null) return;
                if (!result.Success)
                {
                    Notify(result.Message);
                    return;
                }

                // Server đã phát thưởng: áp vào dữ liệu local giống nhận mốc sự kiện.
                var charData = Client.Instance.CharacterData;
                foreach (var reward in result.Rewards) reward.Claim(charData);
                Notify($"Bạn nhận được: {string.Join(", ", result.Rewards.Select(x => x.GetRichText()))}");
                element.Tier.IsClaimed = true;
                Render();
                claimableChanged?.Invoke(result.ClaimableCount);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                Notify("Nhận thưởng thất bại, vui lòng thử lại");
            }
            finally
            {
                claiming = false;
            }
        }
    }
}
