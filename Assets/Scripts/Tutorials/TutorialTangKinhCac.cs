using System.Collections;
using System.Linq;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using UnityEngine;

namespace Tutorials
{
    /// <summary>
    /// Hand-tut bên trong Thiên Tinh Các (scene TangKinhCac):
    /// 1) Mua Công Pháp Dưỡng Đao Thuật (Id = 2) đủ 3 quyển trong một lượt.
    /// 2) Mua Thần Thông Súc Thể (Id = 14).
    /// 3) Bấm Xác Nhận, chờ giao dịch thành công.
    /// 4) Bấm nút Quay Lại (đóng popup shop).
    /// 5) Bấm nút Rời Đi (BackSceneBtn) để quay về Thiên Tinh Thành.
    ///
    /// Phải nằm ở scene TangKinhCac (không phải S23) vì game đổi scene bằng
    /// SceneManager.LoadScene() không additive: bấm vào Thiên Tinh Các là S23 bị huỷ sạch.
    ///
    /// ShopUI là singleton dùng chung (đăng ký qua Pool trong ShopUI.Init()) và được
    /// ShopScene.InitBG() gọi ShowInitShop() khi scene này load, nên ở đây chỉ cần chờ (poll)
    /// tới lúc nó hiện lên đúng loại shop Skill là được - không gán tay qua Inspector.
    /// </summary>
    public class TutorialTangKinhCac : BaseBehaviour
    {
        [Header("Tutorial component (Tutorial.prefab đặt trong scene TangKinhCac)")]
        [SerializeField] private Tutorial tutorial;

        [Tooltip("Thời gian tối đa (giây) chờ ShopUI hiện lên sau khi vào scene. Quá hạn thì bỏ " +
                 "qua phần hướng dẫn mua đồ để không kẹt tutorial.")]
        [SerializeField] private float waitShopTimeout = 10f;

        public const string tutorialTangKinhCacKey = "isFirstTutorialTangKinhCac";

        private const int CongPhapDuongDaoThuatId = 2;
        private const int ThanThongSucTheId = 14;
        private const int RequiredCongPhapCount = 3;

        private ShopUI shopUI;
        private UnityEngine.UI.Button btnLeaveScene;

        private void Start()
        {
            if (tutorial == null) return;

            // Chỉ chạy khi đã đi đúng mạch tutorial (vừa được dẫn bấm vào Thiên Tinh Các ở S23)
            // và phần này chưa làm xong.
            if (PlayerPrefs.GetInt(TutorialThienTinhCac.tutorialThienTinhCacKey, 0) == 0) return;
            if (PlayerPrefs.GetInt(tutorialTangKinhCacKey, 0) != 0) return;

            StartCoroutine(IEWaitForShopThenGuideBuy());
        }

        private void OnDestroy()
        {
            if (shopUI == null) return;

            shopUI.OnBasketChanged -= OnBasketChangedWaitingCongPhap;
            shopUI.OnBasketChanged -= OnBasketChangedWaitingThanThong;
            shopUI.OnBuySuccess -= OnBuySuccess;

            var btnClose = shopUI.BtnClose;
            if (btnClose != null) btnClose.onClick.RemoveListener(OnClickBack);

            if (btnLeaveScene != null) btnLeaveScene.onClick.RemoveListener(OnClickLeaveScene);
        }

        // ============================================================
        // Chờ ShopUI hiện lên + danh sách NPC bán render xong
        // ============================================================

        private IEnumerator IEWaitForShopThenGuideBuy()
        {
            float timeLeft = waitShopTimeout;
            ShopUI shop = null;

            while (timeLeft > 0f)
            {
                shop = Pool.Get<ShopUI>();
                if (shop != null && shop.gameObject.activeInHierarchy && shop.eShopType == EShopType.Skill)
                    break;

                timeLeft -= Time.deltaTime;
                yield return null;
            }

            if (shop == null || !shop.gameObject.activeInHierarchy || shop.eShopType != EShopType.Skill)
            {
                FinishTutorial();
                yield break;
            }

            shopUI = shop;

            // Danh sách vật phẩm NPC bán được render sau khi server trả kết quả LOAD_SHOP.
            timeLeft = waitShopTimeout;
            while (timeLeft > 0f &&
                   shopUI.FindNpcItem(ItemType.CongPhap, CongPhapDuongDaoThuatId) == null &&
                   shopUI.FindNpcItem(ItemType.ThanThong, ThanThongSucTheId) == null)
            {
                timeLeft -= Time.deltaTime;
                yield return null;
            }

            ShowBuyCongPhap();
        }

        // ============================================================
        // Mua Công Pháp Dưỡng Đao Thuật (Id = 2) đủ 3 quyển
        // ============================================================

        private void ShowBuyCongPhap()
        {
            var item = shopUI.FindNpcItem(ItemType.CongPhap, CongPhapDuongDaoThuatId);
            if (item == null)
            {
                ShowBuyThanThong();
                return;
            }

            if (shopUI.GetBuyCount(ItemType.CongPhap, CongPhapDuongDaoThuatId) >= RequiredCongPhapCount)
            {
                ShowBuyThanThong();
                return;
            }

            var steps = new[]
            {
                new TutorialStep
                {
                    TargetRect = item.GetComponent<RectTransform>(),
                    Text =
                        $"Hãy chạm vào Dưỡng Đao Thuật {RequiredCongPhapCount} lần để thêm đủ {RequiredCongPhapCount} quyển vào giỏ mua.",
                    RequireRealAction = true,
                    DialogPosition = DialogPosition.TopLeft
                }
            };

            shopUI.OnBasketChanged += OnBasketChangedWaitingCongPhap;
            tutorial.ShowTutorial(steps);
        }

