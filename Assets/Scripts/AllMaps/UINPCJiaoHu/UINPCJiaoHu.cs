using Base;
using CreateCharacter;
using DG.Tweening;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public enum ERelationShip
{
    None,
    Quen,
    BanBe,
    ThanThiet,
}
public class UINPCJiaoHu : PanelGroup , INetworkEvent.ITangQuaNPC , INetworkEvent.IThinhGiaoNPC
{
    [SerializeField]
    public static Dictionary<ERelationShip, Color> relationShipColor = new Dictionary<ERelationShip, Color>
    {
        { ERelationShip.None, Color.white },
        { ERelationShip.Quen, Color.green },
    // Use ColorUtility to parse the hex string
        { ERelationShip.BanBe, HexToColor("FF9292") },
        { ERelationShip.ThanThiet, Color.red },
    };
    public static Dictionary<ERelationShip, string> relationShipText = new Dictionary<ERelationShip, string>
    {
        { ERelationShip.None, "Lạ" }, // Use { key, value }
        { ERelationShip.Quen, "Quen Biết" },
        { ERelationShip.BanBe, "Bằng Hữu" },
        { ERelationShip.ThanThiet, "Thân Mật" },

    };
    public static Dictionary<ERelationShip, int> relationShipRequireThanThong = new Dictionary<ERelationShip, int>
    {
        { ERelationShip.None, 1 }, // Use { key, value }
        { ERelationShip.Quen, 100 },
        { ERelationShip.BanBe, 500 },
        { ERelationShip.ThanThiet, 1000 },

    };
    public static Dictionary<ERelationShip, int> relationShipRequireCongPhap = new Dictionary<ERelationShip, int>
    {
        { ERelationShip.None, 1 }, // Use { key, value }
        { ERelationShip.Quen, 200 },
        { ERelationShip.BanBe, 1000 },
        { ERelationShip.ThanThiet, 2000 },

    };
    public enum ETypeSkillRelationShip
    {
        ThanThong,
        CongPhap
    }
    public static ERelationShip GetERelationShip(int myScore, ETypeSkillRelationShip eType)
    {
        Dictionary<ERelationShip, int> relationRequire = eType == ETypeSkillRelationShip.ThanThong ? relationShipRequireThanThong : relationShipRequireCongPhap;
        return relationRequire
        .Where(pair => myScore >= pair.Value) // Lọc các mốc mà điểm của bạn đã đạt tới
        .OrderByDescending(pair => pair.Value) // Sắp xếp từ cao xuống thấp
        .FirstOrDefault() // Lấy mốc cao nhất
        .Key;
    }
    [SerializeField] public Popup npcPop;
    [SerializeField] public Popup xemXetPop;
    [SerializeField] public Popup thinhGiaoPop;
    [SerializeField] public Popup tangQuaPop;
   
    [SerializeField]
    public NPCLeftSVItem prefabNPCLeftItem;
    [SerializeField]
    public Text countCharText;
    [SerializeField] private CharacterStatsSecure characterStatsTarget;

    [Header("--- Pop JiaoHuo ---")]

    [SerializeField] private CharacterGraphic charVisualPopJiaoHua;
    [SerializeField]
    private Button btnTroChuyen;
    [SerializeField]
    private Button btnLuanBan;
    [SerializeField]
    private Button btnTangQua;
    [SerializeField]
    private Button btnThinhGiao;
    [SerializeField]
    private Button btnXemXet;
    [SerializeField]
    private Button btnChanDanh;
    [SerializeField]
    private Button btnBack;
    [SerializeField]
    private Button closeOrOpenListNPC;
   
    [SerializeField] private Text textName;
    [Header("--- Pop Thỉnh Giáo ---")]

    [SerializeField] private Text textRelationshipValue;
    [SerializeField] private Image sliderRelationship;
    [SerializeField] private Sprite spriteSelectTab;
    [SerializeField] private Sprite spriteNoSelectTab;
    [SerializeField]
    private Text relateText1;
    [SerializeField] private Text relateText2;
    [SerializeField] private Text relateText3;
    [SerializeField] private ItemInventory prefabSlotThinhGiaoSlot1;
    [SerializeField] private ItemInventory prefabSlotThinhGiaoSlot2;
    [SerializeField] private ItemInventory prefabSlotThinhGiaoSlot3;
    [SerializeField] private Button btnTabThanThong;
    [SerializeField] private Button btnTabCongPhap;
    [SerializeField] private Color colorNoneRelation => HexToColor("7A644A");
    [SerializeField] private ERelationShip currentRelationShip = ERelationShip.None;
    [SerializeField] private Sprite spriteIconThanThongSelect;
    [SerializeField] private Sprite spriteIconThanThongNoSelect;
    [SerializeField] private Sprite spriteIconCongPhapSelect;
    [SerializeField] private Sprite spriteIconCongPhapNoSelect;
    [SerializeField] private Color colorTextSelect => HexToColor("F8D8A3");
    [SerializeField] private Color colorTextNoSelect => HexToColor("3F8784");
    public ETypeSkillRelationShip CurrentPageTypeRelationShip;

