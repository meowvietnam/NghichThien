using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIBeQuan : BaseBehaviour , INetworkEvent.ITuLuyen, INetworkEvent.ILinhNgo
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
        Init();
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
    [SerializeField] private Button btnUnTuLuyen;

    [Header("Cấu hình chi phí")]
    public double baseCost = 100f;    // Chi phí tại Lv1
    public float r = 0.2f;           // Tỉ lệ tăng mỗi cấp (10%)
    public float m = 5f;           // Hệ số nhảy vọt mỗi 3 cấp
    public double CostPriceTieuHao1Month => CalculateCost(Client.Instance.CharacterData.Level);
    private void InitPageTuLuyen()
    {
        
        var cpChuTu = Pool.Get<GameConfig>().GetCongPhapWithId(Client.Instance.CharacterData.ChuTu1Used);
        string nameCPstr = "Không";
      
        int tocDoTuLuyenCP = 0;
        if (cpChuTu != null)
        {
            nameCPstr = cpChuTu.nameItem;
            tocDoTuLuyenCP = cpChuTu.GetTocDoTuLuyen(Client.Instance.CharacterData.CongPhapOwner.GetValueOrDefault(Client.Instance.CharacterData.ChuTu1Used));
        }
        nameCP.text = $"《{nameCPstr}》";
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
        sliderTuLuyen.maxValue = (int)(Client.Instance.CharacterData.Stats.ThoNguyen - GameConfig.GetIntTuoiTac(Client.Instance.CharacterData.Stats.TuoiTac)) * 10 * 12 / 100;
        sliderTuLuyen.onValueChanged.RemoveAllListeners();
        sliderTuLuyen.onValueChanged.AddListener((value) =>
        {
            // 1. Ép kiểu value về int (vì thời gian tu luyện của bạn là SecureInt/int)
            int tongSoThang = Mathf.RoundToInt(value);
            // 2. Cập nhật vào biến SecureInt
            currentTuLuyenTimeInt = (tongSoThang);
            UpdateTuLuyenTimeUI();
            textPriceTieuHao.text = $"{CostPriceTieuHao1Month * tongSoThang}";
            SetUpBtnTuLuyen();
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
            if (cpChuTu == null)
            {
                NotifyGroup.Instance.Show("Trang bị công pháp chủ tu mới có thể bế quan");
                return;
            }
            if (Client.Instance.CharacterData.Stats.LinhThach >= CostPriceTieuHao1Month * currentTuLuyenTimeInt)
            {
                Observer.Notify(EventList.Network.TU_LUYEN,this, currentTuLuyenTimeSpan);
            }    
        });
        SetEventButton(btnUnTuLuyen, () =>
        {
            if (cpChuTu == null)
            {
                NotifyGroup.Instance.Show("Trang bị công pháp chủ tu mới có thể bế quan");
            }
            if (Client.Instance.CharacterData.Stats.LinhThach < CostPriceTieuHao1Month * currentTuLuyenTimeInt)
            {
                NotifyGroup.Instance.Show("Không đủ linh thạch");
            }
        });
        SetUpBtnTuLuyen();
        void SetUpBtnTuLuyen()
        {
            if (cpChuTu == null || Client.Instance.CharacterData.Stats.LinhThach < CostPriceTieuHao1Month * currentTuLuyenTimeInt)
            {
                btnUnTuLuyen.gameObject.SetActive(true);
                btnTuLuyen.gameObject.SetActive(false);
            }
            else
            {
                btnUnTuLuyen.gameObject.SetActive(false);
                btnTuLuyen.gameObject.SetActive(true);

            }
        }
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
    [SerializeField] private Sprite[] spriteQualityFulls;
    [SerializeField] private Sprite[] spriteFrames;
    [SerializeField] private Color[] colorQualitys;
    private void InitVisualItemSkill(ItemInventory item, BaseSkill data, ETypeInventoryItem eTypeInventoryItem = ETypeInventoryItem.Skill)
    {
        Sprite frame = null;
        if (data != null)
        {
            frame = spriteFrames[((((int)data.eTierSkill - 1) / 3) * 2) + 1];

            item.SetColorName(colorQualitys[((((int)data.eTierSkill - 1) / 3) * 2) + 1]);
            item.SetQualityFull(spriteQualityFulls[((((int)data.eTierSkill - 1) / 3) * 2) + 1]);

        }

        item.Init(data, frame);

        item.InitInventory(eTypeInventoryItem, null, null);

      
    }
    [Header("Lĩnh Hội")]
    [SerializeField] private ItemInventory prefabItemInvenPanelLinhHoi;

    [SerializeField] private Text priceTextLinhHoi;
    [SerializeField] private Text textTimer;
    [SerializeField] private Text textInfor;
    [SerializeField] private Button btnLinhHoi;
    [SerializeField] private Button btnUnLinhHoi;
    private void InitVisualItemSkillLinhHoi(ItemInventory item, BaseSkill data, ItemRef itemRef, ETypeInventoryItem eTypeInventoryItem = ETypeInventoryItem.Skill)
    {
        InitVisualItemSkill(item,data,eTypeInventoryItem);
        item.InitAction(null, null, () =>
        {
            int cost = (int)GameConfig.GetPriceItemBuy(itemRef, null) / 2;
            priceTextLinhHoi.text = $"{cost}";
            if(itemRef.Type == ItemType.ThanThong)
            {
                ThanThong thanThong = (ThanThong)data;
                textInfor.text = thanThong.GetDesc(ThanThong.GetDaiCanhGioi(Client.Instance.CharacterData.Level));
            }    
            else if (itemRef.Type == ItemType.CongPhap)
            {
                CongPhap cp = (CongPhap)data;
                int daiCanhGioiCP = Client.Instance.CharacterData.CongPhapOwner.GetValueOrDefault(itemRef.Id);
                if(daiCanhGioiCP == 0)
                {
                    daiCanhGioiCP = 1;
                }    
                textInfor.text = cp.GetDesc(daiCanhGioiCP);


            }
            SetEventButton(btnLinhHoi, () =>
            {
                if (Client.Instance.CharacterData.Stats.LinhThach >= cost)
                {
                    Observer.Notify(EventList.Network.LINH_NGO, this, itemRef.Id , itemRef.Type);
                }
            });
            SetEventButton(btnUnLinhHoi, () =>
            {
                if (Client.Instance.CharacterData.Stats.LinhThach < cost)
                {
                    NotifyGroup.Instance.Show("Không đủ linh thạch");
                }
              
            });
            SetUpBtnLinhHoi();
            void SetUpBtnLinhHoi()
            {
                if (Client.Instance.CharacterData.Stats.LinhThach < cost)
                {
                    btnUnLinhHoi.gameObject.SetActive(true);
                    btnLinhHoi.gameObject.SetActive(false);
                }
                else
                {
                    btnUnLinhHoi.gameObject.SetActive(false);
                    btnLinhHoi.gameObject.SetActive(true);

                }
            }
        });
       
    }    
    private void InitPageLinhNgo()
    {
        
        // Lấy dữ liệu gốc
        var thanThongItems = Client.Instance.CharacterData.InventoryManager.ThanThongs;
        var congPhapItems = Client.Instance.CharacterData.InventoryManager.CongPhaps;

        var dictCPOwner = Client.Instance.CharacterData.CongPhapOwner;
        var listThanThongOwner = Client.Instance.CharacterData.ThanThongOwner;

        // Chuyển List sang HashSet để check Contains cực nhanh (O(1))
        var ownerThanThongSet = new HashSet<int>(listThanThongOwner);

        // Lọc Thần Thông: Chỉ lấy những cái ID chưa nằm trong HashSet đã sở hữu
        var filteredThanThong = thanThongItems.Where(x => !ownerThanThongSet.Contains(x.Id)).ToList();

        // Lọc Công Pháp: Chỉ lấy những cái ID chưa nằm trong Dictionary key
        var filteredCongPhap = congPhapItems.Where(x => !dictCPOwner.ContainsKey(x.Id)).ToList();

        var gameConfig = Pool.Get<GameConfig>();

        foreach ( var item in filteredThanThong)
        {
            if(item.Type == LuyenDan.ItemType.ThanThong)
            {
                ThanThong thanThong = gameConfig.GetThanThongWithId(item.Id);
                var itemLinhHoi = GameConfig.GetItemPool(prefabItemInvenPanelLinhHoi);
                InitVisualItemSkillLinhHoi(itemLinhHoi,thanThong,item);
               
            }    
        }
        foreach (var item in filteredCongPhap)
        {
            if (item.Type == LuyenDan.ItemType.CongPhap)
            {
                CongPhap congPhap = gameConfig.GetCongPhapWithId(item.Id);
                var itemLinhHoi = GameConfig.GetItemPool(prefabItemInvenPanelLinhHoi);
                InitVisualItemSkillLinhHoi(itemLinhHoi, congPhap, item);

            }
        }
     
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

    public void SuccessLinhNgo()
    {
        throw new NotImplementedException();
    }

    public void FailLinhNgo(string message)
    {
        throw new NotImplementedException();
    }
}

