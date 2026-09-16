using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorials
{
    /// <summary>
    /// Hand-tut trong Thiên Tinh Thành (scene S23): chỉ người chơi bấm vào Thiên Tinh Các.
    ///
    /// LƯU Ý KIẾN TRÚC: game đổi scene bằng SceneManager.LoadScene() (KHÔNG additive - xem
    /// GameManager.ChangeSceneSuccess()), nên ngay khi bấm nút này thì scene S23 bị huỷ, kéo theo
    /// object này, coroutine của nó và cả Tutorial instance trong S23 cũng bị huỷ theo. Vì vậy
    /// phần hướng dẫn mua đồ KHÔNG thể nằm ở đây được, mà phải nằm ở scene TangKinhCac
    /// (xem TutorialTangKinhCac).
    /// </summary>
    public class TutorialThienTinhCac : BaseBehaviour
    {
        [Header("Tutorial component (Tutorial.prefab đặt trong scene S23)")]
        [SerializeField] private Tutorial tutorial;

        [Header("Nút/icon mở Thiên Tinh Các trong Thiên Tinh Thành (S23)")]
        [Tooltip("Button thật để vào Thiên Tinh Các - chính là Button nằm cùng GameObject với " +
                 "LoadSceneBtn có nameScene = \"TangKinhCac\".")]
        [SerializeField] private Button btnVaoThienTinhCac;

        /// <summary>Bật lên khi người chơi đã được dẫn bấm vào Thiên Tinh Các.
        /// TutorialTangKinhCac ở scene sau đọc cờ này để biết có chạy tiếp phần mua đồ hay không.</summary>
        public const string tutorialThienTinhCacKey = "isFirstTutorialThienTinhCac";

        /// <summary>Bật lên khi người chơi đã được dẫn bấm vào Khách Sạn (scene Home).</summary>
        public const string tutorialKhachSanKey = "isFirstTutorialKhachSan";

        private void Start()
        {
            if (tutorial == null) return;

            // Chưa mua đồ -> dẫn vào Thiên Tinh Các.
            if (PlayerPrefs.GetInt(tutorialThienTinhCacKey, 0) == 0)
            {
                if (btnVaoThienTinhCac == null) return;
                ShowPointBtnVaoThienTinhCac();
                return;
            }

            // Mua xong, quay về S23 -> dẫn tiếp sang Khách Sạn.
            if (PlayerPrefs.GetInt(TutorialTangKinhCac.tutorialTangKinhCacKey, 0) != 0)
            {
                TryShowPointKhachSan();
            }
        }

        private void OnDestroy()
        {
            if (btnVaoThienTinhCac != null)
                btnVaoThienTinhCac.onClick.RemoveListener(OnClickVaoThienTinhCac);

            if (btnVaoKhachSan != null)
                btnVaoKhachSan.onClick.RemoveListener(OnClickVaoKhachSan);
        }

        private void ShowPointBtnVaoThienTinhCac()
        {
            var steps = new[]
            {
                new TutorialStep
                {
                    TargetRect = btnVaoThienTinhCac.GetComponent<RectTransform>(),
                    Text = "Hãy chạm vào Thiên Tinh Các để tìm mua công pháp và thần thông.",
                    RequireRealAction = true
                }
            };

            btnVaoThienTinhCac.onClick.AddListener(OnClickVaoThienTinhCac);
            tutorial.ShowTutorial(steps);
        }

        private void OnClickVaoThienTinhCac()
        {
            btnVaoThienTinhCac.onClick.RemoveListener(OnClickVaoThienTinhCac);
            tutorial.CompleteInteractiveStep();

            // Xong bước này. Scene TangKinhCac sắp được load sẽ tự chạy tiếp phần mua đồ.
            PlayerPrefs.SetInt(tutorialThienTinhCacKey, 1);
        }

        // ============================================================
        // Bước kế: chỉ vào Khách Sạn (nút load scene "Home")
        // ============================================================

        private Button btnVaoKhachSan;

        private void TryShowPointKhachSan()
        {
            if (PlayerPrefs.GetInt(tutorialKhachSanKey, 0) != 0) return;

            // Tự tìm nút theo tên scene nó mở, khỏi phải gán tay trong Inspector.
            var loadBtn = FindObjectsByType<LoadSceneBtn>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .FirstOrDefault(x => x.NameScene == "Home");

            btnVaoKhachSan = loadBtn != null ? loadBtn.GetComponent<Button>() : null;
            if (btnVaoKhachSan == null) return;

            // Lưu trạng thái ngay khi kích hoạt để không bị kích hoạt lại lần 2.
            PlayerPrefs.SetInt(tutorialKhachSanKey, 1);

            var steps = new[]
            {
                new TutorialStep
                {
                    TargetRect = btnVaoKhachSan.GetComponent<RectTransform>(),
                    Text = "Hãy vào Khách Sạn để bế quan tu luyện.",
                    RequireRealAction = true
                }
            };

            btnVaoKhachSan.onClick.AddListener(OnClickVaoKhachSan);
            tutorial.ShowTutorial(steps);
        }

        private void OnClickVaoKhachSan()
        {
            btnVaoKhachSan.onClick.RemoveListener(OnClickVaoKhachSan);
            tutorial.CompleteInteractiveStep();
            PlayerPrefs.SetInt(tutorialKhachSanKey, 1);
        }
    }
}
