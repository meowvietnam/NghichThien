using AllMaps.EventReward;
using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaPanel : PanelGroup , INetworkEvent.IGacha
{

    [SerializeField] private List<Button> listBtnPage;
    [SerializeField] private List<Image> listPageVisual;
    [SerializeField] private Button currentBtn; 
    public static Dictionary<ETypeGacha, int> PageGachaWithRequireValue = new Dictionary<ETypeGacha, int>
    {
        {ETypeGacha.ThanThong,400},
        {ETypeGacha.CongPhap,500},
        {ETypeGacha.DanDuoc,500},
        {ETypeGacha.DuocLieu,300},
        {ETypeGacha.VatLieu,200},
    };
    [SerializeField] private Button btnGacha1;
    [SerializeField] private Button btnGacha10;
    [SerializeField] public Text txtRequire1 => btnGacha1.transform.GetChild(0).GetComponent<Text>();
    [SerializeField] public Text txtRequire10 => btnGacha10.transform.GetChild(0).GetComponent<Text>();
    [SerializeField] private Button btnBack;
    public ETypeGacha CurrentTypeGacha => currentBtn != null?(ETypeGacha)listBtnPage.IndexOf(currentBtn) : ETypeGacha.ThanThong;
    public override void Init()
    {
        base.Init();
        for (int i = 0; i < listBtnPage.Count; i++)
        {
            int index = i;
            var btn = listBtnPage[index];
            SetEventButton(btn, () =>
            {

                InitBtn(btn);
                
            });

        }
        SetEventButton(btnGacha1, () =>
        {
            int require = PageGachaWithRequireValue[CurrentTypeGacha];
            if(Client.Instance.CharacterData.MoneyVip < require)
            {
                NotifyGroup.Instance.Show("Bạn không đủ tiên ngọc");
            }    
            else
            {
                Pool.Get<PopupConfirmAction>().ShowPopConfirmAction($"Xác nhận tiêu {require} tiên ngọc quay 1 lần", () =>
                {
                    NotifyGroup.Instance.Show("Quay 1 lần thành công");
                    Observer.Notify(EventList.Network.GACHA, this, CurrentTypeGacha, false);//isGacha10
                });
               

            }
        });
        SetEventButton(btnGacha10, () =>
        {
            int require = PageGachaWithRequireValue[CurrentTypeGacha] * 10;
            if (Client.Instance.CharacterData.MoneyVip < require)
            {
                NotifyGroup.Instance.Show("Bạn không đủ tiên ngọc");
            }
            else
            {
                Pool.Get<PopupConfirmAction>().ShowPopConfirmAction($"Xác nhận tiêu {require} tiên ngọc quay 10 lần", () =>
                {
                    NotifyGroup.Instance.Show("Quay 10 lần thành công");
                    Observer.Notify(EventList.Network.GACHA, this, CurrentTypeGacha, true);//isGacha10
                });
            }    
        });
        SetEventButton(btnBack, () =>
        {
           gameObject.SetActive(false);
        });
    }
    private void InitBtn(Button btnRef)
    {
        if(currentBtn != null)
        {
            int indexCurrent = listBtnPage.IndexOf(currentBtn);
            listPageVisual[indexCurrent].gameObject.SetActive(false);
            currentBtn.transform.GetChild(0).gameObject.SetActive(false);
        }    
        this.currentBtn = btnRef;
        int indexRef = listBtnPage.IndexOf(btnRef);
        listPageVisual[indexRef].gameObject.SetActive(true);
        txtRequire1.text = $"x{PageGachaWithRequireValue[(ETypeGacha)indexRef]}";
        txtRequire10.text = $"x{PageGachaWithRequireValue[(ETypeGacha)indexRef] * 10}";
        btnRef.transform.GetChild(0).gameObject.SetActive(true);

    }
    public void SetActive()
    {
        this.gameObject.SetActive(true);
        InitBtn(listBtnPage[0]);
    }
    public void SuccessGacha(RewardSnapshot rewardGacha)
    {
        Pool.Get<EventRewardPanel>().InitShowEventReward(rewardGacha,"May mắn luôn ở quanh ta!");
    }   
    public void FailGacha(string message)
    {
        NotifyGroup.Instance.Show(message);
    }    

}
