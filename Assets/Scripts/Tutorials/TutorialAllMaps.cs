using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AllMaps;
using LongDH.LongDHModules;
using LongDHModules;
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

        [Header("Hand tut: chỉ vào Thiên Tinh Thành (Id = 23) rồi tới nút Tiến vào")]
        [Tooltip("Component Stronghold của node Thiên Tinh Thành trên bản đồ (Map.Id = 23). " +
                 "Kéo GameObject node Thiên Tinh Thành (cùng object chứa thienTinhThanh ở trên) vào đây.")]
        [SerializeField] private Stronghold thienTinhThanhStronghold;

        public const string tutorialPointThienTinhKey = "isFirstTutorialPointThienTinhThanh";

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

            if (thienTinhThanhStronghold != null)
                thienTinhThanhStronghold.OnEnteredRange -= OnArrivedThienTinhThanh;
        }

        private void CheckTutorial()
        {
            var isFirstTutorial = PlayerPrefs.GetInt(tutorialAllMapKey, 0) == 0 &&
                                  Client.Instance.CharacterData.QuestManager.CurrentMainQuest != 0;
            if (!isFirstTutorial) return;

            var currentMap = Pool.Get<MapManager>()?.CurrentMap;
            if (currentMap?.actionMap is BaseEnemyCombatResource ||
                currentMap?.actionMap is QuestResource)
            {
                return;
            }

            ShowTutorial();
        }

        private void OnCompleted()
        {
            // "tutorial" (component dùng chung) bắn OnCompleted sau MỖI lần ShowTutorial() kết
            // thúc - không chỉ riêng phần giới thiệu bản đồ dài ở IEShowTutorial(), mà cả 2 bước
            // hand-tut phía dưới (chỉ vào thành, chỉ vào nút Tiến vào) cũng dùng chung component
            // này nên cũng sẽ làm OnCompleted bắn thêm 2 lần nữa. Nếu không chặn lại, mỗi lần người
            // chơi hoàn thành 1 bước hand-tut thì y hệt đoạn code "kết thúc intro" này sẽ chạy lại:
            // camera bị kéo về finishedTarget, camera2DDrag bị bật lại giữa chừng, và
            // TryShowPointThienTinhThanh() bị gọi lại làm hiện lại tutorial chỉ-vào-thành - đúng
            // hiện tượng "tut tắt, cam lia 1 lúc, tut hiện lại". Nên chỉ cho phần này chạy đúng 1
            // lần duy nhất (lúc intro thật sự vừa xong).
            if (PlayerPrefs.GetInt(tutorialAllMapKey, 0) != 0) return;

            var cam = CameraFollow.Instance;
            cam.target = finishedTarget;
            PlayerPrefs.SetInt(tutorialAllMapKey, 1);
            camera2DDrag.enabled = true;

            TryShowPointThienTinhThanh();
        }

        // ============================================================
        // Hand tut: sau khi xong tutorial giới thiệu bản đồ, chỉ người chơi
        // đi vào Thiên Tinh Thành (Id = 23) rồi bấm "Tiến vào".
        // ============================================================

        private void TryShowPointThienTinhThanh()
        {
            if (PlayerPrefs.GetInt(tutorialPointThienTinhKey, 0) != 0) return;
            if (thienTinhThanhStronghold == null || thienTinhThanh == null) return;

            camera2DDrag.enabled = false;

            var steps = new[]
            {
                new TutorialStep
                {
                    TargetRenderer = thienTinhThanh,
                    Text = "Hãy chạm vào Thiên Tinh Thành để tiến đến nơi này.",
                    RequireRealAction = true
                }
            };

            thienTinhThanhStronghold.OnMapClicked += OnClickThienTinhThanh;
            tutorial.ShowTutorial(steps);
        }

        private void OnClickThienTinhThanh()
        {
            thienTinhThanhStronghold.OnMapClicked -= OnClickThienTinhThanh;

            // BẮT BUỘC phải trả camera về nhân vật NGAY TẠI ĐÂY, trước khi thoát hàm này.
            // Lý do: MapManager.PlayerMoveTo() mở đầu bằng
            //     if (CameraFollow.Instance.target != player.transform) return;
            // (guard để khoá di chuyển trong lúc tutorial đang lia camera đi giới thiệu bản đồ).
            // Bước hand-tut này dùng TargetRenderer nên Tutorial.OnSetTarget đã đẩy
            // CameraFollow.target sang bám vào chính toà thành => cú chạm vào thành sẽ bị guard
            // trên chặn, nhân vật đứng yên, không bao giờ tới nơi, và tut kẹt mãi ở bước chờ.
            // Map.OnPointerClick() bắn OnMapClicked?.Invoke() TRƯỚC khi gọi PlayerMoveTo(), nên chỉ
            // cần trả camera về nhân vật ở đây là CHÍNH cú chạm vừa rồi sẽ di chuyển được ngay.
            // (finishedTarget chính là transform của Player - cùng GameObject "Visual" trong
            // Player.prefab với component AllMaps.Player mà MapManager đang giữ.)
            var cam = CameraFollow.Instance;
            if (cam != null && finishedTarget != null)
            {
                cam.target = finishedTarget;
                cam.IsFollow = true;
            }

            // Trả lại quyền kéo camera bình thường trong lúc nhân vật đi tới thành.
            if (camera2DDrag != null) camera2DDrag.enabled = true;

            tutorial.CompleteInteractiveStep();

            // QUAN TRỌNG: KHÔNG dùng EventList.GamePlay.MOVE_MAP_SUCCESS để biết "đã tới nơi" -
            // sự kiện đó bắn ra ngay khi server chấp nhận lệnh di chuyển, TRƯỚC KHI nhân vật chạy
            // tới (xem MapManager.Move()/OnMoveToMapResult()). Panel "Tiến vào" (btnEnter) chỉ thật
            // sự xuất hiện sau khi animation đi bộ chạy xong, qua Stronghold.OnPlayerEnter() - nên
            // phải chờ đúng sự kiện OnEnteredRange (bắn ra trong OnPlayerEnter()) mới đúng.
            if (thienTinhThanhStronghold.IsEnterPanelActive)
            {
                // Trường hợp hiếm: nhân vật đã đứng sẵn ở đó từ trước (panel đã hiện rồi).
                ShowPointBtnTienVao();
            }
            else
            {
                thienTinhThanhStronghold.OnEnteredRange += OnArrivedThienTinhThanh;
            }
        }

        private void OnArrivedThienTinhThanh()
        {
            thienTinhThanhStronghold.OnEnteredRange -= OnArrivedThienTinhThanh;
            ShowPointBtnTienVao();
        }

        private void ShowPointBtnTienVao()
        {
            var btnEnter = thienTinhThanhStronghold.BtnEnter;
            if (btnEnter == null)
            {
                PlayerPrefs.SetInt(tutorialPointThienTinhKey, 1);
                camera2DDrag.enabled = true;
                return;
            }

            var steps = new[]
            {
                new TutorialStep
                {
                    TargetRect = btnEnter.GetComponent<RectTransform>(),
                    Text = "Chạm vào nút \"Tiến vào\" để bước vào Thiên Tinh Thành.",
                    RequireRealAction = true
                }
            };

            btnEnter.onClick.AddListener(OnClickTienVao);
            tutorial.ShowTutorial(steps);
        }

        private void OnClickTienVao()
        {
            thienTinhThanhStronghold.BtnEnter.onClick.RemoveListener(OnClickTienVao);
            tutorial.CompleteInteractiveStep();
            PlayerPrefs.SetInt(tutorialPointThienTinhKey, 1);
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
                    Text = "Đây là Dật Phong Thành, nơi mua bán các loại vật liệu và trang bị.",
                    DialogPosition = DialogPosition.TopLeft
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
