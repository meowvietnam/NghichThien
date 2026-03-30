using CreateCharacter;
using DG.Tweening;
using LongDH.LongDHModules;
using Network;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum ERelationShip
{
    None,
    Quen,
    BanBe,
    ThanThiet,
}
public class UINPCJiaoHu : PanelGroup
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
    [SerializeField] private Sprite[] spriteQualityFulls;
    [SerializeField] private Sprite[] spriteFrames;
    [SerializeField] private Color[] colorQualitys;
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
    public void ShowNPCList(List<CharacterStatsSecure> listCharStats)
    {
        OpenOrCloseNpcList(true);
        ResetVisual(prefabNPCLeftItem.transform);
        SetEventButton(closeOrOpenListNPC, () =>
        {
            EventBtnOpenOrCloseNPCList();
        });
        foreach (var item in listCharStats)
        {
            var itemNpc = GetItemPool(prefabNPCLeftItem);
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
        float openX = 240;
        float closeX = 0;
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
            npcPop.Hide();
        });
        SetEventButton(btnXemXet, () =>
        {
            xemXetPop.Show();
            ShowPopDoXet();
        });
        SetEventButton(btnTangQua, () =>
        {
            tangQuaPop.Show();

        });
        SetEventButton(btnThinhGiao, () =>
        {
            thinhGiaoPop.Show();
            InitPopThinhGiao();

        });
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
    private void CategorizeSlotAndInitVisual(List<BaseSkill> listBaseSkillData)
    {
        var categorize = CategorizeSlot(listBaseSkillData);
        foreach (var item in categorize.Item1)
        {
            var itemInventory = GetItemPool(prefabSlotThinhGiaoSlot1);
            InitVisualItemSkill(itemInventory, item);
        }
        foreach (var item in categorize.Item2)
        {
            var itemInventory = GetItemPool(prefabSlotThinhGiaoSlot2);
            InitVisualItemSkill(itemInventory, item);
        }
        foreach (var item in categorize.Item3)
        {
            var itemInventory = GetItemPool(prefabSlotThinhGiaoSlot3);
            InitVisualItemSkill(itemInventory, item);
        }
    }    
    private void InitCongPhapTab()
    {
        ResetVisual(prefabSlotThinhGiaoSlot1.transform);
        ResetVisual(prefabSlotThinhGiaoSlot2.transform);
        ResetVisual(prefabSlotThinhGiaoSlot3.transform);
        var GameConfig = Pool.Get<GameConfig>();

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
        CategorizeSlotAndInitVisual(listCongPhapData.Cast<BaseSkill>().ToList());


        SetSliderVisual(ETypeSkillRelationShip.CongPhap);

    }
    private T GetItemPool<T>(T prefabItem) where T : Component
    {
        // Lấy Transform của cha để duyệt qua các con
        Transform parent = prefabItem.transform.parent;
        T item = null;
        // Duyệt từ index 1 (bỏ qua index 0 thường là chính cái prefab/template)
        for (int i = 1; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            // Nếu object con đang không hoạt động (đang rảnh trong pool)
            if (!child.gameObject.activeSelf)
            {
                item = child.GetComponent<T>();
                item.gameObject.SetActive(true);
                return item;
            }
        }
        item = Instantiate(prefabItem, parent);
        item.gameObject.SetActive(true);
        // Nếu không tìm thấy cái nào rảnh, khởi tạo cái mới
        return item;
    }
    private void InitThanThongTab()
    {
        ResetVisual(prefabSlotThinhGiaoSlot1.transform);
        ResetVisual(prefabSlotThinhGiaoSlot2.transform);
        ResetVisual(prefabSlotThinhGiaoSlot3.transform);
        this.currentRelationShip = GetERelationShip(relatetionshipValue, ETypeSkillRelationShip.ThanThong);
        btnTabThanThong.image.sprite = spriteSelectTab;
        btnTabCongPhap.image.sprite = spriteNoSelectTab;
        List<ThanThong> listThanThongData = new List<ThanThong>();
        var GameConfig = Pool.Get<GameConfig>();

        foreach (var item in characterStatsTarget.thanThongEquip)
        {
            listThanThongData.Add(GameConfig.GetThanThongWithId(item.Value));
        }
        CategorizeSlotAndInitVisual(listThanThongData.Cast<BaseSkill>().ToList());
        SetSliderVisual(ETypeSkillRelationShip.ThanThong);

    }
    private void ResetVisual(Transform prefab)
    {
        for (int i = 1; i < prefab.transform.parent.childCount; i++)
        {
            prefab.transform.parent.GetChild(i).gameObject.SetActive(false);
        }
        
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
    private void InitVisualItemSkill(ItemInventory item, BaseSkill data)
    {
        Sprite frame = null;
        if (data != null)
        {
            frame = spriteFrames[((((int)data.eTierSkill - 1) / 3) * 2) + 1];

            item.SetColorName(colorQualitys[((((int)data.eTierSkill - 1) / 3) * 2) + 1]);
            item.SetQualityFull(spriteQualityFulls[((((int)data.eTierSkill - 1) / 3) * 2) + 1]);

        }

        item.Init(data, frame);

        item.InitInventory(ETypeInventoryItem.Skill);


    }
    public void ShowNpcPop(CharacterStatsSecure charStats)
    {
        this.characterStatsTarget = charStats;
        npcPop.Show();
        textName.text = characterStatsTarget.nameUser;
        charVisualPopJiaoHua.SetGender(characterStatsTarget.Gender);
        charVisualPopJiaoHua.SetSkin(characterStatsTarget.Skins);


    }
    public void ShowPopDoXet()
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
        textLevel.text = GameConfig.levelWithText[ThanThong.GetDaiCanhGioi(characterStatsTarget.level)];
        textHaoCam.text = Client.Instance.CharacterData.RelationShip.GetValueOrDefault(characterStatsTarget.idChar).ToString();
        List<ThanThong> listThanThongData = new List<ThanThong>();
        var gameConfig = Pool.Get<GameConfig>();
        foreach (var item in characterStatsTarget.thanThongEquip)
        {
            listThanThongData.Add(gameConfig.GetThanThongWithId(item.Value));
        }
        for (int i = 0; i < listThanThongItemSlot.Count; i++)
        {
            var item = listThanThongItemSlot[i];
            var data = i<listThanThongData.Count?listThanThongData[i]:null;

            InitVisualItemSkill(item,data);
        }
        InitVisualItemSkill(listCongPhapItemSlot[0],gameConfig.GetCongPhapWithId(characterStatsTarget.chuTu1Used.Value));
        InitVisualItemSkill(listCongPhapItemSlot[1], gameConfig.GetCongPhapWithId(characterStatsTarget.chuTu2Used.Value));
        InitVisualItemSkill(listCongPhapItemSlot[2], gameConfig.GetCongPhapWithId(characterStatsTarget.phuTu1Used.Value));
        InitVisualItemSkill(listCongPhapItemSlot[3], gameConfig.GetCongPhapWithId(characterStatsTarget.phuTu2Used.Value));
        InitVisualItemSkill(listCongPhapItemSlot[4], gameConfig.GetCongPhapWithId(characterStatsTarget.phuTu3Used.Value));
        InitVisualItemSkill(listCongPhapItemSlot[5], gameConfig.GetCongPhapWithId(characterStatsTarget.phuTu4Used.Value));
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
}
