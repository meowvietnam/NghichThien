using System;
using System.Collections;
using DG.Tweening;
using System.Linq;
using LuyenDan;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorials
{
    /// <summary>
    /// Hand-tut toàn bộ mạch Bế Quan (đặt trong scene Home):
    /// 1) Chờ popup Bế Quan mở và tut kể chuyện TutorialBeQuan chạy xong.
    /// 2) Tu luyện: chỉ nút chọn tháng (kèm lời nhắc về HP / tâm cảnh), đủ số tháng thì chỉ nút Tu Luyện.
    /// 3) Mở lại Bế Quan (popup tự đóng sau khi tu luyện - xem UIBeQuan.ActionTuLuyen).
    /// 4) Lĩnh Ngộ: học Dưỡng Đao Thuật (Id 2) rồi tới Súc Thể (Id 14).
    /// 5) Đột Phá: nâng Dưỡng Đao Thuật tới cấp yêu cầu.
    /// 6) Chỉ nút đóng popup Bế Quan.
    ///
    /// Các bước mở popup xác nhận (PopupConfirmAction) được hoàn tất NGAY khi người chơi bấm nút,
    /// không chờ tới lúc thành công - vì canvas tutorial nằm trên cùng (sortingOrder 12222), nếu
    /// còn che thì người chơi không bấm được nút Xác Nhận của popup đó.
    /// </summary>
    public class TutorialBeQuanFlow : BaseBehaviour
    {
        [Header("Tutorial component (Tutorial.prefab đặt trong scene Home)")]
        [SerializeField] private Tutorial tutorial;

        [Header("Cấu hình")]
        [Tooltip("Số tháng tu luyện cần chọn trước khi cho bấm nút Tu Luyện.")]
        [SerializeField] private int monthsRequired = 3;

        [Tooltip("Đột phá Dưỡng Đao Thuật tới cấp này thì dừng. Nếu hết nguyên liệu giữa chừng " +
                 "thì tự bỏ qua để không kẹt tutorial.")]
        [SerializeField] private int congPhapTargetLevel = 5;

        [Tooltip("Thời gian tối đa (giây) chờ UI xuất hiện ở mỗi chặng.")]
        [SerializeField] private float waitUiTimeout = 15f;

        public const string tutorialBeQuanFlowKey = "isFirstTutorialBeQuanFlow";

        private const int CongPhapDuongDaoThuatId = 2;
        private const int ThanThongSucTheId = 14;

        private UIBeQuan beQuan;

        private void Start()
        {
            if (tutorial == null) return;
            if (PlayerPrefs.GetInt(tutorialBeQuanFlowKey, 0) != 0) return;

            beQuan = FindObjectsByType<UIBeQuan>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .FirstOrDefault();
            if (beQuan == null) return;

            StartCoroutine(IEFlow());
        }

        // ============================================================
        // Mạch chính
        // ============================================================

        private IEnumerator IEFlow()
        {
            // --- Chờ người chơi mở popup Bế Quan (không đặt hạn giờ: họ có thể làm việc khác
            // trước) rồi chờ nốt tut kể chuyện TutorialBeQuan chạy xong. ---
            yield return new WaitUntil(IsBeQuanOpened);
            yield return WaitPopupAnimation(beQuan.popBeQuan);

            yield return WaitUntilOrTimeout(
                () => PlayerPrefs.GetInt(TutorialBeQuan.tutorialBeQuan, 0) != 0, 600f);

            // --- Tu luyện ---
            yield return IETuLuyen();

            // --- Lĩnh ngộ 2 quyển vừa mua ---
            yield return IELinhNgo(ItemType.CongPhap, CongPhapDuongDaoThuatId, "Dưỡng Đao Thuật");
            yield return IELinhNgo(ItemType.ThanThong, ThanThongSucTheId, "Súc Thể");

            // --- Đột phá Dưỡng Đao Thuật ---
            yield return IEDotPha();

            // --- Đóng popup Bế Quan ---
            yield return IECloseBeQuan();

            PlayerPrefs.SetInt(tutorialBeQuanFlowKey, 1);
        }

        // ============================================================
        // Tu luyện
        // ============================================================

        private IEnumerator IETuLuyen()
        {
            var slider = beQuan.SliderTuLuyen;
            var btnAdd = beQuan.BtnAddSlider;

            if (slider != null && btnAdd != null && slider.value < monthsRequired)
            {
                yield return ShowStepUntil(
                    btnAdd,
                    $"Mỗi tháng tu luyện hồi 10% khí huyết, đồng thời tu vi tăng mạnh.\n" +
                    $"Nhưng chớ lạm dụng: tâm cảnh giảm sẽ khiến tốc độ tu luyện giảm 50%.\n" +
                    $"Hãy chọn {monthsRequired} tháng.",
                    () => slider.value >= monthsRequired,
                    DialogPosition.TopLeft);
            }

            var btnTuLuyen = beQuan.BtnTuLuyen;
            if (btnTuLuyen == null) yield break;

            bool success = false;
            Action onSuccess = () => success = true;
            beQuan.OnTuLuyenSuccess += onSuccess;

            // Bấm xong sẽ hiện popup xác nhận -> hoàn tất bước ngay để không che popup đó.
            yield return ShowStepClick(btnTuLuyen, "Chạm vào đây để bắt đầu tu luyện.");

            // Tâm cảnh thấp hơn tu vi thì UIBeQuan bật 2 popup xác nhận liên tiếp
            // (cảnh báo tâm cảnh -> xác nhận tiêu hao linh thạch), bình thường chỉ 1.
            yield return GuideConfirmPopups(2);

            yield return WaitUntilOrTimeout(() => success, 60f);
            beQuan.OnTuLuyenSuccess -= onSuccess;

            // UIBeQuan tự đóng popup Bế Quan sau khi tu luyện -> phải mở lại mới đi tiếp được.
            if (!IsBeQuanOpened() && beQuan.BtnShowPopBeQuan != null)
            {
                yield return ShowStepUntil(
                    beQuan.BtnShowPopBeQuan,
                    "Hãy vào lại Bế Quan để tiếp tục.",
                    IsBeQuanOpened,
                    DialogPosition.TopLeft);

                yield return WaitPopupAnimation(beQuan.popBeQuan);
            }
        }

        // ============================================================
        // Lĩnh ngộ
        // ============================================================

        private IEnumerator IELinhNgo(ItemType type, int id, string displayName)
        {
            if (IsLearned(type, id)) yield break;

            // Mở trang Lĩnh Ngộ
            var btnPage = beQuan.GetPageButton(UIBeQuan.ETypePage.LinhNgo);
            if (btnPage != null && beQuan.currentBtnWithPage?.typePage != UIBeQuan.ETypePage.LinhNgo)
            {
                yield return ShowStepUntil(
                    btnPage,
                    "Vào Lĩnh Ngộ để học công pháp và thần thông đang có.",
                    () => beQuan.currentBtnWithPage?.typePage == UIBeQuan.ETypePage.LinhNgo);
            }

            // Chọn đúng quyển cần học
            yield return WaitUntilOrTimeout(() => beQuan.FindLinhNgoItem(type, id) != null);
            var item = beQuan.FindLinhNgoItem(type, id);
            if (item == null) yield break;

            bool selected = false;
            Action<ItemRef> onSelected = r =>
            {
                if (r != null && r.Type == type && r.Id == id) selected = true;
            };
            beQuan.OnLinhNgoItemSelected += onSelected;
            yield return ShowStepUntil(item, $"Chọn {displayName} để lĩnh ngộ.", () => selected);
            beQuan.OnLinhNgoItemSelected -= onSelected;

            // Bấm nút Lĩnh Ngộ (sau đó là popup xác nhận)
            var btnLinhHoi = beQuan.BtnLinhHoi;
            if (btnLinhHoi == null) yield break;

            bool success = false;
            Action onSuccess = () => success = true;
            beQuan.OnLinhNgoSuccess += onSuccess;

            yield return ShowStepClick(btnLinhHoi, $"Chạm vào đây để lĩnh ngộ {displayName}.");
            yield return GuideConfirmPopups();
            yield return WaitUntilOrTimeout(() => success || IsLearned(type, id), 60f);

            beQuan.OnLinhNgoSuccess -= onSuccess;
        }

        private bool IsLearned(ItemType type, int id)
        {
            var inv = Client.Instance.CharacterData.InventoryManager;
            if (type == ItemType.ThanThong)
                return inv.ThanThongOwners.Any(x => x.Id == id);

            return inv.CongPhapOwners.Any(x => x.Id == id);
        }

        // ============================================================
        // Đột phá
        // ============================================================

        private IEnumerator IEDotPha()
        {
            if (GetCongPhapLevel(CongPhapDuongDaoThuatId) >= congPhapTargetLevel) yield break;

            var btnPage = beQuan.GetPageButton(UIBeQuan.ETypePage.Update);
            if (btnPage != null && beQuan.currentBtnWithPage?.typePage != UIBeQuan.ETypePage.Update)
            {
                yield return ShowStepUntil(
                    btnPage,
                    "Vào Đột Phá để nâng cảnh giới cho công pháp.",
                    () => beQuan.currentBtnWithPage?.typePage == UIBeQuan.ETypePage.Update);
            }

            int guard = 0;
            while (GetCongPhapLevel(CongPhapDuongDaoThuatId) < congPhapTargetLevel && guard++ < 20)
            {
                // Không đủ số bản sao công pháp thì dừng êm, KHÔNG mời người chơi bấm.
                // Mỗi lần đột phá tiêu tốn (cấp hiện tại + 1) bản sao nên càng lên cấp càng tốn:
                // lên tới cấp 5 cần tổng cộng 2+3+4+5 = 14 quyển.
                if (!CanDotPha()) break;

                yield return WaitUntilOrTimeout(() => beQuan.FindDotPhaItem(CongPhapDuongDaoThuatId) != null);
                var item = beQuan.FindDotPhaItem(CongPhapDuongDaoThuatId);
                if (item == null) break;

                bool selected = false;
                Action<ItemRef> onSelected = r =>
                {
                    if (r != null && r.Id == CongPhapDuongDaoThuatId) selected = true;
                };
                beQuan.OnDotPhaItemSelected += onSelected;
                yield return ShowStepUntil(item, "Chọn Dưỡng Đao Thuật để đột phá.", () => selected);
                beQuan.OnDotPhaItemSelected -= onSelected;

                var btnUpdate = beQuan.BtnUpdateCongPhap;

                // Thiếu linh thạch / thiếu bản sao thì UIBeQuan ẩn nút này đi.
                if (btnUpdate == null || !btnUpdate.gameObject.activeInHierarchy) break;

                int levelBefore = GetCongPhapLevel(CongPhapDuongDaoThuatId);
                bool success = false;
                Action onSuccess = () => success = true;
                beQuan.OnDotPhaCongPhapSuccess += onSuccess;

                yield return ShowStepClick(btnUpdate, "Chạm vào Đột Phá để nâng cấp công pháp.");
                yield return GuideConfirmPopups();

                // Chờ kết quả nhưng có hạn: nếu cú bấm không dẫn tới đâu thì thoát êm thay vì
                // đứng hình. UIBeQuan có kẽ hở: SetUpBtnUpdate chỉ ẩn nút khi
                // (cpRef != null && cpRef.Count < cost2) - hết sạch bản sao thì cpRef = null nên
                // điều kiện sai, nút VẪN hiện, mà handler của nút lại đòi cpRef != null nên bấm
                // vào không có gì xảy ra, cũng không có popup nào.
                yield return WaitUntilOrTimeout(
                    () => success || GetCongPhapLevel(CongPhapDuongDaoThuatId) > levelBefore, 20f);

                beQuan.OnDotPhaCongPhapSuccess -= onSuccess;

                if (!success && GetCongPhapLevel(CongPhapDuongDaoThuatId) <= levelBefore) break;

                yield return null;
            }
        }

        /// <summary>
        /// Đủ điều kiện đột phá chưa - soi theo đúng công thức của UIBeQuan:
        /// cost2 = cấp hiện tại + 1 bản sao công pháp.
        /// </summary>
        private bool CanDotPha()
        {
            int required = GetCongPhapLevel(CongPhapDuongDaoThuatId) + 1;

            var cpRef = Client.Instance.CharacterData.InventoryManager.CongPhaps
                .FirstOrDefault(x => x.Id == CongPhapDuongDaoThuatId);

            return cpRef != null && cpRef.Count >= required;
        }

        private int GetCongPhapLevel(int id)
        {
            var owners = Client.Instance.CharacterData.InventoryManager.CongPhapOwners;
            var key = new CreateCharacter.CongPhap { Id = (ushort)id };
            return owners.TryGetValue(key, out var cp) ? cp.Level : 0;
        }

        // ============================================================
        // Đóng popup Bế Quan
        // ============================================================

        private IEnumerator IECloseBeQuan()
        {
            if (!IsBeQuanOpened()) yield break;

            var btnClose = beQuan.popBeQuan != null ? beQuan.popBeQuan.BtnClose : null;
            if (btnClose == null) yield break;

            yield return ShowStepUntil(btnClose, "Chạm vào đây để rời khỏi Bế Quan.", () => !IsBeQuanOpened());
        }

        private bool IsBeQuanOpened()
        {
            return beQuan != null
                   && beQuan.popBeQuan != null
                   && beQuan.popBeQuan.gameObject.activeInHierarchy;
        }

        // ============================================================
        // Helper dùng chung
        // ============================================================

        /// <summary>Khoanh sáng target và chờ tới khi điều kiện thật xảy ra mới đi tiếp.</summary>
        private IEnumerator ShowStepUntil(Component target, string text, Func<bool> isDone,
            DialogPosition position = DialogPosition.TopRight)
        {
            if (target == null || isDone == null || isDone()) yield break;

            var rect = target.GetComponent<RectTransform>();
            if (rect == null) yield break;

            tutorial.ShowTutorial(new[]
            {
                new TutorialStep
                {
                    TargetRect = rect,
                    Text = text,
                    DialogPosition = position,
                    RequireRealAction = true
                }
            });

            yield return new WaitUntil(isDone);
            tutorial.CompleteInteractiveStep();
        }

        /// <summary>
        /// Khoanh sáng một nút và hoàn tất NGAY khi nút đó được bấm. Dùng cho các nút mở popup
        /// xác nhận: phải tắt lớp phủ tutorial ngay, nếu không người chơi bị chặn không bấm được
        /// nút Xác Nhận của popup.
        /// </summary>
        private IEnumerator ShowStepClick(Button target, string text,
            DialogPosition position = DialogPosition.TopRight)
        {
            if (target == null) yield break;

            var rect = target.GetComponent<RectTransform>();
            if (rect == null) yield break;

            bool clicked = false;
            UnityEngine.Events.UnityAction onClick = () => clicked = true;
            target.onClick.AddListener(onClick);

            tutorial.ShowTutorial(new[]
            {
                new TutorialStep
                {
                    TargetRect = rect,
                    Text = text,
                    DialogPosition = position,
                    RequireRealAction = true
                }
            });

            yield return new WaitUntil(() => clicked);

            target.onClick.RemoveListener(onClick);
            tutorial.CompleteInteractiveStep();
        }

        /// <summary>
        /// Hướng dẫn bấm Xác Nhận trên popup PopupConfirmAction, chịu được cả trường hợp một
        /// hành động bật liên tiếp nhiều popup.
        /// </summary>
        private IEnumerator GuideConfirmPopups(int maxPopup = 1)
        {
            for (int i = 0; i < maxPopup; i++)
            {
                // Popup có thể không xuất hiện (VD tâm cảnh vẫn tốt) -> chờ ngắn rồi thôi.
                yield return WaitUntilOrTimeout(IsConfirmPopupOpened, 2f);
                if (!IsConfirmPopupOpened()) yield break;

                var popup = Pool.Get<PopupConfirmAction>();
                yield return WaitPopupAnimation(popup);

                // Mốc hoàn tất: popup tắt HOẶC nội dung đổi (popup kế tiếp dùng lại cùng instance).
                string content = popup.ContentText;

                yield return ShowStepUntil(
                    popup.BtnConfirm,
                    "Chạm vào Xác Nhận để tiếp tục.",
                    () => !IsConfirmPopupOpened() || popup.ContentText != content);
            }
        }

        private bool IsConfirmPopupOpened()
        {
            var popup = Pool.Get<PopupConfirmAction>();
            return popup != null && popup.gameObject.activeInHierarchy;
        }

        /// <summary>
        /// Chờ hiệu ứng DOScale của popup chạy xong rồi mới khoanh sáng, tránh tính khung sáng
        /// lúc popup còn đang phình ra khiến vị trí bị lệch.
        /// </summary>
        private IEnumerator WaitPopupAnimation(Component popup)
        {
            if (popup == null) yield break;

            float timeLeft = 2f;
            while (timeLeft > 0f && DOTween.IsTweening(popup.transform))
            {
                timeLeft -= Time.deltaTime;
                yield return null;
            }

            // thêm 1 frame cho layout ổn định
            yield return null;
        }

        private IEnumerator WaitUntilOrTimeout(Func<bool> condition, float timeout = -1f)
        {
            float timeLeft = timeout > 0f ? timeout : waitUiTimeout;
            while (timeLeft > 0f && !condition())
            {
                timeLeft -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
