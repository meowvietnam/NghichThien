using System;
using System.Collections;
using UnityEngine;

namespace Tutorials
{
    public class TutorialHome : BaseBehaviour
    {
        [SerializeField] private Tutorial tutorial;
        [SerializeField] private RectTransform beQuan;
        public const string tutorialHome = "isFirstTutorialHome";

        private void Start()
        {
            var isFirstTutorial = PlayerPrefs.GetInt(tutorialHome, 0) == 0;
            if (!isFirstTutorial) return;
            tutorial.OnCompleted += OnCompleted;
            ShowTutorial();
        }

        private void OnCompleted()
        {
            PlayerPrefs.SetInt(tutorialHome, 1);
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
    }
}