    private int relatetionshipValue => Client.Instance.CharacterData.RelationShip.GetValueOrDefault(characterStatsTarget.idChar);
    [Header("--- Pop Dò Xét ---")]
    [SerializeField] private CharacterGraphic charVisualPopXemXet;
    [SerializeField] private Text textThanThuc;
    [SerializeField] private Text textTocDoBay;
    [SerializeField] private Text textNgoTinh;
    [SerializeField] private Text textTuChat;
    [SerializeField] private Text textThoNguyen;
    [SerializeField] private Text textSinhMenh;
    [SerializeField] private Text textTuoiTac;
    [SerializeField] private Text textLevel;
    [SerializeField] private Text textNamePopXemXet;
    [SerializeField] private Text textHaoCam;
    [SerializeField] private List<ItemInventory> listThanThongItemSlot;
    [SerializeField] private List<ItemInventory> listCongPhapItemSlot;
    [Header("--- Pop Tặng Quà ---")]
    [SerializeField] private ItemInventory slotItemGift;
    [SerializeField] private Text textMoney;
    [SerializeField] private Text textNameUser;
    [SerializeField] private Text textHaoCamPopTangQua;
    [SerializeField] private CharacterGraphic visualChar;
  
    [SerializeField] private Button btnConfirmTangQua;
    [SerializeField]
    private Bag1ItemPrefabInventory bag;

     public void InitPopTangQua()
    {
        var characterData = Client.Instance.CharacterData;
        var gameConfig = Pool.Get<GameManager>();
        List<ItemRef> itemDatas = new List<ItemRef>();
        itemDatas.AddRange(characterData.InventoryManager.Herbs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.Recipes.ToList());
        itemDatas.AddRange(characterData.InventoryManager.ThanThongs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.CongPhaps.ToList());
        itemDatas.AddRange(characterData.InventoryManager.VuKhis.ToList());
        itemDatas.AddRange(characterData.InventoryManager.PhongCus.ToList());
        itemDatas.AddRange(characterData.InventoryManager.TrangSucs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.LinhDans.ToList());
        // Sử dụng LINQ để trích xuất Ref từ mỗi phần tử trong furnaceDatas
        var furnaceRefs = characterData.InventoryManager.Furnaces.Select(f => f.Ref).ToList();

        // Thêm danh sách Ref vừa lấy được vào itemDatas
        itemDatas.AddRange(furnaceRefs);
        bag.InitDataItem(itemDatas, (ItemInventory) =>
        {
            ItemInventory.InitAction(null, null, () =>
            {


                if (slotItemGift.itemRef.Id != ItemInventory.itemRef.Id)
                {
                    if (ItemInventory.itemRef.Type == ItemType.ThanThong || ItemInventory.itemRef.Type == ItemType.CongPhap)
                    {
                        Pool.Get<UIManager>().InitVisualItemSkill(slotItemGift, (BaseSkill)ItemInventory.itemData);


                    }
                    Pool.Get<UIManager>().InitVisualItemInventory(slotItemGift, ItemInventory.iItemData, ItemInventory.itemRef);
                    slotItemGift.InitCount(1);
                }
                else
                {
                    if (slotItemGift.count < ItemInventory.count)
                    {
                        slotItemGift.AddCount(1);
                    }
                }

                //foreach (var item in listItemSlotThanThongs)
                //{
                //    if (item.itemData != null)
                //    {
                //        DisableHighlight(item);
                //    }
                //}
            });
        });
        bag.InitBag(false);
        //reset 
        visualChar.SetGender(characterData.Gender);
        visualChar.SetSkin(characterData.Skins, characterData.Colors);
        textMoney.text = characterData.Stats.LinhThach.ToString();
        textNameUser.text = characterData.Name;
        SetEventButton(btnConfirmTangQua, () =>
        {
            if (slotItemGift.itemRef != null && slotItemGift.count > 0)
            {
                Observer.Notify(EventList.Network.TANG_QUA_NPC, this, characterStatsTarget.idChar, slotItemGift.itemRef.Id, slotItemGift.count);
            }
        });
        textHaoCamPopTangQua.text = Client.Instance.CharacterData.RelationShip.GetValueOrDefault(characterStatsTarget.idChar).ToString();
    }

