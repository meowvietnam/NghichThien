using CreateCharacter;
using DG.Tweening.Core.Easing;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    public enum EMapType
    {
        None,
        KimHongKiemPhai,
        TrucSonTong,
        TinhHaKiemPhai,
        LyHoaMon,
        HoaTranGiao,
        BachDeLau,
        VanTichThanh,
        ThienTinhThanh,
        DatPhongThanh,
        QuangLangThanh,
        ThienCoCac,
        DongPhu
    }

    public class Stronghold : Map, INetworkEvent.IJoinMonPhai
    {
        [SerializeField] private GameObject enter;
        [SerializeField] private Button btnEnter;
        [SerializeField] public EMapType eCurrentMapType;
        [SerializeField] public string nameSceneLoad => GameManager.dictMapTypeWithSceneName[eCurrentMapType];

        /// <summary>Nút "Tiến vào" hiện lên khi người chơi đã đi tới thành trì này (xem <see cref="enter"/>).
        /// Public để hand-tut (Tutorials) có thể gắn thêm listener mà không đụng vào logic gốc.</summary>
        public Button BtnEnter => btnEnter;

        /// <summary>True khi panel "Tiến vào" đang hiện (đã tới nơi), dùng để hand-tut biết lúc nào
        /// nên khoanh sáng nút Tiến vào.</summary>
        public bool IsEnterPanelActive => enter != null && enter.activeSelf;

        /// <summary>
        /// Bắn ra khi người chơi THẬT SỰ đã đi tới thành trì này (panel "Tiến vào" vừa được bật lên
        /// trong OnPlayerEnter()). LƯU Ý: KHÔNG dùng EventList.GamePlay.MOVE_MAP_SUCCESS cho việc
        /// này - sự kiện đó bắn ra ngay khi server chấp nhận lệnh di chuyển (trước cả khi nhân vật
        /// bắt đầu chạy tới, xem MapManager.Move()/OnMoveToMapResult()), còn OnPlayerEnter() chỉ
        /// được gọi sau khi animation di chuyển hoàn tất (MapManager.OnMoveToMapCompleted()) - tức
        /// đúng lúc nút "Tiến vào" mới thật sự xuất hiện.
        /// </summary>
        public event Action OnEnteredRange;

        private void Awake()
        {
        }

        public void SetEventBtnEnterStrongHold(Action eventBase)
        {
            SetEventButton(btnEnter, () =>
            {
                var gameManager = Pool.Get<GameManager>();
                if (gameManager.CurrentEMonPhai == Client.Instance.CharacterData.MonPhai ||
                    gameManager.CurrentEMonPhai == Base.EMonPhai.None)
                {
                    LoadToMapStrongHold();
                }
                else if (gameManager.CurrentEMonPhai != Base.EMonPhai.None &&
                         Client.Instance.CharacterData.MonPhai == Base.EMonPhai.None &&
                         Pool.Get<MainQuestManager>().currentMainQuest > 0)
                {
                    GameManager.SetDialogGacCong(
                        $"phải chăng ngươi muốn gia nhập {GameManager.monPhaiText.GetValueOrDefault(gameManager.CurrentEMonPhai)}?",
                        () =>
                        {
                            Pool.Get<PopupConfirmAction>().ShowPopConfirmAction(
                                $"Xác nhận gia nhập {GameManager.monPhaiText.GetValueOrDefault(gameManager.CurrentEMonPhai)}",
                                () =>
                                {
                                    Observer.Notify(EventList.Network.JOIN_MONPHAI, this, gameManager.CurrentEMonPhai);
                                    LoadToMapStrongHold();
                                });
                        });
                }
                else
                {
                    GameManager.SetDialogGacCong("xin dừng bước nơi này người ngoài không thể tiến vào");
                }
            });

            void LoadToMapStrongHold()
            {
                Observer.Notify(EventList.GamePlay.LOAD_SCENE, nameSceneLoad, eCurrentMapType, this);
                eventBase?.Invoke();
            }
        }

        public override void OnPlayerEnter()
        {
            SetEventBtnEnterStrongHold(() => { base.OnPlayerEnter(); });
            enter.SetActive(true);
            OnEnteredRange?.Invoke();
        }

        public override void OnPlayerExit()
        {
            enter.SetActive(false);
        }

        public void SuccessJoinMonPhai()
        {
            NotifyGroup.Instance.Show(
                $"Thành công gia nhập {GameManager.monPhaiText.GetValueOrDefault(Pool.Get<GameManager>().CurrentEMonPhai)}");
        }

        public void FailJoinMonPhai(string message)
        {
            Debug.Log("FailJoinMonPhai + " + message);
        }
    }
}