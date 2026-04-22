using LongDH.LongDHModules;
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
    public class Stronghold : Map
    {
        [SerializeField] private GameObject enter;
        [SerializeField] private Button btnEnter;
        [SerializeField] private EMapType eCurrentMapType;
        [SerializeField] public string nameSceneLoad => GameManager.dictMapTypeWithSceneName[eCurrentMapType];

        private void Awake()
        {
            SetEventButton(btnEnter, () =>
            {
                Observer.Notify(EventList.GamePlay.LOAD_SCENE, nameSceneLoad , eCurrentMapType);
            });
        }
        public override void OnPlayerEnter()
        {
            enter.SetActive(true);
        }
        public override void OnPlayerExit()
        {
            enter.SetActive(false);
        }
    }
}