    public void ShowNPCList(List<CharacterStatsSecure> listCharStats)
    {
        npcPop.Hide();
        thinhGiaoPop.Hide();
        xemXetPop.Hide();
        tangQuaPop.Hide();
        OpenOrCloseNpcList(true);
        ResetVisual(prefabNPCLeftItem.transform);
        SetEventButton(closeOrOpenListNPC, () =>
        {
            EventBtnOpenOrCloseNPCList();
        });
        foreach (var item in listCharStats)
        {
            var itemNpc = GameManager.GetItemPool(prefabNPCLeftItem);
            itemNpc.Init(item);

        }
        countCharText.text = listCharStats.Count.ToString();
        transform.SetAsLastSibling();

    }    
    private void EventBtnOpenOrCloseNPCList()
    {
        float openScaleBtn = -1;
        bool isOpen = closeOrOpenListNPC.transform.localScale.x == openScaleBtn;
        OpenOrCloseNpcList(isOpen);
    }
    public void OpenOrCloseNpcList(bool isWantOpen)
    {
        //float openX = 240;
        //float closeX = 0;
        float openScaleBtn = -1;
        Transform npcList = closeOrOpenListNPC.transform.parent.transform;

        if (!isWantOpen)
        {
            // close
            npcList.DOMove(transform.GetChild(0).transform.position,0.5f);
            closeOrOpenListNPC.transform.localScale = new Vector3(openScaleBtn ,1,1);
        }    
        else
        {
            npcList.DOMove(transform.GetChild(1).transform.position, 0.5f);
            closeOrOpenListNPC.transform.localScale = new Vector3(openScaleBtn * -1, 1, 1);


        }

    }    
    public override void Init()
    {
        base.Init();

        SetEventButton(btnBack, () =>
        {
            HideAllPop();
        });
        SetEventButton(btnXemXet, () =>
        {
            HideAllPop();
            xemXetPop.Show();
            InitPopDoXet();
        });
        SetEventButton(btnTangQua, () =>
        {
            HideAllPop();
            tangQuaPop.Show();
            InitPopTangQua();
           

        });
        SetEventButton(btnThinhGiao, () =>
        {
            HideAllPop();
            thinhGiaoPop.Show();
            InitPopThinhGiao();

        });
        SetEventButton(btnLuanBan, () =>
        {
            HideAllPop();
            Pool.Get<GameManager>().GoToCombat(new GameManager.CharCombatInit
            {
                charStats = characterStatsTarget,
                eTypeVisual = ETypeVisualChar.Character,
            });

        });
    }
    private void HideAllPop()
    {
        npcPop.Hide();
        xemXetPop.Hide();
        tangQuaPop.Hide();
        thinhGiaoPop.Hide();

    }
    private void SetSliderVisual(ETypeSkillRelationShip eType)
    {
        float amount = relatetionshipValue / (eType == ETypeSkillRelationShip.CongPhap?relationShipRequireCongPhap[currentRelationShip] : relationShipRequireThanThong[currentRelationShip]);
        float final = amount + amount * 0.25f;
        sliderRelationship.fillAmount = final;
       
        switch (currentRelationShip)
        {
            case ERelationShip.None:
                relateText1.color = colorNoneRelation;
                relateText2.color = colorNoneRelation;
                relateText3.color = colorNoneRelation;

                // Set lock
                prefabSlotThinhGiaoSlot1.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
                prefabSlotThinhGiaoSlot2.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
                prefabSlotThinhGiaoSlot3.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
                break;
            case ERelationShip.Quen:
                relateText1.color = Color.white;
                relateText2.color = colorNoneRelation;
                relateText3.color = colorNoneRelation;
                //Set lock
                prefabSlotThinhGiaoSlot1.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
                prefabSlotThinhGiaoSlot2.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
                prefabSlotThinhGiaoSlot3.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
                break;
            case ERelationShip.BanBe:
                relateText1.color = Color.white;
                relateText2.color = Color.white;
                relateText3.color = colorNoneRelation;
                // set lock
                prefabSlotThinhGiaoSlot1.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
                prefabSlotThinhGiaoSlot2.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
                prefabSlotThinhGiaoSlot3.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);

                break;
            case ERelationShip.ThanThiet:
                relateText1.color = Color.white;
                relateText2.color = Color.white;
                relateText3.color = Color.white;
                //Set lock
                prefabSlotThinhGiaoSlot1.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
                prefabSlotThinhGiaoSlot2.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
                prefabSlotThinhGiaoSlot3.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
                break;
            default:
                break;
        }

    }
  
    private (List<BaseSkill>,List<BaseSkill>,List<BaseSkill>) CategorizeSlot(List<BaseSkill> listThanThongData)
    {
        // Giả sử listThanThongData là danh sách các đối tượng có thuộc tính eTierSkill

        List<BaseSkill> list1 = new List<BaseSkill>();
        List<BaseSkill> list2 = new List<BaseSkill>();
        List<BaseSkill> list3 = new List<BaseSkill>();
        // 1. Kiểm tra list có null hoặc trống không
        if (listThanThongData == null || listThanThongData.Count == 0)
            return (list1, list2, list3);

        // 2. Lọc bỏ các phần tử null bên trong list trước khi tìm Max
        // và đảm bảo x không null khi truy cập eTierSkill
        var validItems = listThanThongData.Where(x => x != null).ToList();

        if (validItems.Count == 0)
            return (list1, list2, list3);

        var maxTier = validItems.Max(x => x.eTierSkill);

        foreach (var item in validItems)
        {
            var tier = item.eTierSkill;

            // TRƯỜNG HỢP 1: Có Thiên Giai (Bất kể Hạ/Trung/Thượng)
            if (maxTier >= BaseSkill.ETierSkill.ThienGiaiHa)
            {
                if (tier >= BaseSkill.ETierSkill.ThienGiaiHa) list3.Add(item);
                else if (tier >= BaseSkill.ETierSkill.DiaGiaiHa) list2.Add(item);
                else if (tier >= BaseSkill.ETierSkill.NhanGiaiHa) list1.Add(item);
            }
            // TRƯỜNG HỢP 2: Cao nhất là Địa Giai
            else if (maxTier >= BaseSkill.ETierSkill.DiaGiaiHa)
            {
                if (tier >= BaseSkill.ETierSkill.DiaGiaiTrung) list3.Add(item); // Địa Thượng + Trung
                else if (tier == BaseSkill.ETierSkill.DiaGiaiHa) list2.Add(item); // Địa Hạ
                else if (tier >= BaseSkill.ETierSkill.NhanGiaiHa) list1.Add(item); // Nhân Giai
            }
            // TRƯỜNG HỢP 3: Cao nhất chỉ có Nhân Giai
            else if (maxTier >= BaseSkill.ETierSkill.NhanGiaiHa)
            {
                if (tier == BaseSkill.ETierSkill.NhanGiaiThuong) list3.Add(item);
                else if (tier == BaseSkill.ETierSkill.NhanGiaiTrung) list2.Add(item);
                else if (tier == BaseSkill.ETierSkill.NhanGiaiHa) list1.Add(item);
            }
        }
       
        return (list1, list2,list3);
    }
    private void CategorizeSlotAndInitVisual(List<BaseSkill> listBaseSkillData,ERelationShip eTypeRelationShip)
    {
        var categorize = CategorizeSlot(listBaseSkillData);
        foreach (var item in categorize.Item1)
        {
            var itemInventory = GameManager.GetItemPool(prefabSlotThinhGiaoSlot1);
            itemInventory.InitAction(null, null, null);

            Pool.Get<UIManager>().InitVisualItemSkill(itemInventory, item);
            if(eTypeRelationShip  == ERelationShip.Quen)
            {
                CallActionThinhGiao(itemInventory);

            }
        }
        foreach (var item in categorize.Item2)
        {
            var itemInventory = GameManager.GetItemPool(prefabSlotThinhGiaoSlot2);
            itemInventory.InitAction(null, null, null);

            Pool.Get<UIManager>().InitVisualItemSkill(itemInventory, item);
            if (eTypeRelationShip == ERelationShip.BanBe)
            {
                CallActionThinhGiao(itemInventory);

            }
        }
        foreach (var item in categorize.Item3)
        {
            var itemInventory = GameManager.GetItemPool(prefabSlotThinhGiaoSlot3);
            itemInventory.InitAction(null, null,null);

            Pool.Get<UIManager>().InitVisualItemSkill(itemInventory, item);
            if (eTypeRelationShip == ERelationShip.ThanThiet)
            {
                CallActionThinhGiao(itemInventory);

            }
        }
        void CallActionThinhGiao(ItemInventory item)
        {
            item.InitAction(null, null, () =>
            {
                int require = CurrentPageTypeRelationShip == ETypeSkillRelationShip.ThanThong? relationShipRequireThanThong[eTypeRelationShip] : relationShipRequireCongPhap[eTypeRelationShip];
                if(relatetionshipValue >= require)
                {
                    Pool.Get<PopupConfirmAction>().ShowPopConfirmAction($"Xác nhận tiêu hao {require} điểm hảo cảm thỉnh giáo {item.itemData.nameItem}", () =>
                    {
                        Observer.Notify(EventList.Network.THINH_GIAO_NPC,this,characterStatsTarget.idChar,item.itemRef.Id,item.itemRef.Type);
                    });
                }    
            });
        }
    }    
    private void InitCongPhapTab()
    {
        ResetVisual(prefabSlotThinhGiaoSlot1.transform);
        ResetVisual(prefabSlotThinhGiaoSlot2.transform);
        ResetVisual(prefabSlotThinhGiaoSlot3.transform);
        var GameConfig = Pool.Get<GameManager>();

        this.currentRelationShip = GetERelationShip(relatetionshipValue, ETypeSkillRelationShip.CongPhap);
        btnTabThanThong.image.sprite = spriteNoSelectTab;
        btnTabCongPhap.image.sprite = spriteSelectTab;
        List<CongPhap> listCongPhapData = new List<CongPhap>();
        listCongPhapData.Add(GameConfig.GetCongPhapWithId(characterStatsTarget.chuTu1Used.Value));
        listCongPhapData.Add(GameConfig.GetCongPhapWithId(characterStatsTarget.chuTu2Used.Value));
        listCongPhapData.Add(GameConfig.GetCongPhapWithId(characterStatsTarget.phuTu1Used.Value));
        listCongPhapData.Add(GameConfig.GetCongPhapWithId(characterStatsTarget.phuTu2Used.Value));
        listCongPhapData.Add(GameConfig.GetCongPhapWithId(characterStatsTarget.phuTu3Used.Value));
        listCongPhapData.Add(GameConfig.GetCongPhapWithId(characterStatsTarget.phuTu4Used.Value));
        CategorizeSlotAndInitVisual(listCongPhapData.Cast<BaseSkill>().ToList(), currentRelationShip);
        ResetVisualTab();


        SetSliderVisual(ETypeSkillRelationShip.CongPhap);
        SetVisualBtnThinhGiao(btnTabCongPhap, colorTextSelect, spriteIconCongPhapSelect);
        CurrentPageTypeRelationShip = ETypeSkillRelationShip.CongPhap;

    }

    private void InitThanThongTab()
    {
        ResetVisualTab();
        ResetVisual(prefabSlotThinhGiaoSlot1.transform);
        ResetVisual(prefabSlotThinhGiaoSlot2.transform);
        ResetVisual(prefabSlotThinhGiaoSlot3.transform);
        this.currentRelationShip = GetERelationShip(relatetionshipValue, ETypeSkillRelationShip.ThanThong);
        btnTabThanThong.image.sprite = spriteSelectTab;
        btnTabCongPhap.image.sprite = spriteNoSelectTab;
        List<ThanThong> listThanThongData = new List<ThanThong>();
        var GameConfig = Pool.Get<GameManager>();

        foreach (var item in characterStatsTarget.thanThongEquip)
        {
            listThanThongData.Add(GameConfig.GetThanThongWithId(item.Value));
        }
        CategorizeSlotAndInitVisual(listThanThongData.Cast<BaseSkill>().ToList(), currentRelationShip);
        SetSliderVisual(ETypeSkillRelationShip.ThanThong);
        SetVisualBtnThinhGiao(btnTabThanThong, colorTextSelect, spriteIconThanThongSelect);
        CurrentPageTypeRelationShip = ETypeSkillRelationShip.ThanThong;

    }
    private void ResetVisual(Transform prefab)
    {
        for (int i = 1; i < prefab.transform.parent.childCount; i++)
        {
            prefab.transform.parent.GetChild(i).gameObject.SetActive(false);
        }
        
    }
    private void SetVisualBtnThinhGiao(Button btn, Color colorText, Sprite spriteIcon)
    {
        btn.transform.GetChild(0).GetComponent<Text>().color = colorText;
        btn.transform.GetChild(1).GetComponent<Image>().sprite = spriteIcon;

    }
    private void ResetVisualTab()
    {
        SetVisualBtnThinhGiao(btnTabCongPhap, colorTextNoSelect, spriteIconCongPhapNoSelect);
        SetVisualBtnThinhGiao(btnTabThanThong, colorTextNoSelect, spriteIconThanThongNoSelect);
    }    
    private void InitPopThinhGiao()
    {
        Client.Instance.CharacterData.RelationShip.TryGetValue(characterStatsTarget.idChar,out int relatetionshipValue);
        textRelationshipValue.text = relatetionshipValue.ToString();
        InitCongPhapTab();
        SetEventButton(btnTabCongPhap, () =>
        {
            InitCongPhapTab();
        });
        SetEventButton(btnTabThanThong, () =>
        {
            InitThanThongTab();
        });




    }
   
    public void ShowNpcPop(CharacterStatsSecure charStats)
    {
        this.characterStatsTarget = charStats;
        npcPop.Show();
        textName.text = characterStatsTarget.nameUser;
        charVisualPopJiaoHua.SetGender(characterStatsTarget.Gender);
        charVisualPopJiaoHua.SetSkin(characterStatsTarget.Skins);


    }
    public void InitPopDoXet()
    {
        textNamePopXemXet.text = characterStatsTarget.nameUser;
        charVisualPopXemXet.SetGender(characterStatsTarget.Gender);
        charVisualPopXemXet.SetSkin(characterStatsTarget.Skins);
        textThanThuc.text = characterStatsTarget.thanThuc.Value.ToString();
        textTocDoBay.text = characterStatsTarget.tocDoBay.Value.ToString();
        textNgoTinh.text = characterStatsTarget.ngoTinh.Value.ToString();
        textTuChat.text = characterStatsTarget.tuChat.Value.ToString();
        textThoNguyen.text = characterStatsTarget.thoNguyen.Value.ToString();
        textTuoiTac.text = characterStatsTarget.tuoiTac.Value.ToString();
        textSinhMenh.text = characterStatsTarget.hpMax.Value.ToString();
        textLevel.text = GameManager.levelWithText[characterStatsTarget.level];
        textHaoCam.text = Client.Instance.CharacterData.RelationShip.GetValueOrDefault(characterStatsTarget.idChar).ToString();
        List<ThanThong> listThanThongData = new List<ThanThong>();
        var gameConfig = Pool.Get<GameManager>();
        foreach (var item in characterStatsTarget.thanThongEquip)
        {
            listThanThongData.Add(gameConfig.GetThanThongWithId(item.Value));
        }
        for (int i = 0; i < listThanThongItemSlot.Count; i++)
        {
            var item = listThanThongItemSlot[i];
            var data = i<listThanThongData.Count?listThanThongData[i]:null;

            Pool.Get<UIManager>().InitVisualItemSkill(item,data);
        }
        Pool.Get<UIManager>().InitVisualItemSkill(listCongPhapItemSlot[0],gameConfig.GetCongPhapWithId(characterStatsTarget.chuTu1Used.Value));
        Pool.Get<UIManager>().InitVisualItemSkill(listCongPhapItemSlot[1], gameConfig.GetCongPhapWithId(characterStatsTarget.chuTu2Used.Value));
        Pool.Get<UIManager>().InitVisualItemSkill(listCongPhapItemSlot[2], gameConfig.GetCongPhapWithId(characterStatsTarget.phuTu1Used.Value));
        Pool.Get<UIManager>().InitVisualItemSkill(listCongPhapItemSlot[3], gameConfig.GetCongPhapWithId(characterStatsTarget.phuTu2Used.Value));
        Pool.Get<UIManager>().InitVisualItemSkill(listCongPhapItemSlot[4], gameConfig.GetCongPhapWithId(characterStatsTarget.phuTu3Used.Value));
        Pool.Get<UIManager>().InitVisualItemSkill(listCongPhapItemSlot[5], gameConfig.GetCongPhapWithId(characterStatsTarget.phuTu4Used.Value));
    }
    // Helper method to keep your dictionary clean
    private static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString("#" + hex, out Color color))
        {
            return color;
        }
        return Color.white; // Fallback
    }

    public void SuccessTangQuaNpc()
    {
        throw new NotImplementedException();
    }

    public void FailTangQuaNpc(string message)
    {
        throw new NotImplementedException();
    }

    public void SuccessThinhGiaoNpc()
    {
        throw new NotImplementedException();
    }

    public void FailThinhGiaoNpc(string message)
    {
        throw new NotImplementedException();
    }
}
