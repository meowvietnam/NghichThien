using UnityEngine;

namespace Tutorials
{
    public class TutorialBoCao : BaseBehaviour
    {
        [SerializeField] private Tutorial tutorial;
        [SerializeField] private RectTransform boCao;
        public const string tutorialBoCao = "isFirstTutorialBoCao";

        private void Start()
        {
            var isFirstTutorial = PlayerPrefs.GetInt(tutorialBoCao, 0) == 0;
            if (!isFirstTutorial) return;

            // Lưu trạng thái ngay khi tutorial được kích hoạt để không bị kích hoạt lại lần 2.
            PlayerPrefs.SetInt(tutorialBoCao, 1);
            ShowTutorial();
        }

        private void ShowTutorial()
        {
            TutorialStep[] steps = new[]
            {
                new TutorialStep()
                {
                    TargetRect = boCao,
                    DialogPosition = DialogPosition.TopLeft,
                    Text =
                        "Đây là Bố Cáo, nơi đạo hữu có thể tìm kiếm những nhiệm vụ đang được phát hành.\nMỗi thành trì đều có danh sách nhiệm vụ riêng, và danh sách này có thể được làm mới.\nHãy lựa chọn những nhiệm vụ phù hợp để hoàn thành và nhận Tiên Ngọc."
                },
            };
            tutorial.ShowTutorial(steps);
        }
    }
}