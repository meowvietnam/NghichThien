using AllMaps.EventReward;
using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using System.Collections.Generic;
using System.Linq;
using Base;
using UnityEngine;
using UnityEngine.UI;

public class GachaPanel : PanelGroup, INetworkEvent.IGacha
{
    [SerializeField] private List<Button> listBtnPage;
    [SerializeField] private List<GameObject> listPageVisual;
    [SerializeField] private Button currentBtn;

    public static Dictionary<ETypeGacha, int> PageGachaWithRequireValue = new Dictionary<ETypeGacha, int>
    {
        { ETypeGacha.ThanThong, 400 },
        { ETypeGacha.CongPhap, 500 },
        { ETypeGacha.DanDuoc, 500 },
        { ETypeGacha.DuocLieu, 300 },
        { ETypeGacha.VatLieu, 200 },
    };

    private BaseSkill.ETypeSkill typeSkill;
    [SerializeField] private Button btnGacha1;
    [SerializeField] private Button btnGacha10;
    [SerializeField] public Text txtRequire1 => btnGacha1.transform.GetChild(0).GetComponent<Text>();
    [SerializeField] public Text txtRequire10 => btnGacha10.transform.GetChild(0).GetComponent<Text>();
    [SerializeField] private Button btnBack;
    [SerializeField] private Text thresholdText;

    public ETypeGacha CurrentTypeGacha =>
        currentBtn != null ? (ETypeGacha)listBtnPage.IndexOf(currentBtn) : ETypeGacha.ThanThong;

    public override void Init()
    {
        base.Init();
        for (int i = 0; i < listBtnPage.Count; i++)
        {
            int index = i;
            var btn = listBtnPage[index];
            SetEventButton(btn, () => { InitBtn(btn); });
        }

        SetEventButton(btnGacha1, () =>
        {
            var moneyVip = Client.Instance.CharacterData.MoneyVip;
            var require = GetPrice(CurrentTypeGacha);

            if (moneyVip < require)
            {
                NotifyGroup.Instance.Show("Bạn không đủ tiên ngọc");
            }
            else
            {
                Pool.Get<PopupConfirmAction>().ShowPopConfirmAction(
                    $"Xác nhận tiêu {Utils.FormatCount(require)} tiên ngọc quay 1 lần.  Bạn còn lại {Utils.FormatCount(moneyVip)} Tiên ngọc",
                    () =>
                    {
                        Observer.Notify(EventList.Network.GACHA, this, CurrentTypeGacha, false, typeSkill); //isGacha10
                    });
            }
        });
        SetEventButton(btnGacha10, () =>
        {
            var moneyVip = Client.Instance.CharacterData.MoneyVip;
            int require = GetPrice(CurrentTypeGacha) * 10;
            if (moneyVip < require)
            {
                NotifyGroup.Instance.Show("Bạn không đủ tiên ngọc");
            }
            else
            {
                Pool.Get<PopupConfirmAction>().ShowPopConfirmAction(
                    $"Xác nhận tiêu {Utils.FormatCount(require)} tiên ngọc quay 10 lần. Bạn còn lại {Utils.FormatCount(moneyVip)} Tiên ngọc",
                    () =>
                    {
                        Observer.Notify(EventList.Network.GACHA, this, CurrentTypeGacha, true, typeSkill); //isGacha10
                    });
            }
        });
        SetEventButton(btnBack, () => { gameObject.SetActive(false); });
        UpdateThreshold();
    }

    private int GetPrice(ETypeGacha gacha)
    {
        var alpha = 1;
        var typeSpecials = new[] { ETypeGacha.ThanThong, ETypeGacha.CongPhap };
        if (typeSpecials.Contains(CurrentTypeGacha))
        {
            if (typeSkill != BaseSkill.ETypeSkill.None)
            {
                alpha = 2;
            }
        }

        int require = PageGachaWithRequireValue[gacha] * alpha;
        return require;
    }

    private void InitBtn(Button btnRef)
    {
        if (currentBtn != null)
        {
            int indexCurrent = listBtnPage.IndexOf(currentBtn);
            listPageVisual[indexCurrent].SetActive(false);
            currentBtn.transform.GetChild(0).gameObject.SetActive(false);
        }

        this.currentBtn = btnRef;
        UpdatePrice();
        btnRef.transform.GetChild(0).gameObject.SetActive(true);
        UpdateThreshold();
    }

    private void UpdatePrice()
    {
        int indexRef = listBtnPage.IndexOf(currentBtn);
        listPageVisual[indexRef].SetActive(true);
        txtRequire1.text = $"x{GetPrice((ETypeGacha)indexRef)}";
        txtRequire10.text = $"x{GetPrice((ETypeGacha)indexRef) * 10}";
    }

    public void SetActive()
    {
        gameObject.SetActive(true);
        InitBtn(listBtnPage[0]);
    }

    public void SuccessGacha(RewardSnapshot rewardGacha)
    {
        var panel = Pool.Get<EventRewardPanel>();
        panel.InitShowEventReward(rewardGacha, "May mắn luôn ở quanh ta!");
        UpdateThreshold();
    }

    private void UpdateThreshold()
    {
        var gacha = Client.Instance.CharacterData.GachaManager;
        var total = gacha.Get(CurrentTypeGacha, typeSkill);
        var threshold = gacha.Threshold;
        var remaining = threshold - total % threshold;
        var text = $"Còn {remaining} lần quay nữa bạn sẽ nhận được vật phẩm hiếm";
        if (thresholdText != null)
        {
            thresholdText.text = text;
        }
    }

    public void SetTypeSkill(BaseSkill.ETypeSkill skill)
    {
        typeSkill = skill;
        UpdateThreshold();
        UpdatePrice();
    }

    public void FailGacha(string message)
    {
        NotifyGroup.Instance.Show(message);
    }
}