using UnityEngine;

namespace Tutorials
{
    public class TutorialLuyenDan : BaseBehaviour
    {
        [SerializeField] private Tutorial tutorial;
        [SerializeField] private RectTransform luyenDan;
        public const string tutorialLuyenDan = "isFirstTutorialLuyenDan";

        private void Start()
        {
            var isFirstTutorial = PlayerPrefs.GetInt(tutorialLuyenDan, 0) == 0;
            if (!isFirstTutorial) return;
            tutorial.OnCompleted += OnCompleted;
            ShowTutorial();
        }

        private void OnCompleted()
        {
            PlayerPrefs.SetInt(tutorialLuyenDan, 1);
        }

        private void ShowTutorial()
        {
            TutorialStep[] steps = new[]
            {
                new TutorialStep()
                {
                    TargetRect = luyenDan,
                    Text = "Muốn trở thành một luyện đan sư? Hãy bắt đầu bằng việc lựa chọn Đan Lô."
                },

                new TutorialStep()
                {
                    TargetRect = luyenDan,
                    Text = "Khi chưa có Đan Phương, tỷ lệ luyện đan thành công là 25%."
                },

                new TutorialStep()
                {
                    TargetRect = luyenDan,
                    Text = "Khi đã sở hữu Đan Phương, tỷ lệ thành công sẽ tăng lên 100%."
                },

                new TutorialStep()
                {
                    TargetRect = luyenDan,
                    Text = "Đan Lô phẩm chất càng cao, tỷ lệ luyện đan thành công càng lớn."
                },

                new TutorialStep()
                {
                    TargetRect = luyenDan,
                    Text = "Đan Lô Nhất Phẩm và Nhị Phẩm có thể mua tại Vân Tịch Thành."
                },

                new TutorialStep()
                {
                    TargetRect = luyenDan,
                    Text = "Đan Lô Tam Phẩm trở lên có thể nhận được khi quay thưởng tại mục Đan Dược."
                },

                new TutorialStep()
                {
                    TargetRect = luyenDan,
                    Text = "Ngoài ra, đạo hữu có thể tìm mua Đan Lô cao phẩm tại các buổi đấu giá."
                },
            };
            tutorial.ShowTutorial(steps);
        }
    }
}