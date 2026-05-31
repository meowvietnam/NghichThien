using LongDH.LongDHModules;
using Network;
using System;
using System.Collections.Generic;
using LongDHModules;
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
        

    }
    public class Stronghold : Map , INetworkEvent.IJoinMonPhai
    {
        [SerializeField] private GameObject enter;
        [SerializeField] private Button btnEnter;
        [SerializeField] public EMapType eCurrentMapType;
        [SerializeField] public string nameSceneLoad => GameManager.dictMapTypeWithSceneName[eCurrentMapType];

        private void Awake()
        {
           
        }
        public void SetEventBtnEnterStrongHold(Action eventBase)
        {
            SetEventButton(btnEnter, () =>
            {
                var gameManager = Pool.Get<GameManager>();
                if (gameManager.CurrentEMonPhai == Client.Instance.CharacterData.MonPhai || gameManager.CurrentEMonPhai == Base.EMonPhai.None)
                {
                    LoadToMapStrongHold();
                }
                else if (gameManager.CurrentEMonPhai != Base.EMonPhai.None && Client.Instance.CharacterData.MonPhai == Base.EMonPhai.None && Pool.Get<MainQuestManager>().currentMainQuest > 0)
                {
                    GameManager.SetDialogGacCong($"phải chăng ngươi muốn gia nhập {GameManager.monPhaiText.GetValueOrDefault(gameManager.CurrentEMonPhai)}?", () =>
                    {
                        Pool.Get<PopupConfirmAction>().ShowPopConfirmAction($"Xác nhận gia nhập {GameManager.monPhaiText.GetValueOrDefault(gameManager.CurrentEMonPhai)}", () =>
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
                Observer.Notify(EventList.GamePlay.LOAD_SCENE, nameSceneLoad, eCurrentMapType,this);
                eventBase?.Invoke();
            }
        }    
        public override void OnPlayerEnter()
        {
            SetEventBtnEnterStrongHold(() =>
            {
                base.OnPlayerEnter();
            });
            enter.SetActive(true);
        }
        public override void OnPlayerExit()
        {
            enter.SetActive(false);
        }

        public void SuccessJoinMonPhai()
        {

        }

        public void FailJoinMonPhai(string message)
        {
        }
    }
}
