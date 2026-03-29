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
        { ERelationShip.None, 0 }, // Use { key, value }
        { ERelationShip.Quen, 100 },
        { ERelationShip.BanBe, 500 },
        { ERelationShip.ThanThiet, 1000 },

    };
    public static Dictionary<ERelationShip, int> relationShipRequireCongPhap = new Dictionary<ERelationShip, int>
    {
        { ERelationShip.None, 0 }, // Use { key, value }
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
    [Header("--- Pop JiaoHuo ---")]

    [SerializeField] private CharacterStatsSecure characterStatsTarget;
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
    private int relatetionshipValue => Client.Instance.CharacterData.RelationShip[characterStatsTarget.idChar];

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
        
        if (!isWantOpen)
        {
            // close
            closeOrOpenListNPC.transform.parent.transform.DOMoveX(closeX,0.5f);
            closeOrOpenListNPC.transform.localScale = new Vector3(openScaleBtn * -1,1,1);
        }    
        else
        {
            closeOrOpenListNPC.transform.parent.transform.DOMoveX(openX, 0.5f);
            closeOrOpenListNPC.transform.localScale = new Vector3(openScaleBtn, 1, 1);


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
        });
        SetEventButton(btnTangQua, () =>
        {
            tangQuaPop.Show();

        });
        SetEventButton(btnThinhGiao, () =>
        {
            thinhGiaoPop.Show();

        });
    }
    private void SetSliderVisual()
    {
        float amount = relatetionshipValue / relationShipRequireCongPhap[currentRelationShip];
        float final = amount + amount * 0.25f;
        sliderRelationship.fillAmount = final;
       
        switch (currentRelationShip)
        {
            case ERelationShip.None:
                relateText1.color = colorNoneRelation;
                relateText2.color = colorNoneRelation;
                relateText3.color = colorNoneRelation;

                // Set lock
                prefabSlotThinhGiaoSlot1.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
                prefabSlotThinhGiaoSlot2.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
                prefabSlotThinhGiaoSlot3.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
                break;
            case ERelationShip.Quen:
                relateText1.color = Color.white;
                relateText2.color = colorNoneRelation;
                relateText3.color = colorNoneRelation;
                //Set lock
                prefabSlotThinhGiaoSlot1.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);

                break;
            case ERelationShip.BanBe:
                relateText1.color = Color.white;
                relateText2.color = Color.white;
                relateText3.color = colorNoneRelation;
                // set lock
                prefabSlotThinhGiaoSlot1.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
                prefabSlotThinhGiaoSlot2.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);

                break;
            case ERelationShip.ThanThiet:
                relateText1.color = Color.white;
                relateText2.color = Color.white;
                relateText3.color = Color.white;
                //Set lock
                prefabSlotThinhGiaoSlot1.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
                prefabSlotThinhGiaoSlot2.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
                prefabSlotThinhGiaoSlot3.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
                break;
            default:
                break;
        }

    }
  
    private (List<BaseSkill>,List<BaseSkill>,List<BaseSkill>) CategorizeSlot(List<BaseSkill> listThanThongData)
    {
        // Giả sử listThanThongData là danh sách các đối tượng có thuộc tính eTierSkill
        var maxTier = listThanThongData.Max(x => x.eTierSkill);

        List<BaseSkill> list1 = new List<BaseSkill>();
        List<BaseSkill> list2 = new List<BaseSkill>();
        List<BaseSkill> list3 = new List<BaseSkill>();

        foreach (var item in listThanThongData)
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
    private void InitCongPhapTab()
    {
        ResetVisual(prefabSlotThinhGiaoSlot1.transform);
        ResetVisual(prefabSlotThinhGiaoSlot2.transform);
        ResetVisual(prefabSlotThinhGiaoSlot3.transform);

        this.currentRelationShip = GetERelationShip(relatetionshipValue, ETypeSkillRelationShip.CongPhap);
        btnTabThanThong.image.sprite = spriteNoSelectTab;
        btnTabCongPhap.image.sprite = spriteSelectTab;
        foreach (var item in characterStatsTarget.thanThongEquip)
        {

        }
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
        var categorize = CategorizeSlot(listThanThongData.Cast<BaseSkill>().ToList());
        foreach (var item in categorize.Item1)
        {
            var itemInventory = GetItemPool(prefabSlotThinhGiaoSlot1);
            InitVisualItemSkill(itemInventory,item);
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
    private void ResetVisual(Transform prefab)
    {
        for (int i = 1; i < prefab.transform.parent.childCount; i++)
        {
            prefab.transform.parent.GetChild(i).gameObject.SetActive(false);
        }
        
    }    
    private void InitPopThinhGiao()
    {
       
        int relatetionshipValue = Client.Instance.CharacterData.RelationShip[characterStatsTarget.idChar];
        textRelationshipValue.text = relatetionshipValue.ToString();
        InitThanThongTab();
        SetEventButton(btnTabCongPhap, () =>
        {
            InitCongPhapTab();
        });
        SetEventButton(btnTabThanThong, () =>
        {
            InitThanThongTab();
        });
        SetSliderVisual();




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
