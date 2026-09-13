using System.Collections;
using System.Linq;
using UnityEngine;

namespace Tutorials
{
    /// <summary>
    /// Khách sạn (scene Home):
    /// 1) Tut kể chuyện giới thiệu Bế Quan (giữ nguyên như cũ).
    /// 2) Nối tiếp: hand-tut bắt người chơi bấm thật vào nút Bế Quan.
    /// </summary>
    public class TutorialHome : BaseBehaviour
    {
        [SerializeField] private Tutorial tutorial;
        [SerializeField] private RectTransform beQuan;
        public const string tutorialHome = "isFirstTutorialHome";

        /// <summary>Bật lên khi người chơi đã bấm vào Bế Quan theo hướng dẫn.</summary>
        public const string tutorialClickBeQuanKey = "isFirstTutorialClickBeQuan";

        private void Start()
        {
            if (tutorial == null) return;

            var isFirstTutorial = PlayerPrefs.GetInt(tutorialHome, 0) == 0;
            if (isFirstTutorial)
            {
                tutorial.OnCompleted += OnCompleted;
                ShowTutorial();
                return;
            }

            // Phần kể chuyện đã xem rồi -> vào thẳng bước bấm Bế Quan nếu chưa làm.
            TryShowClickBeQuan();
        }

        private void OnCompleted()
        {
            // OnCompleted bắn ra sau MỖI lần ShowTutorial() kết thúc, kể cả bước hand-tut bên dưới
            // (dùng chung component tutorial này), nên phải chặn để chỉ chạy đúng một lần.
            if (PlayerPrefs.GetInt(tutorialHome, 0) != 0) return;

            PlayerPrefs.SetInt(tutorialHome, 1);
            TryShowClickBeQuan();
        }

        private void ShowTutorial()
        {
            TutorialStep[] steps = new[]
            {
                new TutorialStep()
                {
                    TargetRect = beQuan,
                    Text =
                        "Muốn nâng cao tu vi? Hãy bắt đầu từ Bế Quan.\nTại đây, đạo hữu có thể tu luyện, lĩnh ngộ và đột phá, từng bước nâng cao thực lực.",
                    DialogPosition = DialogPosition.TopLeft
                }
            };
            tutorial.ShowTutorial(steps);
        }

        // ============================================================
        // Hand-tut: bấm thật vào Bế Quan
        // ============================================================

        private void TryShowClickBeQuan()
        {
            if (PlayerPrefs.GetInt(tutorialClickBeQuanKey, 0) != 0) return;
            if (beQuan == null) return;

            StartCoroutine(IEClickBeQuan());
        }

        private IEnumerator IEClickBeQuan()
        {
            var steps = new[]
            {
                new TutorialStep
                {
                    TargetRect = beQuan,
                    Text = "Hãy chạm vào Bế Quan để bắt đầu tu luyện.",
                    DialogPosition = DialogPosition.TopLeft,
                    RequireRealAction = true
                }
            };

            tutorial.ShowTutorial(steps);

            // Hoàn tất khi popup Bế Quan thật sự mở, thay vì bắt listener của Button - như vậy
            // không phụ thuộc việc RectTransform "beQuan" có đúng là object gắn Button hay không.
            yield return new WaitUntil(IsBeQuanOpened);

            tutorial.CompleteInteractiveStep();
            PlayerPrefs.SetInt(tutorialClickBeQuanKey, 1);
        }

        private bool IsBeQuanOpened()
        {
            var ui = FindObjectsByType<UIBeQuan>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .FirstOrDefault();

            return ui != null
                   && ui.popBeQuan != null
                   && ui.popBeQuan.gameObject.activeInHierarchy;
        }
    }
}
