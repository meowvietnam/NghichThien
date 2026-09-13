using System;
using System.Collections;
using System.Linq;
using Base;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using Network;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorials
{
    /// <summary>
    /// Hand-tut trang bị Công Pháp / Thần Thông (đặt trong scene Home, chạy nối sau
    /// TutorialBeQuanFlow):
    /// 1) Mở bảng nhân vật (nút hình nhân vật góc trái).
    /// 2) Vào tab Công Pháp.
    /// 3) Chạm ô Chủ Tu để tháo công pháp đang trang bị.
    /// 4) Kéo Dưỡng Đao Thuật (Id 2) vào ô Chủ Tu.
    /// 5) Kéo công pháp vừa tháo vào ô Phụ Tu 1.
    /// 6) Vào tab Thần Thông.
    /// 7) Kéo Súc Thể (Id 14) vào ô còn trống.
    ///
    /// Thao tác trang bị ở đây là KÉO-THẢ thật (TabPanel dùng ItemSelect: chạm giữ vật phẩm trong
    /// túi rồi thả vào ô), nên mỗi bước khoanh sáng chỗ BẮT ĐẦU kéo và chỉ hoàn tất khi dữ liệu
    /// trang bị thật sự thay đổi.
    /// </summary>
    public class TutorialTrangBi : BaseBehaviour
    {
        [Header("Tutorial component (có thể dùng chung instance với TutorialBeQuanFlow)")]
        [SerializeField] private Tutorial tutorial;

        [SerializeField] private float waitUiTimeout = 15f;

        public const string tutorialTrangBiKey = "isFirstTutorialTrangBi";

        private const int CongPhapDuongDaoThuatId = 2;
        private const int ThanThongSucTheId = 14;

        private TabPanel tabPanel;

        private void Start()
        {
            if (tutorial == null) return;
            if (PlayerPrefs.GetInt(tutorialTrangBiKey, 0) != 0) return;

            StartCoroutine(IEFlow());
        }

        private IEnumerator IEFlow()
        {
            // Mạch Bế Quan kết thúc SAU khi scene này đã chạy Start(), nên phải chờ ở đây chứ
            // không được thoát sớm trong Start().
            yield return new WaitUntil(
                () => PlayerPrefs.GetInt(TutorialBeQuanFlow.tutorialBeQuanFlowKey, 0) != 0);

            // --- Mở bảng nhân vật ---
            // Nút hình nhân vật nằm trong UIHeadPanel thuộc UIManager - object DontDestroyOnLoad
            // sống xuyên scene, KHÔNG thể kéo reference từ scene Home sang. UIHeadPanel là
            // PanelGroup nên tự đăng ký vào Pool, lấy động ở đây là chuẩn nhất.
            if (!IsTabPanelOpened())
            {
                yield return WaitUntilOrTimeout(() => GetHeadButton() != null);

                var headBtn = GetHeadButton();
                if (headBtn != null)
                {
                    yield return ShowStepUntil(
                        headBtn,
                        "Chạm vào hình nhân vật để mở bảng nhân vật.",
                        IsTabPanelOpened,
                        DialogPosition.BottomRight);
                }
            }

            yield return WaitUntilOrTimeout(IsTabPanelOpened, 120f);
            if (!IsTabPanelOpened()) yield break;

            tabPanel = Pool.Get<TabPanel>();
            if (tabPanel == null) yield break;

            // --- Tab Công Pháp ---
            yield return IEOpenTab(TabPanel.ETypePanel.CongPhap, "Vào mục Công Pháp để trang bị.");

            // Công pháp đang ở ô Chủ Tu (nếu có) - lát nữa sẽ chuyển nó xuống Phụ Tu 1.
            // LƯU Ý: EquipmentManager.ChuTu1Used mặc định là ushort.MaxValue khi CHƯA trang bị gì
            // (không phải 0), nên phải dựa vào ô Chủ Tu có dữ liệu hay không mới chắc.
            var chuTuSlot = tabPanel.ChuTuSlot;
            int oldChuTuId = Client.Instance.CharacterData.EquipmentManager.ChuTu1Used;

            bool hasOldChuTu = chuTuSlot != null
                               && chuTuSlot.itemData != null
                               && oldChuTuId != ushort.MaxValue
                               && oldChuTuId != CongPhapDuongDaoThuatId;

            // --- Tháo công pháp chủ tu ---
            if (hasOldChuTu)
            {
                yield return ShowStepUntil(
                    chuTuSlot,
                    "Chạm vào công pháp đang trang bị ở Chủ Tu để tháo ra.",
                    () => chuTuSlot.itemData == null);
            }

            // --- Kéo Dưỡng Đao Thuật vào ô Chủ Tu ---
            yield return IEDragCongPhapToSlot(
                CongPhapDuongDaoThuatId,
                ETypeSlotCP.ChuTu1,
                "Kéo Dưỡng Đao Thuật thả vào ô Chủ Tu.");

            // --- Kéo công pháp cũ vào ô Phụ Tu 1 ---
            if (hasOldChuTu)
            {
                yield return IEDragCongPhapToSlot(
                    oldChuTuId,
                    ETypeSlotCP.PhuTu1,
                    "Kéo công pháp vừa tháo thả vào ô Phụ Tu 1.");
            }

            // --- Tab Thần Thông ---
            yield return IEOpenTab(TabPanel.ETypePanel.ThanThong, "Vào mục Thần Thông.");

            // --- Kéo Súc Thể vào ô trống ---
            if (!IsThanThongEquipped(ThanThongSucTheId))
            {
                yield return WaitUntilOrTimeout(
                    () => tabPanel.FindBagItem(ItemType.ThanThong, ThanThongSucTheId) != null);

                if (tabPanel.FindBagItem(ItemType.ThanThong, ThanThongSucTheId) != null)
                {
                    yield return IEDragStep(
                        () => tabPanel.FindBagItem(ItemType.ThanThong, ThanThongSucTheId),
                        () => tabPanel.FindEmptyThanThongSlot(),
                        "Kéo Súc Thể thả vào một ô thần thông còn trống.",
                        "Thả vào ô đang được khoanh sáng.",
                        () => IsThanThongEquipped(ThanThongSucTheId));
                }
            }

            PlayerPrefs.SetInt(tutorialTrangBiKey, 1);
        }

        // ============================================================
        // Các bước nhỏ
        // ============================================================

        private IEnumerator IEOpenTab(TabPanel.ETypePanel type, string text)
        {
            if (tabPanel.currentBtnWithPanel != null && tabPanel.currentBtnWithPanel.typePanel == type)
                yield break;

            var btnTab = tabPanel.GetTabButton(type);
            if (btnTab == null) yield break;

            yield return ShowStepUntil(
                btnTab,
                text,
                () => tabPanel.currentBtnWithPanel != null && tabPanel.currentBtnWithPanel.typePanel == type);

            // Danh sách trong tab được dựng lại sau khi đổi tab.
            yield return null;
        }

        private IEnumerator IEDragCongPhapToSlot(int congPhapId, ETypeSlotCP slot, string textSource)
        {
            if (tabPanel.GetTypeSlotCP(congPhapId) == slot) yield break;

            yield return WaitUntilOrTimeout(
                () => tabPanel.FindBagItem(ItemType.CongPhap, congPhapId) != null);

            if (tabPanel.FindBagItem(ItemType.CongPhap, congPhapId) == null) yield break;

            yield return IEDragStep(
                () => tabPanel.FindBagItem(ItemType.CongPhap, congPhapId),
                () => GetCongPhapSlot(slot),
                textSource,
                "Thả vào ô đang được khoanh sáng.",
                () => tabPanel.GetTypeSlotCP(congPhapId) == slot);
        }

        private Component GetCongPhapSlot(ETypeSlotCP slot)
        {
            switch (slot)
            {
                case ETypeSlotCP.ChuTu1: return tabPanel.ChuTuSlot;
                case ETypeSlotCP.PhuTu1: return tabPanel.PhuTu1Slot;
                default: return null;
            }
        }

        /// <summary>
        /// Bước kéo-thả: chưa kéo thì khoanh sáng vật phẩm nguồn, vừa nhấc lên thì chuyển khung
        /// sáng sang ô đích để người chơi biết thả vào đâu; thả trượt / huỷ thì quay lại khoanh
        /// sáng vật phẩm nguồn.
        /// </summary>
        private IEnumerator IEDragStep(Func<Component> source, Func<Component> destination,
            string textSource, string textDestination, Func<bool> isDone)
        {
            bool hasShown = false;
            bool lastDragging = false;

            while (!isDone())
            {
                bool dragging = IsDragging();

                if (!hasShown || dragging != lastDragging)
                {
                    hasShown = true;
                    lastDragging = dragging;

                    var target = dragging ? destination() : source();
                    var text = dragging ? textDestination : textSource;

                    ShowStepImmediate(target, text);
                }

                yield return null;
            }

            tutorial.CompleteInteractiveStep();
        }

        private bool IsDragging()
        {
            var select = tabPanel != null ? tabPanel.ItemSelectControl : null;
            return select != null && select.IsDragging;
        }

        /// <summary>Hiện bước ngay lập tức (startDelay = 0) - dùng khi đổi target giữa lúc kéo.</summary>
        private void ShowStepImmediate(Component target, string text)
        {
            if (target == null) return;

            var rect = target.GetComponent<RectTransform>();
            if (rect == null) return;

            tutorial.ShowTutorial(new[]
            {
                new TutorialStep
                {
                    TargetRect = rect,
                    Text = text,
                    RequireRealAction = true
                }
            }, 0f);
        }

        private Button GetHeadButton()
        {
            var headPanel = Pool.Get<UIHeadPanel>();
            return headPanel != null ? headPanel.HeadBtn : null;
        }

        private bool IsTabPanelOpened()
        {
            var panel = Pool.Get<TabPanel>();
            return panel != null && panel.gameObject.activeInHierarchy;
        }

        private bool IsThanThongEquipped(int id)
        {
            return Client.Instance.CharacterData.EquipmentManager.ThanThongEquips
                .Any(x => x.Id == id);
        }

        // ============================================================
        // Helper
        // ============================================================

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
