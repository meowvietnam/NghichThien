using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AllMaps;
using Network;
using UnityEngine;

namespace Tutorials
{
    public class TutorialAllMaps : BaseBehaviour
    {
        [SerializeField] private Tutorial tutorial;
        [SerializeField] private Renderer vanTichThanh;
        [SerializeField] private MainQuestManager mainQuestManager;
        [SerializeField] private EnemyActionEnterMapManager enemyManager;
        [SerializeField] private SideQuestManager sideQuestManager;
        private string tutorialAllMapKey = "isFirstTutorialAllMaps";
        [SerializeField] private Transform finishedTarget;

        private void Start()
        {
            tutorial.OnSetTarget += OnSetTarget;
            tutorial.OnCompleted += OnCompleted;
            var isFirstTutorial = PlayerPrefs.GetInt(tutorialAllMapKey, 0) == 0 && Client.Instance.CharacterData.QuestManager.CurrentMainQuest != 0;
            if (!isFirstTutorial) return;
            ShowTutorial();
        }

        private void OnCompleted()
        {
            var cam = CameraFollow.Instance;
            cam.target = finishedTarget;
            PlayerPrefs.SetInt(tutorialAllMapKey, 1);
        }

        private void OnSetTarget(Renderer renderer)
        {
            var cam = CameraFollow.Instance;
            cam.target = renderer.transform;
        }

        private void ShowTutorial()
        {
            StartCoroutine(IEShowTutorial());
        }

        private IEnumerator IEShowTutorial()
        {
            yield return new WaitForSeconds(8);
            List<TutorialStep> steps = new List<TutorialStep>
            {
                new()
                {
                    TargetRenderer = vanTichThanh,
                    Text =
                        "Đây là Vân Tịch Thành, một trong những thành trì quan trọng trên bản đồ.\nMỗi thành đều có những đặc sản và vật phẩm riêng.\nVân Tịch Thành nổi tiếng với dược liệu và đan lô, rất thích hợp cho các đạo hữu muốn bắt đầu con đường luyện đan."
                }
            };
            var firstMainQuest = mainQuestManager.currentMapMainQuests.FirstOrDefault();
            if (firstMainQuest != null)
            {
                var render = GetRendererInMap(firstMainQuest);
                var step = new TutorialStep()
                {
                    TargetRenderer = render,
                    Text =
                        "Những quyển trục màu vàng trên bản đồ chính là nhiệm vụ chính tuyến.\nChính tuyến được chia thành nhiều phần, hoàn thành từng phần sẽ nhận được nhiều phần thưởng.\nHãy theo dõi chúng để không bỏ lỡ những hành trình quan trọng phía trước."
                };
                steps.Add(step);
            }

            var firstEnemy = enemyManager.currentMapRandom.FirstOrDefault();
            if (firstEnemy != null)
            {
                var render = GetRendererInMap(firstEnemy);
                var step = new TutorialStep()
                {
                    TargetRenderer = render,
                    Text =
                        "Những dấu hiệu màu tím trên bản đồ là nơi quái vật xuất hiện.\nQuái vật sẽ liên tục làm mới, hãy đến đây để tìm kiếm và khiêu chiến chúng."
                };
                steps.Add(step);
            }

            var firstSideQuest = sideQuestManager.currentMapRandom.FirstOrDefault();
            if (firstSideQuest != null)
            {
                var render = GetRendererInMap(firstSideQuest);
                var step = new TutorialStep()
                {
                    TargetRenderer = render,
                    Text =
                        "Đây là Kỳ Ngộ, những cơ duyên bất ngờ có thể gặp trên hành trình.\nMỗi ngày, đạo hữu có thể hoàn thành tối đa 10 Kỳ Ngộ.\nNếu bỏ lỡ, Kỳ Ngộ sẽ tự động làm mới sau mỗi tháng.\nHoàn thành Kỳ Ngộ có thể nhận được nhiều Tiên Ngọc."
                };
                steps.Add(step);
            }

            tutorial.ShowTutorial(steps.ToArray());
        }

        private Renderer GetRendererInMap(Map map)
        {
            return map.transform.Find("Level1Move").GetChild(0).GetComponent<SpriteRenderer>();
        }
    }
}