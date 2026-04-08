using Base;
using LongDH.LongDHModules;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBeQuan : BaseBehaviour , INetworkEvent.ITuLuyen
{
    public enum ETypePage
    {

        None,
        TuLuyen,
        LinhNgo,
        Update,



    }
    [System.Serializable]
    public class ButtonWithPage
    {
        [SerializeField] public Button btn;
        [SerializeField] public GameObject page;
        [SerializeField] public ETypePage typePage;


    }
    [SerializeField] private ButtonWithPage[] btns;
    [SerializeField] private ButtonWithPage currentBtnWithPage;
    [SerializeField] private Button btnShowPopBeQuan;
    [SerializeField] private Popup popBeQuan;
    private void Awake()
    {
        
    }
    public void Init()
    {
        SetEventButton(btnShowPopBeQuan, () =>
        {
            popBeQuan.Show();
            btns[0].btn.onClick?.Invoke();
        });
        foreach (var item in btns)
        {
            SetEventButton(item.btn, () =>
            {
                if (currentBtnWithPage.typePage == item.typePage)
                {
                    return;
                }
                currentBtnWithPage = item;
                foreach (var item2 in btns)
                {
                    item2.page.gameObject.SetActive(false);
                }
                currentBtnWithPage.page.gameObject.SetActive(true);
               
                if (item.typePage == ETypePage.TuLuyen)
                {
                    InitPageTuLuyen();
                }
                if (item.typePage == ETypePage.LinhNgo)
                {
                    InitPageLinhNgo();
                }
                if (item.typePage == ETypePage.Update)
                {
                    InitPageUpdate();
                }

            });
        }
    }
    [Header("Tu Luyện")]
    [SerializeField] private Text nameCP;
    [SerializeField] private Text valueTuLuyenForCPText;
    [SerializeField] private Text percentTuChatText;
    [SerializeField] private Text percentTamCanhText;
    [SerializeField] private Text tamCanhValueText;
    [SerializeField] private Text tamCanhText;
    [SerializeField] private Text tuChatText;
    [SerializeField] private Text totalValueTuLuyenText;
    [SerializeField] private Slider sliderTuLuyen;
    [SerializeField] private Button btnAddSlider;
    [SerializeField] private Button btnRemoveSlider;
    [SerializeField] private Button btnTuLuyen;
    [SerializeField] private Text textNam;
    [SerializeField] private Text textThang;
    [SerializeField]
    private TimeSpan currentTuLuyenTimeSpan => GameConfig.ConvertMonthToTimeSpan(currentTuLuyenTimeInt);
    private SecureInt currentTuLuyenTimeInt; // month
    [SerializeField] private Text textPriceTieuHao;
    [Header("Cấu hình chi phí")]
    public double baseCost = 100f;    // Chi phí tại Lv1
    public float r = 0.2f;           // Tỉ lệ tăng mỗi cấp (10%)
    public float m = 5f;           // Hệ số nhảy vọt mỗi 3 cấp
    public double CostPriceTieuHao1Month => CalculateCost(Client.Instance.CharacterData.Level);
    private void InitPageTuLuyen()
    {
        var cpChuTu = Pool.Get<GameConfig>().GetCongPhapWithId(Client.Instance.CharacterData.ChuTu1Used);
        nameCP.text = $"《{cpChuTu.nameItem}》";
        int tocDoTuLuyenCP = cpChuTu.GetTocDoTuLuyen(Client.Instance.CharacterData.CongPhapOwner.GetValueOrDefault(Client.Instance.CharacterData.ChuTu1Used));
        valueTuLuyenForCPText.text = tocDoTuLuyenCP.ToString();
        int percentTuChat = Client.Instance.CharacterData.Stats.TuChat;
        percentTuChatText.text = $"{percentTuChat}%";
        var getTamCanh = GameConfig.GetTamCanhValue(Client.Instance.CharacterData.Stats.TamCanh);

        int percentTamCanh =  0;
        tamCanhValueText.text = "Tăng lên";
        if (getTamCanh.index > ThanThong.GetDaiCanhGioi(Client.Instance.CharacterData.Level))
        {
            percentTamCanh = +50;
            tamCanhValueText.text = "Tăng lên";
        }
        else if (getTamCanh.index < ThanThong.GetDaiCanhGioi(Client.Instance.CharacterData.Level))
        {
            percentTamCanh = -50;
            tamCanhValueText.text = "Giảm xuống";

        }
        percentTamCanhText.text = $"{percentTamCanh}%";
        int GetTotalValueBuffWithMultiplier(List<float>percent)
        {
            float total = tocDoTuLuyenCP;
            foreach (var item in percent)
            {
                total *= (1 + item / 100f);
            }    
            return (int)total;
                
        }
        tuChatText.text = GameConfig.tuChatWithText[Client.Instance.CharacterData.Stats.LinhCan];
        tamCanhText.text = getTamCanh.name;
        totalValueTuLuyenText.text = $"{GetTotalValueBuffWithMultiplier(new List<float> { percentTuChat,100,percentTamCanh})}";
        sliderTuLuyen.minValue = 0 ;
        sliderTuLuyen.maxValue = (int)(Client.Instance.CharacterData.Stats.ThoNguyen - GameConfig.GetIntTuoiTac(Client.Instance.CharacterData.Stats.TuoiTac)) * 10 / 100;
        sliderTuLuyen.onValueChanged.RemoveAllListeners();
        sliderTuLuyen.onValueChanged.AddListener((value) =>
        {
            // 1. Ép kiểu value về int (vì thời gian tu luyện của bạn là SecureInt/int)
            int tongSoThang = Mathf.RoundToInt(value);
            // 2. Cập nhật vào biến SecureInt
            currentTuLuyenTimeInt = (tongSoThang);
            UpdateTuLuyenTimeUI();
            textPriceTieuHao.text = $"{CostPriceTieuHao1Month * tongSoThang}";
        });
        // Nút cộng: Thay đổi giá trị của Slider
        SetEventButton(btnAddSlider, () =>
        {
            // Kiểm tra để không vượt quá maxValue
            if (sliderTuLuyen.value < sliderTuLuyen.maxValue)
            {
                sliderTuLuyen.value += 1;
                // Khi gán .value, onValueChanged sẽ tự chạy
            }
        });

        // Nút trừ: Thay đổi giá trị của Slider
        SetEventButton(btnRemoveSlider, () =>
        {
            // Kiểm tra để không nhỏ hơn minValue
            if (sliderTuLuyen.value > sliderTuLuyen.minValue)
            {
                sliderTuLuyen.value -= 1;
                // Khi gán .value, onValueChanged sẽ tự chạy
            }
        });
        SetEventButton(btnTuLuyen, () =>
        {
            if(Client.Instance.CharacterData.Stats.LinhThach >= CostPriceTieuHao1Month * currentTuLuyenTimeInt)
            {
                Observer.Notify(EventList.Network.TU_LUYEN,this, currentTuLuyenTimeSpan);
            }    
        });

    }
    public double CalculateCost(int level)
    {
        if (level < 1) return 0;

        // Công thức: [Base * (1 + r)^(L-1)] * (M ^ floor((L-1)/3))
        double steadyGrowth = Math.Pow(1 + r, level - 1);
        double jumpFactor = Math.Pow(m, Math.Floor((level - 1) / 3.0));

        double finalCost = (baseCost * steadyGrowth) * jumpFactor;

        return Math.Round(finalCost);
    }
    void UpdateTuLuyenTimeUI()
    {
        // Lấy giá trị int từ SecureInt
        int tongSoThang = currentTuLuyenTimeInt;

        // Tính toán số năm và số tháng dư
        int nam = tongSoThang / 12;
        int thang = tongSoThang % 12;

        // Gán giá trị lên UI
        textNam.text = nam.ToString();
        textThang.text = thang.ToString();
    }
    private void InitPageLinhNgo()
    {

    }
    private void InitPageUpdate()
    {

    }

    public void SuccessTuLuyen()
    {
        throw new NotImplementedException();
    }

    public void FailTuLuyen(string message)
    {
        throw new NotImplementedException();
    }
}

