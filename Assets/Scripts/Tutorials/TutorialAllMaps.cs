using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AllMaps;
using LongDH.LongDHModules;
using Network;
using UnityEngine;

namespace Tutorials
{
    public class TutorialAllMaps : BaseBehaviour
    {
        [SerializeField] private Tutorial tutorial;
        [SerializeField] private Renderer vanTichThanh;
        [SerializeField] private Renderer thienTinhThanh;
        [SerializeField] private Renderer datPhongThanh;
        [SerializeField] private MainQuestManager mainQuestManager;
        [SerializeField] private EnemyActionEnterMapManager enemyManager;
        [SerializeField] private SideQuestManager sideQuestManager;
        public const string tutorialAllMapKey = "isFirstTutorialAllMaps";
        [SerializeField] private Transform finishedTarget;
        [SerializeField] private Camera2DDrag camera2DDrag;

        private void Start()
        {
            tutorial.OnSetTarget += OnSetTarget;
            tutorial.OnCompleted += OnCompleted;
            Observer.AddObserver(EventList.GamePlay.MOVE_MAP_SUCCESS, OnMoveMap);
        }

        private void OnMoveMap()
        {
            CheckTutorial();
        }

        private void OnDestroy()
        {
            Observer.RemoveObserver(EventList.GamePlay.MOVE_MAP_SUCCESS, OnMoveMap);
        }

        private void CheckTutorial()
        {
            var isFirstTutorial = PlayerPrefs.GetInt(tutorialAllMapKey, 0) == 0 &&
                                  Client.Instance.CharacterData.QuestManager.CurrentMainQuest != 0;
            Debug.Log(isFirstTutorial);
            if (!isFirstTutorial) return;
            ShowTutorial();
        }

        private void OnCompleted()
        {
            var cam = CameraFollow.Instance;
            cam.target = finishedTarget;
            PlayerPrefs.SetInt(tutorialAllMapKey, 1);
            camera2DDrag.enabled = true;
        }

        private void OnSetTarget(Renderer renderer)
        {
            var cam = CameraFollow.Instance;
            cam.target = renderer.transform;
        }

        private void ShowTutorial()
        {
            camera2DDrag.enabled = false;
            StartCoroutine(IEShowTutorial());
        }

        private IEnumerator IEShowTutorial()
        {
            var cam = CameraFollow.Instance;

            yield return new WaitUntil(() => cam.target == finishedTarget);

            List<TutorialStep> steps = new List<TutorialStep>
            {

                new TutorialStep()
                {
                    TargetRenderer = vanTichThanh,
                    Text = "Đây là Vân Tịch Thành, một trong những thành trì quan trọng trên bản đồ."
                },

                new TutorialStep()
                {
                    TargetRenderer = vanTichThanh,
                    Text = "Mỗi thành đều có những đặc sản và vật phẩm riêng."
                },

                new TutorialStep()
                {
                    TargetRenderer = vanTichThanh,
                    Text =
                        "Vân Tịch Thành nổi tiếng với dược liệu và Đan Lô, rất thích hợp cho các đạo hữu muốn bắt đầu con đường luyện đan."
                },

                new TutorialStep()
                {
                    TargetRenderer = thienTinhThanh,
                    Text = "Đây là Thiên Tinh Thành, nơi hội tụ công pháp và thần thông trong thiên hạ."
                },

                new TutorialStep()
                {
                    TargetRenderer = thienTinhThanh,
                    Text =
                        "Đạo hữu có thể đến đây để tìm mua công pháp và thần thông phù hợp với con đường tu luyện của mình."
                },

                new TutorialStep()
                {
                    TargetRenderer = datPhongThanh,
                    Text = "Đây là Dật Phong Thành, nơi mua bán các loại vật liệu và trang bị."
                },

                new TutorialStep()
                {
                    TargetRenderer = datPhongThanh,
                    Text = "Nếu cần chuẩn bị vật liệu hoặc tìm kiếm trang bị, đừng quên ghé qua nơi này."
                }
            };


            var firstMainQuest = mainQuestManager.currentMapMainQuests.FirstOrDefault();

            if (firstMainQuest != null)
            {
                var render = GetRendererInMap(firstMainQuest);

                steps.Add(new TutorialStep()
                {
                    TargetRenderer = render,
                    Text = "Những quyển trục màu vàng trên bản đồ chính là nhiệm vụ chính tuyến."
                });

                steps.Add(new TutorialStep()
                {
                    TargetRenderer = render,
                    Text = "Chính tuyến được chia thành nhiều phần, mỗi phần đều có những phần thưởng riêng."
                });

                steps.Add(new TutorialStep()
                {
                    TargetRenderer = render,
                    Text = "Hãy theo dõi nhiệm vụ để không bỏ lỡ những hành trình quan trọng phía trước."
                });
            }

            var firstEnemy = enemyManager.currentMapRandom.FirstOrDefault();

            if (firstEnemy != null)
            {
                var render = GetRendererInMap(firstEnemy);

                steps.Add(new TutorialStep()
                {
                    TargetRenderer = render,
                    Text = "Những dấu hiệu màu tím trên bản đồ là nơi quái vật xuất hiện."
                });

                steps.Add(new TutorialStep()
                {
                    TargetRenderer = render,
                    Text = "Quái vật sẽ liên tục làm mới. Hãy đến đây để tìm kiếm và khiêu chiến chúng."
                });
            }

            var firstSideQuest = sideQuestManager.currentMapRandom.FirstOrDefault();

            if (firstSideQuest != null)
            {
                var render = GetRendererInMap(firstSideQuest);

                steps.Add(new TutorialStep()
                {
                    TargetRenderer = render,
                    Text = "Đây là Kỳ Ngộ, những cơ duyên bất ngờ có thể gặp trên hành trình."
                });

                steps.Add(new TutorialStep()
                {
                    TargetRenderer = render,
                    Text = "Mỗi ngày, đạo hữu có thể hoàn thành tối đa 10 Kỳ Ngộ."
                });

                steps.Add(new TutorialStep()
                {
                    TargetRenderer = render,
                    Text = "Nếu bỏ lỡ, Kỳ Ngộ sẽ tự động làm mới sau mỗi tháng."
                });

                steps.Add(new TutorialStep()
                {
                    TargetRenderer = render,
                    Text = "Hoàn thành Kỳ Ngộ có thể nhận được nhiều Tiên Ngọc."
                });
            }

            if (steps.Count > 0)
                tutorial.ShowTutorial(steps.ToArray());
        }

        private Renderer GetRendererInMap(Map map)
        {
            return map.transform.Find("Level1Move").GetChild(0).GetComponent<SpriteRenderer>();
        }
    }
}