        private void OnBasketChangedWaitingCongPhap()
        {
            if (shopUI.GetBuyCount(ItemType.CongPhap, CongPhapDuongDaoThuatId) < RequiredCongPhapCount) return;

            shopUI.OnBasketChanged -= OnBasketChangedWaitingCongPhap;
            tutorial.CompleteInteractiveStep();
            ShowBuyThanThong();
        }

        // ============================================================
        // Mua Thần Thông Súc Thể (Id = 14)
        // ============================================================

        private void ShowBuyThanThong()
        {
            var item = shopUI.FindNpcItem(ItemType.ThanThong, ThanThongSucTheId);
            if (item == null)
            {
                ShowConfirm();
                return;
            }

            if (shopUI.GetBuyCount(ItemType.ThanThong, ThanThongSucTheId) >= 1)
            {
                ShowConfirm();
                return;
            }

            var steps = new[]
            {
                new TutorialStep
                {
                    TargetRect = item.GetComponent<RectTransform>(),
                    Text = "Hãy chạm vào Súc Thể để thêm thần thông này vào giỏ mua.",
                    RequireRealAction = true,
                    DialogPosition = DialogPosition.TopLeft
                }
            };

            shopUI.OnBasketChanged += OnBasketChangedWaitingThanThong;
            tutorial.ShowTutorial(steps);
        }

        private void OnBasketChangedWaitingThanThong()
        {
            if (shopUI.GetBuyCount(ItemType.ThanThong, ThanThongSucTheId) < 1) return;

            shopUI.OnBasketChanged -= OnBasketChangedWaitingThanThong;
            tutorial.CompleteInteractiveStep();
            ShowConfirm();
        }

        // ============================================================
        // Nút Xác Nhận -> chờ mua thành công
        // ============================================================

        private void ShowConfirm()
        {
            var btnConfirm = shopUI.BtnConfirm;
            if (btnConfirm == null)
            {
                FinishTutorial();
                return;
            }

            var steps = new[]
            {
                new TutorialStep
                {
                    TargetRect = btnConfirm.GetComponent<RectTransform>(),
                    Text = "Chạm vào Xác Nhận để hoàn tất mua hàng.",
                    RequireRealAction = true
                }
            };

            shopUI.OnBuySuccess += OnBuySuccess;
            tutorial.ShowTutorial(steps);
        }

        private void OnBuySuccess()
        {
            shopUI.OnBuySuccess -= OnBuySuccess;
            tutorial.CompleteInteractiveStep();
            ShowBack();
        }

        // ============================================================
        // Nút Quay Lại
        // ============================================================

        private void ShowBack()
        {
            var btnClose = shopUI.BtnClose;
            if (btnClose == null)
            {
                FinishTutorial();
                return;
            }

            var steps = new[]
            {
                new TutorialStep
                {
                    TargetRect = btnClose.GetComponent<RectTransform>(),
                    Text = "Chạm vào đây để quay lại.",
                    RequireRealAction = true
                }
            };

            btnClose.onClick.AddListener(OnClickBack);
            tutorial.ShowTutorial(steps);
        }

        private void OnClickBack()
        {
            shopUI.BtnClose.onClick.RemoveListener(OnClickBack);
            tutorial.CompleteInteractiveStep();
            ShowLeaveScene();
        }

        // ============================================================
        // Nút Rời Đi: quay về Thiên Tinh Thành (S23)
        // ============================================================

        private void ShowLeaveScene()
        {
            // Nút rời đi nằm trong prefab dùng chung BackSceneCanvas (object "CloseTab" gắn
            // BackSceneBtn, bắn EventList.GamePlay.BACK_SCENE). Tìm thẳng trong scene thay vì bắt
            // gán tay qua Inspector - prefab này có mặt ở mọi scene con nên cách này dùng lại được
            // cho các bước hand-tut sau (Khách sạn, Bế Quan...).
            var backSceneBtn = FindObjectsByType<BackSceneBtn>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .FirstOrDefault();

            btnLeaveScene = backSceneBtn != null
                ? backSceneBtn.GetComponent<UnityEngine.UI.Button>()
                : null;

            if (btnLeaveScene == null)
            {
                FinishTutorial();
                return;
            }

            var steps = new[]
            {
                new TutorialStep
                {
                    TargetRect = btnLeaveScene.GetComponent<RectTransform>(),
                    Text = "Chạm vào đây để rời khỏi Thiên Tinh Các.",
                    RequireRealAction = true,
                    DialogPosition = DialogPosition.TopLeft
                }
            };

            btnLeaveScene.onClick.AddListener(OnClickLeaveScene);
            tutorial.ShowTutorial(steps);
        }

        private void OnClickLeaveScene()
        {
            btnLeaveScene.onClick.RemoveListener(OnClickLeaveScene);
            tutorial.CompleteInteractiveStep();
            FinishTutorial();
        }

        private void FinishTutorial()
        {
            PlayerPrefs.SetInt(tutorialTangKinhCacKey, 1);
        }
    }
}
