using System.Collections;
using UnityEngine;

namespace Tutorials
{
    public class TutorialBeQuan : BaseBehaviour
    {
        [SerializeField] private Tutorial tutorial;
        [SerializeField] private RectTransform tuLuyen;
        [SerializeField] private RectTransform linhNgo;
        [SerializeField] private RectTransform dotPha;
        public const string tutorialBeQuan = "isFirstTutorialBeQuan";

        private void Start()
        {
            var isFirstTutorial = PlayerPrefs.GetInt(tutorialBeQuan, 0) == 0;
            if (!isFirstTutorial) return;
            tutorial.OnCompleted += OnCompleted;
            ShowTutorial();
        }

        private void OnCompleted()
        {
            PlayerPrefs.SetInt(tutorialBeQuan, 1);
        }

        private void ShowTutorial()
        {
            TutorialStep[] steps = new[]
            {
                new TutorialStep()
                {
                    TargetRect = tuLuyen,
                    Text =
                        "Tu luyện giúp đạo hữu từng bước nâng cao tu vi.\nMỗi tháng, tu luyện có thể hồi phục 10% HP.\nĐạo hữu cũng có thể tu luyện nhanh để tiết kiệm thời gian, nhưng đừng quá lạm dụng.\nKhi tâm cảnh suy giảm, tốc độ tu luyện cũng sẽ bị ảnh hưởng.",
                },
                new TutorialStep()
                {
                    TargetRect = linhNgo,
                    Text =
                        "Lĩnh Ngộ là nơi đạo hữu học tập những sách thần thông và công pháp mà mình sở hữu.\nHãy lĩnh ngộ để biến những bí tịch trong tay thành sức mạnh thực sự.",
                },
                new TutorialStep()
                {
                    TargetRect = dotPha,
                    Text =
                        "Sau khi đã lĩnh ngộ công pháp, hãy đến Đột Phá để tiếp tục nâng cao cảnh giới của chúng.\nMỗi lần đột phá, thực lực của đạo hữu sẽ tiến thêm một bước.",
                }
            };
            tutorial.ShowTutorial(steps);
        }
    }
}