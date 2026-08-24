using AllMaps.ItemProvider;
using Base;
using ComboSkillResource.NPCData;
using CreateCharacter;
using DG.Tweening;
using GoogleMobileAds.Api;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AllMaps.EventReward;
using UnityEngine;
using UnityEngine.UI;
using CongPhap = Skill.CongPhap;
using ThanThong = Skill.ThanThong;

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
    [SerializeField] public PopupInfor xemXetPop;
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

    private int relatetionshipValue => Client.Instance.CharacterData.Social.GetRelationshipWithNPC(characterStatsTarget.idChar);
  
    [Header("--- Pop Tặng Quà ---")]
    [SerializeField] private ItemInventory slotItemGift;
    [SerializeField] private Text textMoney;
    [SerializeField] private Text textNameUser;
    [SerializeField] private Text textHaoCamPopTangQua;
    [SerializeField] private CharacterGraphic visualChar;
  
    [SerializeField] private Button btnConfirmTangQua;
    [SerializeField]
    private Bag1ItemPrefabInventory bag;
    private InventoryUIFacade inventoryUIFacade;

    public InventoryUIFacade InventoryUIFacade => inventoryUIFacade ?? (inventoryUIFacade = new InventoryUIFacade());


    private void ResetItemBag()
     {
        var oldItem = bag.InventoryUIFacade.listItemUI.FirstOrDefault(x => x.itemRef.Id == slotItemGift.itemRef.Id);
        if(oldItem != null)
        {
            oldItem.gameObject.SetActive(true);
            oldItem.InitCount(oldItem.itemRef.Count);
        }
        this.InventoryUIFacade.RenderBagItem(slotItemGift,null,false);
     }    
     public void InitPopTangQua()
     {
        slotItemGift.Init(null, null);

        var characterData = Client.Instance.CharacterData;
        var gameConfig = Pool.Get<GameManager>();
        List<ItemRef> itemDatas = new List<ItemRef>();
        itemDatas.AddRange(characterData.InventoryManager.Herbs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.Recipes.ToList());
        itemDatas.AddRange(characterData.InventoryManager.ThanThongs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.CongPhaps.ToList());
        // itemDatas.AddRange(characterData.InventoryManager.VuKhis.ToList());
        // itemDatas.AddRange(characterData.InventoryManager.PhongCus.ToList());
        // itemDatas.AddRange(characterData.InventoryManager.TrangSucs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.LinhDans.ToList());
        // Sử dụng LINQ để trích xuất Ref từ mỗi phần tử trong furnaceDatas
        var furnaceRefs = characterData.InventoryManager.Furnaces.Select(f => f.CreateRefSelfID()).ToList();

        // Thêm danh sách Ref vừa lấy được vào itemDatas
        itemDatas.AddRange(furnaceRefs);
        itemDatas.AddRange(characterData.InventoryManager.VuKhis.Select(f => f.CreateRefSelfId()).ToList());
        itemDatas.AddRange(characterData.InventoryManager.PhongCus.Select(f => f.CreateRefSelfId()).ToList());
        itemDatas.AddRange(characterData.InventoryManager.TrangSucs.Select(f => f.CreateRefSelfId()).ToList());
        bag.filterControl.InitAllFilterType();
        bag.InitDataItem(itemDatas, (ItemInventory) =>
        {
            ItemInventory.InitAction(null, null, () =>
            {
                if (slotItemGift.itemData == null || (slotItemGift.itemRef != null && slotItemGift.itemRef.Id != ItemInventory.itemRef.Id))
                {
                    if (slotItemGift.itemRef != null && slotItemGift.itemRef.Id != ItemInventory.itemRef.Id)
                    {
                        ResetItemBag();
                    }
                    ItemTypeHandlerBagFactory.Create(ItemInventory.itemRef.Type).Handle(slotItemGift, ItemInventory.itemRef, false);
                    slotItemGift.InitCount(1);
                    ItemInventory.AddCount(-1);

                }

                else
                {
                    if (ItemInventory.count > 0)
                    {
                        slotItemGift.AddCount(1);
                        ItemInventory.AddCount(-1);
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
        }, false, true);
        slotItemGift.InitAction(null, null, () =>
        {
            ResetItemBag();
        });
        bag.InitBag(false);
        //reset 
        visualChar.SetGender(characterData.Gender);
        visualChar.SetSkin(characterData.Appearance.Skins, characterData.Appearance.Colors);
        textMoney.text = characterData.Stats.LinhThach.ToString();
        textNameUser.text = characterData.Name;
        SetEventButton(btnConfirmTangQua, () =>
        {
            var myId = characterStatsTarget.idChar.Value;
            var npcDatas = Pool.Get<NPCManager>().npcDatas;
            if (npcDatas.All(x => x.charStats.idChar != myId))
            {
                NotifyGroup.Instance.Show("NPC không hợp lệ!");
                return;
            }
            var item =  slotItemGift.itemRef;
            if(item == null) return;
            var value = ItemProviderFactory.GetProvider(item.Type).ValueTangQuaNPCWithItem(item.Id, characterStatsTarget.level);
            var xungHo = GameManager.XungHoText(characterStatsTarget.level,characterStatsTarget.Gender);
            if(value.Item1 <= 0)
            {
                SayDialogManager.Instance.ShowDialogWithSkin(characterStatsTarget.nameUser,characterStatsTarget.Gender,characterStatsTarget.Skins,characterStatsTarget.Colors, new List<string> { $"{xungHo.targetCallMe}, ta không cần thứ này!" });
            }
            else
            {
                if (slotItemGift.count > 0)
                {
                    SayDialogManager.Instance.ShowDialogWithSkin(characterStatsTarget.nameUser, characterStatsTarget.Gender, characterStatsTarget.Skins, characterStatsTarget.Colors, new List<string> { $"{xungHo.targetCallMe}, sảng khoái" });
                    item.Count = (ushort)slotItemGift.count;
                    int valueAdd = value.Item1 * item.Count * value.Item2 * 2;
                    Debug.Log($"Value: [{value.Item1},{value.Item2}], ValueAdd: {valueAdd}, Item = [{item}]");
                    NotifyGroup.Instance.Show($"Mối quan hệ giữa bạn và {characterStatsTarget.nameUser} tăng thêm {valueAdd}");

                    Observer.Notify(EventList.Network.TANG_QUA_NPC, this, myId, slotItemGift.itemRef);
                    var InventoryManager = Client.Instance.CharacterData.InventoryManager;
                    if (slotItemGift.itemRef.Type == ItemType.Furnace)
                    {
                        InventoryManager.RemoveFurnace((ushort)slotItemGift.itemRef.Id);

                    }
                    else if (slotItemGift.itemRef.Type == ItemType.VuKhi || slotItemGift.itemRef.Type == ItemType.PhongCu || slotItemGift.itemRef.Type == ItemType.TrangSuc)
                    {
                        InventoryManager.RemoveEquipment((ushort)slotItemGift.itemRef.Id);
                    }
                    else
                    {
                        InventoryManager.Add(slotItemGift.itemRef, -slotItemGift.itemRef.Count);
                    }
                    this.InventoryUIFacade.RenderBagItem(slotItemGift, null, false);
                }
            }
           
        });
     }
    public void OnReloadNpcThisNode()
    {
        List<CharacterStatsSecure> listCharStats = new List<CharacterStatsSecure>();
        Transform uiParent = prefabNPCLeftItem.transform.parent;

        // 1. Thu thập tất cả Id của NPC hiện đang có trên UI (tránh trùng lặp khi reload)
        HashSet<int> existingNpcIds = new HashSet<int>();
        for (int i = 0; i < uiParent.childCount; i++)
        {
            var itemComponent = uiParent.GetChild(i).GetComponent<NPCLeftSVItem>();
            if (itemComponent != null && itemComponent.characterStats != null)
            {
                existingNpcIds.Add(itemComponent.characterStats.idChar);
                itemComponent.Init(itemComponent.characterStats);
            }
        }

        var currentNode = Client.Instance.CharacterData.Progression.IdCurrentNode;
        // 2. Lọc danh sách NPC thuộc Node hiện tại VÀ chưa xuất hiện trên UI (chưa có Id trong existingNpcIds)
        List<NPCData> npcOnCurrentMap = Pool.Get<NPCManager>().npcDatas
            .Where(x => x.charStats.idNode == currentNode
                     && !existingNpcIds.Contains(x.charStats.idChar)) // Điều kiện check chưa tồn tại ở đây
            .ToList();

        // 3. Nạp stats của các NPC mới vào list xử lý
        foreach (var character in npcOnCurrentMap)
        {
            listCharStats.Add(character.charStats);
        }

        // 4. Sinh UI Item mới cho các NPC mới được thêm
        foreach (var item in listCharStats)
        {
            var itemNpc = GameManager.GetItemPoolWithPrefabInParent(prefabNPCLeftItem);
            itemNpc.Init(item);
        }

        // 5. Cập nhật số lượng hiển thị (Trừ đi 1 nếu prefab gốc của bạn đang ẩn nằm trong parent này)
        countCharText.text = $"{uiParent.childCount - 1}";
    }
    public void ShowNPCList(List<CharacterStatsSecure> listCharStats)
    {
        npcPop.Hide();
        thinhGiaoPop.Hide();
        xemXetPop.Hide();
        tangQuaPop.Hide();
        xemXetPop.InitActionClose(() =>
        {
            Pool.Get<GameManager>().Cleanup();

        });
        tangQuaPop.InitActionClose(() =>
        {
            Pool.Get<GameManager>().Cleanup();

        });
        thinhGiaoPop.InitActionClose(() =>
        {
            Pool.Get<GameManager>().Cleanup();

        });
        OpenOrCloseNpcList(true);
        ResetVisual(prefabNPCLeftItem.transform);
        SetEventButton(closeOrOpenListNPC, () =>
        {
            EventBtnOpenOrCloseNPCList();
        });
        foreach (var item in listCharStats)
        {
            var itemNpc = GameManager.GetItemPoolWithPrefabInParent(prefabNPCLeftItem);
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
            xemXetPop.InitPopDoXet(characterStatsTarget);
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
            Pool.Get<GameManager>().GoToPrepareCombat(new GameManager.CharCombatInit
            {
                charStats = characterStatsTarget,
                eTypeVisual = ETypeVisualChar.Character,
            },ETypeCombat.LuanBan);

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
        float amount = relatetionshipValue / (float)(eType == ETypeSkillRelationShip.CongPhap
            ? relationShipRequireCongPhap.Last().Value
            : relationShipRequireThanThong.Last().Value);
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
    private (List<int>, List<int>, List<int>) CategorizeSlotWithId(List<BaseSkill> listThanThongData)
    {
        // Giả sử listThanThongData là danh sách các đối tượng có thuộc tính eTierSkill

        List<int> list1 = new List<int>();
        List<int> list2 = new List<int>();
        List<int> list3 = new List<int>();
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
                if (tier >= BaseSkill.ETierSkill.ThienGiaiHa) list3.Add(item.Id);
                else if (tier >= BaseSkill.ETierSkill.DiaGiaiHa) list2.Add(item.Id);
                else if (tier >= BaseSkill.ETierSkill.NhanGiaiHa) list1.Add(item.Id);
            }
            // TRƯỜNG HỢP 2: Cao nhất là Địa Giai
            else if (maxTier >= BaseSkill.ETierSkill.DiaGiaiHa)
            {
                if (tier >= BaseSkill.ETierSkill.DiaGiaiTrung) list3.Add(item.Id); // Địa Thượng + Trung
                else if (tier == BaseSkill.ETierSkill.DiaGiaiHa) list2.Add(item.Id); // Địa Hạ
                else if (tier >= BaseSkill.ETierSkill.NhanGiaiHa) list1.Add(item.Id); // Nhân Giai
            }
            // TRƯỜNG HỢP 3: Cao nhất chỉ có Nhân Giai
            else if (maxTier >= BaseSkill.ETierSkill.NhanGiaiHa)
            {
                if (tier == BaseSkill.ETierSkill.NhanGiaiThuong) list3.Add(item.Id);
                else if (tier == BaseSkill.ETierSkill.NhanGiaiTrung) list2.Add(item.Id);
                else if (tier == BaseSkill.ETierSkill.NhanGiaiHa) list1.Add(item.Id);
            }
        }

        return (list1, list2, list3);
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
            var itemInventory = GameManager.GetItemPoolWithPrefabInParent(prefabSlotThinhGiaoSlot1);
            itemInventory.InitAction(null, null, null);

            Pool.Get<UIManager>().InitVisualItemSkill(itemInventory, item);
            if((int)eTypeRelationShip  >= (int)ERelationShip.Quen)
            {
                CallActionThinhGiao(itemInventory);

            }
        }
        foreach (var item in categorize.Item2)
        {
            var itemInventory = GameManager.GetItemPoolWithPrefabInParent(prefabSlotThinhGiaoSlot2);
            itemInventory.InitAction(null, null, null);

            Pool.Get<UIManager>().InitVisualItemSkill(itemInventory, item);
            if ((int)eTypeRelationShip >= (int)ERelationShip.BanBe)
            {
                CallActionThinhGiao(itemInventory);

            }
        }
        foreach (var item in categorize.Item3)
        {
            var itemInventory = GameManager.GetItemPoolWithPrefabInParent(prefabSlotThinhGiaoSlot3);
            itemInventory.InitAction(null, null,null);

            Pool.Get<UIManager>().InitVisualItemSkill(itemInventory, item);
            if ((int)eTypeRelationShip >= (int)ERelationShip.ThanThiet)
            {
                CallActionThinhGiao(itemInventory);

            }
        }
        void CallActionThinhGiao(ItemInventory item)
        {
            item.InitAction(null, null, () =>
            {
                var categorizeCurrent = categorize;
                ERelationShip indexThinhGiao = ERelationShip.Quen;
                if(categorizeCurrent.Item1.FirstOrDefault(x=>x.Id == item.itemRef.Id) != null )
                {
                    indexThinhGiao = ERelationShip.Quen;
                }    
                else if (categorizeCurrent.Item2.FirstOrDefault(x => x.Id == item.itemRef.Id) != null)
                {
                    indexThinhGiao = ERelationShip.BanBe;

                }
                else if (categorizeCurrent.Item3.FirstOrDefault(x => x.Id == item.itemRef.Id) != null)
                {
                    indexThinhGiao = ERelationShip.ThanThiet;

                }
                int require = CurrentPageTypeRelationShip == ETypeSkillRelationShip.ThanThong? relationShipRequireThanThong[indexThinhGiao] : relationShipRequireCongPhap[indexThinhGiao];
                if(relatetionshipValue >= require)
                {
                    Pool.Get<PopupConfirmAction>().ShowPopConfirmAction($"Xác nhận tiêu hao {require} điểm hảo cảm thỉnh giáo {item.itemData.nameItem}", () =>
                    {
                        Observer.Notify(EventList.Network.THINH_GIAO_NPC,this,characterStatsTarget.idChar.Value,item.itemRef.Id,item.itemRef.Type);
                        Client.Instance.CharacterData.Social.RelationshipAddValue((ushort)characterStatsTarget.idChar,-require);
                        if (item.itemRef.Type == ItemType.ThanThong)
                        {
                            InitThanThongTab();
                        }
                        else if (item.itemRef.Type == ItemType.CongPhap)
                        {
                            InitCongPhapTab();

                        }

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
        textRelationshipValue.text = relatetionshipValue.ToString();

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
        textRelationshipValue.text = relatetionshipValue.ToString();

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
        var relatetionshipValue = Client.Instance.CharacterData.Social.GetRelationshipWithNPC(characterStatsTarget.idChar);
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
        NotifyGroup.Instance.Show("Tặng quà thành công");

        textHaoCamPopTangQua.text = Client.Instance.CharacterData.Social.GetRelationshipWithNPC(characterStatsTarget.idChar).ToString();
        OnReloadNpcThisNode();

    }

    public void FailTangQuaNpc(string message)
    {
       NotifyGroup.Instance.Show(message);
    }

    public void SuccessThinhGiaoNpc(ItemRef itemRef)
    {
        NotifyGroup.Instance.Show("Thỉnh giáo thành công vui lòng kiểm tra kho đồ");
        Pool.Get<EventRewardPanel>().InitShowEventReward(new AllMaps.EventReward.RewardSnapshot { ItemRefs = new List<ItemRef> { itemRef }, Money = 0 , MoneyVip = 0 },"Chăm chỉ ắt thành tài!");
        OnReloadNpcThisNode();
    }

    public void FailThinhGiaoNpc(string message)
    {
        NotifyGroup.Instance.Show(message);
    }
}
