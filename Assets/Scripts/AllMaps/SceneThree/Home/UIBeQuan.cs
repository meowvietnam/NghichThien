using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Analytics.IAnalytic;

public class UIBeQuan : BaseBehaviour , INetworkEvent.ITuLuyen, INetworkEvent.ILinhNgo, INetworkEvent.IDotPhaCongPhap
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
    private TimeSpan currentTuLuyenTimeSpan => GameManager.ConvertMonthToTimeSpan(currentTuLuyenTimeInt);
    private SecureInt currentTuLuyenTimeInt; // month
    [SerializeField] private Text textPriceTieuHao;
    [SerializeField] private Button btnUnTuLuyen;

    [Header("Cấu hình chi phí")]
    public double baseCost = 100f;    // Chi phí tại Lv1
    public float r = 0.2f;           // Tỉ lệ tăng mỗi cấp (10%)
    public float m = 5f;           // Hệ số nhảy vọt mỗi 3 cấp
    private CharacterData CharData => Client.Instance.CharacterData;
    public double CostPriceTieuHao1Month => CalculateCost(CharData.Progression.Level);
    private void InitPageTuLuyen()
    {
        var charData = CharData;
        
        var cpChuTu = Pool.Get<GameManager>().GetCongPhapWithId(charData.Equipment.ChuTu1Used);
        string nameCPstr = "Không";
      
        int tocDoTuLuyenCP = 0;
        if (cpChuTu != null)
        {
            nameCPstr = cpChuTu.nameItem;
            var congPhapOwners = charData.InventoryManager.CongPhapOwners;
            var level = 0;
            if (congPhapOwners.TryGetValue(new CreateCharacter.CongPhap()
                {
                    Id = charData.Equipment.ChuTu1Used
                }, out var congPhap))
            {
                level = congPhap.Level;
            }
            tocDoTuLuyenCP = cpChuTu.GetTocDoTuLuyen(level);
        }
        nameCP.text = $"《{nameCPstr}》";
        valueTuLuyenForCPText.text = tocDoTuLuyenCP.ToString();
        int percentTuChat = charData.Stats.TuChat;
        percentTuChatText.text = $"{percentTuChat}%";
        var getTamCanh = GameManager.GetTamCanhValue(charData.Stats.TamCanh);

        int percentTamCanh =  0;
        tamCanhValueText.text = "Tăng lên";
        if (getTamCanh.index > ThanThong.GetDaiCanhGioi(charData.Progression.Level))
        {
            percentTamCanh = +50;
            tamCanhValueText.text = "Tăng lên";
        }
        else if (getTamCanh.index < ThanThong.GetDaiCanhGioi(charData.Progression.Level))
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
        tuChatText.text = GameManager.tuChatWithText[charData.Stats.LinhCan];
        tamCanhText.text = getTamCanh.name;
        totalValueTuLuyenText.text = $"{GetTotalValueBuffWithMultiplier(new List<float> { percentTuChat,100,percentTamCanh})}";
        sliderTuLuyen.minValue = 0 ;
        sliderTuLuyen.maxValue = (int)(charData.Stats.ThoNguyen - GameManager.GetIntTuoiTac(charData.Stats.TuoiTac)) * 10 * 12 / 100;
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
            if (charData.Stats.LinhThach >= CostPriceTieuHao1Month * currentTuLuyenTimeInt)
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
            if (charData.Stats.LinhThach < CostPriceTieuHao1Month * currentTuLuyenTimeInt)
            {
                NotifyGroup.Instance.Show("Không đủ linh thạch");
            }
        });
        SetUpBtnTuLuyen();
        void SetUpBtnTuLuyen()
        {
            if (cpChuTu == null || charData.Stats.LinhThach < CostPriceTieuHao1Month * currentTuLuyenTimeInt)
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
    
    public bool isLoadingMore = false;
  
    
   
    [Header("Lĩnh Hội")]

    [SerializeField] private Text priceTextLinhHoi;
    [SerializeField] private Text textTimer;
    [SerializeField] private Text textInfor;
    [SerializeField] private Button btnLinhHoi;
    [SerializeField] private Button btnUnLinhHoi;
    [SerializeField] private Text textNameLinhHoi;
    [SerializeField] private Bag1ItemPrefabInventory bagLinhHoi;

 
    private void InitPageLinhNgo()
    {
        //reset 
     
        textNameLinhHoi.transform.parent.gameObject.SetActive(false);
        var charData = CharData;
        // Lấy dữ liệu gốc
        var thanThongItems = charData.InventoryManager.ThanThongs;
        var congPhapItems = charData.InventoryManager.CongPhaps;

        var setCPOwner = charData.InventoryManager.CongPhapOwners;
        var listThanThongOwner = charData.InventoryManager.ThanThongOwners.Select(x => x.Id).ToList();

        // Chuyển List sang HashSet để check Contains cực nhanh (O(1))
        var ownerThanThongSet = new HashSet<ushort>(listThanThongOwner);

        // Lọc Thần Thông: Chỉ lấy những cái ID chưa nằm trong HashSet đã sở hữu
        var filteredThanThong = thanThongItems.Where(x => !ownerThanThongSet.Contains(x.Id)).ToList();

        // Lọc Công Pháp: Chỉ lấy những cái ID chưa nằm trong Dictionary key
        var cpSetIds = setCPOwner.Select(x => x.Id).ToHashSet();
        var filteredCongPhap = congPhapItems.Where(x => !cpSetIds.Contains(x.Id)).ToList();

        var gameConfig = Pool.Get<GameManager>();

        // 1. Tạo danh sách chứa cặp (SkillData và ItemRef)

       
        var listItemRefSkill = new List<ItemRef>();
       listItemRefSkill.AddRange(filteredThanThong);
       listItemRefSkill.AddRange(filteredCongPhap);
        bagLinhHoi.filterControl.InitFilterType(new List<ItemType> { ItemType.ThanThong, ItemType.CongPhap });

        bagLinhHoi.InitDataItem(listItemRefSkill, (item) => {
            item.InitAction(null, null, () =>
            {
                Debug.Log("co event linh hoi");
                var data = item.itemData;
                var itemRef = item.itemRef;
                textNameLinhHoi.text = data.nameItem;
                textNameLinhHoi.transform.parent.gameObject.SetActive(true);
                Debug.Log("co event linh hoi2");

                int cost = (int)GameManager.GetPriceItemBuy(itemRef, null) / 2;
                priceTextLinhHoi.text = $"{cost}";
                if (itemRef.Type == ItemType.ThanThong)
                {
                    ThanThong thanThong = (ThanThong)data;
                    textInfor.text = thanThong.GetDesc(ThanThong.GetDaiCanhGioi(charData.Progression.Level));
                }
                else if (itemRef.Type == ItemType.CongPhap)
                {
                    CongPhap cp = (CongPhap)data;
                    var cpOwners = charData.InventoryManager.CongPhapOwners;
                    var r = new CreateCharacter.CongPhap()
                    {
                        Id = itemRef.Id
                    };
                    int daiCanhGioiCP = 1;
                    if (cpOwners.TryGetValue(r, out var c))
                    {
                        daiCanhGioiCP = c.Level;
                    }
                    textInfor.text = cp.GetDesc(daiCanhGioiCP);
                }
                SetEventButton(btnLinhHoi, () =>
                {
                    if (charData.Stats.LinhThach >= cost)
                    {
                        Observer.Notify(EventList.Network.LINH_NGO, this, itemRef.Id, itemRef.Type);
                    }
                });
                SetEventButton(btnUnLinhHoi, () =>
                {
                    if (charData.Stats.LinhThach < cost)
                    {
                        NotifyGroup.Instance.Show("Không đủ linh thạch");
                    }

                });
                SetUpBtnLinhHoi();
                void SetUpBtnLinhHoi()
                {
                    if (charData.Stats.LinhThach < cost)
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
           
        });
        bagLinhHoi.InitBag(false);
      


    }
    [Header("Nâng cấp Công Pháp")]

    [SerializeField] private Text priceTextUpdateCongPhap;
    [SerializeField] private ItemInventory priceItemCongPhap;
    [SerializeField] private Text textTimerUpdateCongPhap;
    [SerializeField] private Text[] textInforUpdateCongPhap;
    [SerializeField] private Button btnUpdateCongPhap;
    [SerializeField] private Button btnUnUpdateCongPhap;
    [SerializeField] private Text textNameDotPha;
    [SerializeField] private Bag1ItemPrefabInventory bagCongPhap;


    private void InitPageUpdate()
    {
      
        textNameDotPha.transform.parent.gameObject.SetActive(false);
        var charData = CharData;
        var cpOwners = charData.InventoryManager.CongPhapOwners;

        var gameConfig = Pool.Get<GameManager>();

        // 1. Tạo danh sách chứa cặp (SkillData và ItemRef)
        List<CongPhap> listSkillsWithRef = new List<CongPhap>();
        List<ItemRef> listSkillsWithItemRef = new List<ItemRef>();

        foreach (var item in cpOwners)
        {
            CongPhap skill = gameConfig.GetCongPhapWithId(item.Id);
            listSkillsWithRef.Add(skill);
            listSkillsWithItemRef.Add(new ItemRef { Id = (ushort)skill.Id, Count = 1, Type = ItemType.CongPhap });
        }



        bagCongPhap.filterControl.InitFilterType(new List<ItemType> { ItemType.CongPhap });

        bagCongPhap.InitDataItem(listSkillsWithItemRef, (item) =>
        {
            item.InitAction(null, null, () =>
            {
                var data=item.itemData;
                var itemRef = item.itemRef;

                textNameDotPha.transform.parent.gameObject.SetActive(true);

                textNameDotPha.text = data.nameItem;

                Pool.Get<UIManager>().InitVisualItemSkill(priceItemCongPhap, (BaseSkill)data, itemRef);
                var cpOwners = charData.InventoryManager.CongPhapOwners;
                var r = new CreateCharacter.CongPhap()
                {
                    Id = itemRef.Id
                };
                int daiCanhGioiCP = 1;
                if (cpOwners.TryGetValue(r, out var c))
                {
                    daiCanhGioiCP = c.Level;
                }
                int cost = (int)GameManager.GetPriceItemBuy(itemRef, null) * (daiCanhGioiCP + 1);
                int cost2 = (daiCanhGioiCP + 1) * 2;
                priceTextUpdateCongPhap.text = $"x{cost}";
                priceItemCongPhap.Init(item.itemData, item.frame.sprite);
                priceItemCongPhap.SetQualityFull(item.qualityFull.sprite);

                priceItemCongPhap.transform.parent.GetComponent<Text>().text = $"x{cost2}";
                if (itemRef.Type == ItemType.CongPhap)
                {
                    CongPhap cp = (CongPhap)data;
                    for (int i = 0; i < textInforUpdateCongPhap.Length; i++)
                    {
                        textInforUpdateCongPhap[i].text = cp.GetDesc(i + 1);
                        if (i + 1 <= daiCanhGioiCP)
                        {
                            textInforUpdateCongPhap[i].transform.GetChild(1).gameObject.SetActive(true);


                        }
                        else
                        {
                            textInforUpdateCongPhap[i].transform.GetChild(1).gameObject.SetActive(false);


                        }
                    }
                }
                SetEventButton(btnUpdateCongPhap, () =>
                {
                    if (charData.Stats.LinhThach >= cost && charData.InventoryManager.CongPhaps.Count(x => x.Id == itemRef.Id) >= cost2)
                    {
                        Observer.Notify(EventList.Network.DOT_PHA_CONGPHAP, this, itemRef.Id, itemRef.Type);
                    }
                });
                SetEventButton(btnUnUpdateCongPhap, () =>
                {
                    if (charData.Stats.LinhThach < cost)
                    {
                        NotifyGroup.Instance.Show("Không đủ linh thạch");
                    }
                    else if (charData.InventoryManager.CongPhaps.Count(x => x.Id == itemRef.Id) < cost2)
                    {
                        NotifyGroup.Instance.Show("Không đủ số lượng công pháp yêu cầu");

                    }

                });
                SetUpBtnUpdate();
                void SetUpBtnUpdate()
                {
                    if (charData.Stats.LinhThach < cost || charData.InventoryManager.CongPhaps.Count(x => x.Id == itemRef.Id) < cost2)
                    {
                        btnUnUpdateCongPhap.gameObject.SetActive(true);
                        btnUpdateCongPhap.gameObject.SetActive(false);
                    }
                    else
                    {
                        btnUnUpdateCongPhap.gameObject.SetActive(false);
                        btnUpdateCongPhap.gameObject.SetActive(true);

                    }
                }
            });
        },true);
        bagCongPhap.InitBag();
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

    public void SuccessDotPhaCongPhap()
    {
        throw new NotImplementedException();
    }

    public void FailDotPhaCongPhap(string message)
    {
        throw new NotImplementedException();
    }
}

