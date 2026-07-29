using AllMaps.ItemProvider;
using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using CreateCharacter.CharStats;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Analytics.IAnalytic;
using CongPhap = Skill.CongPhap;
using ThanThong = Skill.ThanThong;

public class UIBeQuan : BaseBehaviour, INetworkEvent.ITuLuyen, INetworkEvent.ILinhNgo, INetworkEvent.IDotPhaCongPhap,
    INetworkEvent.ISpecialLevelUp
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
    [SerializeField] public ButtonWithPage currentBtnWithPage;
    [SerializeField] private Button btnShowPopBeQuan;
    [SerializeField] public Popup popBeQuan;
    [SerializeField] private Popup popDotPha;

    [Header("InforDotPha")] [SerializeField]
    private ItemInventory prefabItemRequireDotPha;

    [SerializeField] private ItemInventory prefabItemThienDaoDotPha;
    [SerializeField] private Button btnConfirmDotPha;
    [SerializeField] private Button btnShowPopDotPha;

    private void SetupPopupDotPha(List<ItemRef> itemRefs, ItemInventory prefab)
    {
        for (int i = 0; i < prefab.transform.parent.childCount; i++)
        {
            prefab.transform.parent.GetChild(i).gameObject.SetActive(false);
        }

        foreach (var item in itemRefs)
        {
            var itemInventory = GameManager.GetItemPoolWithPrefabInParent(prefab);
            var handler = ItemTypeHandlerBagFactory.Create(item.Type);
            handler.Handle(itemInventory, item, true);
            itemInventory.InitCount(ItemProviderFactory.GetProvider(item.Type).GetCountOwner(item.Id));
            itemInventory.InitCountRequire(item.Count);
        }
    }

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


        SetEventButton(btnShowPopDotPha, () =>
        {
            var inforLevel = LevelResource.Instance.inforLevels[Client.Instance.CharacterData.Progression.Level - 1];
            if (inforLevel.IsSpecialLevelUp &&
                Client.Instance.CharacterData.Progression.CurrentExp >= inforLevel.tuviRequireNextLevel)
            {
                popDotPha.Show();
                SetupPopupDotPha(inforLevel.RequireItem, prefabItemRequireDotPha);
                var btnGreyConfirmDotPha = btnConfirmDotPha.transform.GetChild(0).GetComponent<Button>();
                var canDotPha = inforLevel.RequireItem.All(x =>
                    ItemProviderFactory.GetProvider(x.Type)?.GetCountOwner(x.Id) >= x.Count);
                btnGreyConfirmDotPha.gameObject.SetActive(!canDotPha);
                if (canDotPha)
                {
                    SetEventButton(btnConfirmDotPha, () =>
                    {
                        popDotPha.Hide();
                        Observer.Notify(EventList.Network.SPECIAL_LEVELUP, this);
                    });
                }
                else
                {
                    SetEventButton(btnGreyConfirmDotPha,
                        () => { NotifyGroup.Instance.Show("Không đủ nguyên liệu cần để đột phá"); });
                }

                SetupPopupDotPha(inforLevel.BuffItem, prefabItemThienDaoDotPha);
            }
            else
            {
                NotifyGroup.Instance.Show("Tu vi của bạn cần đạt tới bình cảnh để có thể đột phá");
            }
        });
    }

    [Header("Tu Luyện")] [SerializeField] private Text nameCP;
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

    [Header("Cấu hình chi phí")] public double baseCost = 100f; // Chi phí tại Lv1
    public float r = 0.2f; // Tỉ lệ tăng mỗi cấp (10%)
    public float m = 5f; // Hệ số nhảy vọt mỗi 3 cấp
    private CharacterData CharData => Client.Instance.CharacterData;
    public double CostPriceTieuHao1Month => CalculateCost(CharData.Progression.Level);

    public static int GetTotalTuLuyen(float percentTuChat,float percentTamCanh,float tocDoTuLuyenCP,int tongSoThang)
    {
        var buffExp = GetTotalValueBuffWithMultiplier(new List<float> { percentTuChat, 100, percentTamCanh }, (int)tocDoTuLuyenCP) *
                            tongSoThang;
        var ability = Client.Instance.CharacterData.Stats.Ability;
        if (ability.HasFlag(Ability.TienThienDaoThe))
        {
            buffExp += (int)MathF.Ceiling(buffExp * 0.1f);
        }

        if (ability.HasFlag(Ability.HuyenAmTuyetMach))
        {
            buffExp -= (int)MathF.Ceiling(buffExp * 0.1f);
        }
        return buffExp;
    }
    public static int GetTotalValueBuffWithMultiplier(List<float> percent, int tocDoTuLuyenCP)
    {
        float total = tocDoTuLuyenCP;
        Debug.Log($"TDTL: {total}");
        foreach (var item in percent)
        {
            total *= (1 + item / 100f);
        }

        return (int)total;
    }    
    public static float GetPercentTamCanhPlayer()
    {
        float percentTamCanh = 0;
        var getTamCanh = GameManager.GetTamCanhValue(Client.Instance.CharacterData.Stats.TamCanh);
        if (getTamCanh.index > ThanThong.GetDaiCanhGioi(Client.Instance.CharacterData.Progression.Level))
        {
            percentTamCanh = +50;
        }
        else if (getTamCanh.index < ThanThong.GetDaiCanhGioi(Client.Instance.CharacterData.Progression.Level))
        {
            percentTamCanh = -50;
        }
        return percentTamCanh;
    }
    public static int GetTocDoTuLuyenCharData()
    {
        var charData =Client.Instance.CharacterData;
        var cpChuTu = Pool.Get<GameManager>().GetCongPhapWithId(charData.EquipmentManager.ChuTu1Used);

        int tocDoTuLuyenCP = 0;
        if (cpChuTu != null)
        {
            var congPhapOwners = charData.InventoryManager.CongPhapOwners;
            var level = 0;
            if (congPhapOwners.TryGetValue(new CreateCharacter.CongPhap()
            {
                Id = charData.EquipmentManager.ChuTu1Used
            }, out var congPhap))
            {
                level = congPhap.Level;
            }

            tocDoTuLuyenCP = cpChuTu.GetTocDoTuLuyen(level);
        }
        return tocDoTuLuyenCP;
    }
    private void InitPageTuLuyen()
    {
        var charData = CharData;

        var cpChuTu = Pool.Get<GameManager>().GetCongPhapWithId(charData.EquipmentManager.ChuTu1Used);
        string nameCPstr = "Không";

        int tocDoTuLuyenCP = GetTocDoTuLuyenCharData();
        if (cpChuTu != null)
        {
            nameCPstr = cpChuTu.nameItem;
           
        }

        nameCP.text = $"《{nameCPstr}》";
        valueTuLuyenForCPText.text = tocDoTuLuyenCP.ToString();
        int percentTuChat = charData.Stats.TuChat;
        percentTuChatText.text = $"{percentTuChat}%";
        var getTamCanh = GameManager.GetTamCanhValue(Client.Instance.CharacterData.Stats.TamCanh);
        float percentTamCanh = GetPercentTamCanhPlayer();
        tamCanhValueText.text = "Tăng lên";
        if (percentTamCanh>0)
        {
            percentTamCanh = +50;
            tamCanhValueText.text = "Tăng lên";
        }
        else if (percentTamCanh<0)
        {
            percentTamCanh = -50;
            tamCanhValueText.text = "Giảm xuống";
        }

        percentTamCanhText.text = $"{percentTamCanh}%";

       

        tuChatText.text = GameManager.tuChatWithText[charData.Stats.LinhCan];
        tamCanhText.text = getTamCanh.name;
        totalValueTuLuyenText.text = "0";
        sliderTuLuyen.minValue = 0;
        sliderTuLuyen.value = 0;
        var value = Math.Max(0, charData.Stats.LinhThach / 100);
        value = Math.Min(value, 120);
        sliderTuLuyen.maxValue = value;
        sliderTuLuyen.onValueChanged.RemoveAllListeners();
        sliderTuLuyen.onValueChanged.AddListener((value) =>
        {
            // 1. Ép kiểu value về int (vì thời gian tu luyện của bạn là SecureInt/int)
            int tongSoThang = Mathf.RoundToInt(value);
            if (tongSoThang > 0)
            {
             

                totalValueTuLuyenText.text = $"{GetTotalTuLuyen(percentTuChat,percentTamCanh,tocDoTuLuyenCP,tongSoThang)}";
            }
            else
            {
                totalValueTuLuyenText.text = "0";
            }

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
                if (percentTamCanh >= 0)
                {
                    ActionTuLuyen();
                }
                else if (percentTamCanh < 0)
                {
                    Pool.Get<PopupConfirmAction>().ShowPopConfirmAction(
                        $"Tâm cảnh của bạn đang thấp hơn tu vi dẫn tới hiệu quả tu luyện giảm đi 50% vẫn tiếp tục tu luyện chứ?",
                        () => { ActionTuLuyen(); });
                }
            }

            void ActionTuLuyen()
            {
                Pool.Get<PopupConfirmAction>().ShowPopConfirmAction(
                    $"Xác nhận tiêu hao {CostPriceTieuHao1Month * currentTuLuyenTimeInt} linh thạch để tu luyện trong {textNam.text} năm {textThang.text} tháng",
                    () =>
                    {
                        Observer.Notify(EventList.Network.TU_LUYEN, this, currentTuLuyenTimeSpan);
                        popBeQuan.Hide();
                    });
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


    [Header("Lĩnh Hội")] [SerializeField] private Text priceTextLinhHoi;
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

        bagLinhHoi.InitDataItem(listItemRefSkill, (item) =>
        {
            item.InitAction(null, null, () =>
            {
                Debug.Log("co event linh hoi");
                var data = item.itemData;
                var itemRef = item.itemRef;
                textNameLinhHoi.text = data.nameItem;
                textNameLinhHoi.transform.parent.gameObject.SetActive(true);
                Debug.Log("co event linh hoi2");
                var provider = ItemProviderFactory.GetProvider(item.itemRef.Type);
                int cost = provider.GetPriceItemBuy(item.itemRef.Id);
                priceTextLinhHoi.text = $"{cost}";
                if (data is BaseSkill skill)
                {
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

                    textTimer.text =
                        GameManager.GetDateTimeText(skill.baseTimeLearn /
                                                    (Client.Instance.CharacterData.Stats.NgoTinh));
                }

                SetEventButton(btnLinhHoi, () =>
                {
                    Debug.Log("btnLinhHoi");
                    if (charData.Stats.LinhThach >= cost)
                    {
                        Debug.Log("Xác nhận lĩnh ngộ");

                        Pool.Get<PopupConfirmAction>().ShowPopConfirmAction($"Xác nhận lĩnh ngộ {data.nameItem}", () =>
                        {
                            item.gameObject.SetActive(false);
                            textNameLinhHoi.transform.parent.gameObject.SetActive(false);
                            Observer.Notify(EventList.Network.LINH_NGO, this, (int)itemRef.Id, itemRef.Type);
                        });
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
        }, false, true);
        bagLinhHoi.InitBag(false);
    }

    [Header("Nâng cấp Công Pháp")] [SerializeField]
    private Text priceTextUpdateCongPhap;

    [SerializeField] private ItemInventory priceItemCongPhap;
    [SerializeField] private Text textTimerUpdateCongPhap;
    [SerializeField] private Text[] textInforUpdateCongPhap;
    [SerializeField] private Button btnUpdateCongPhap;
    [SerializeField] private Button btnUnUpdateCongPhap;
    [SerializeField] private Text textNameDotPha;
    [SerializeField] private Bag1ItemPrefabInventory bagCongPhap;

    [SerializeField] private int idCpUpdate;
    [SerializeField] private ETypeSlotCP typeSlot = ETypeSlotCP.None;

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
                var data = item.itemData;
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

                var provider = ItemProviderFactory.GetProvider(item.itemRef.Type);
                int cost = provider.GetPriceItemBuy(item.itemRef.Id);
                int cost2 = (daiCanhGioiCP + 1);
                int costLinhThach = cost * cost2;
                priceTextUpdateCongPhap.text = $"x{costLinhThach}";
                priceItemCongPhap.Init(item.itemData, item.frame.sprite);
                priceItemCongPhap.SetQualityFull(item.qualityFull.sprite);
                priceItemCongPhap.transform.parent.GetComponent<Text>().text = $"x{cost2}";
                if (data is BaseSkill skill)
                {
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

                    textTimerUpdateCongPhap.text =
                        GameManager.GetDateTimeText(skill.baseTimeLearn / Client.Instance.CharacterData.Stats.NgoTinh);
                }

                SetEventButton(btnUpdateCongPhap, () =>
                {
                    var cpRef = charData.InventoryManager.CongPhaps.FirstOrDefault(x => x.Id == itemRef.Id);

                    if (charData.Stats.LinhThach >= costLinhThach && cpRef != null && cpRef.Count >= cost2)
                    {
                        Pool.Get<PopupConfirmAction>().ShowPopConfirmAction($"Xác nhận đột phá {data.nameItem}", () =>
                        {
                            var typeSlotRef = Pool.Get<TabPanel>().GetTypeSlotCP(itemRef.Id);
                            if (typeSlotRef != ETypeSlotCP.None)
                            {
                                typeSlot = typeSlotRef;
                                idCpUpdate = itemRef.Id;
                                Observer.Notify(EventList.Network.UNEQUIP_CONGPHAP, Pool.Get<TabPanel>(), typeSlotRef);
                            }

                            textNameDotPha.transform.parent.gameObject.SetActive(false);
                            Observer.Notify(EventList.Network.DOT_PHA_CONGPHAP, this, (int)itemRef.Id);
                        });
                    }
                });
                SetEventButton(btnUnUpdateCongPhap, () =>
                {
                    if (charData.Stats.LinhThach < costLinhThach)
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
                    var cpRef = charData.InventoryManager.CongPhaps.FirstOrDefault(x => x.Id == itemRef.Id);
                    if (charData.Stats.LinhThach < costLinhThach || (cpRef != null && cpRef.Count < cost2))
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
        }, true);
        bagCongPhap.InitBag();
    }

    public void SuccessTuLuyen(int valueAdd, int valueAddHp, int valueDecreaseTamCanh)
    {
        Observer.Notify(EventList.GamePlay.TIME_ADJUST);
        var notifyGroup = NotifyGroup.Instance;
        notifyGroup.Show(
            $"Tu luyện thành công {textNam.text} năm {textThang.text} tháng nhận được {valueAdd} tu vi");
        notifyGroup.Show($"Khôi phục +{valueAddHp} khí huyết");
        notifyGroup.Show($"Giảm {valueDecreaseTamCanh} tâm cảnh");
        Observer.Notify(EventList.GamePlay.UPDATE_PLAYER_STATS);
    }


    public void FailTuLuyen(string message)
    {
        NotifyGroup.Instance.Show(message);
    }

    public void SuccessLinhNgo()
    {
        NotifyGroup.Instance.Show("Lĩnh ngộ thành công");
        Observer.Notify(EventList.GamePlay.TIME_ADJUST);
    }

    public void FailLinhNgo(string message)
    {
        NotifyGroup.Instance.Show(message);
    }

    public void SuccessDotPhaCongPhap()
    {
        NotifyGroup.Instance.Show("Đột phá thành công");
        if (typeSlot != ETypeSlotCP.None)
        {
            Observer.Notify(EventList.Network.EQUIP_CONGPHAP, Pool.Get<TabPanel>(), typeSlot, (ushort)idCpUpdate);
            typeSlot = ETypeSlotCP.None;
        }

        Observer.Notify(EventList.GamePlay.TIME_ADJUST);
    }

    public void FailDotPhaCongPhap(string message)
    {
        NotifyGroup.Instance.Show(message);
        if (typeSlot != ETypeSlotCP.None)
        {
            Observer.Notify(EventList.Network.EQUIP_CONGPHAP, Pool.Get<TabPanel>(), typeSlot, (ushort)idCpUpdate);
            typeSlot = ETypeSlotCP.None;
        }
    }

    public void SuccessSpecialLevelUp()
    {
        NotifyGroup.Instance.Show("Đột phá thành công");
        Observer.Notify(EventList.GamePlay.UPDATE_PLAYER_STATS);
    }

    public void FailSpecialLevelUp(string message)
    {
        NotifyGroup.Instance.Show(message);
    }
}