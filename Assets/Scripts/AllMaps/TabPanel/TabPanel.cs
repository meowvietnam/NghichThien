using AllMaps;
using AllMaps.ItemProvider;
using Base;
using CreateCharacter;
using CreateCharacter.CharStats;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using LuyenKhi;
using Network;
using Newtonsoft.Json.Linq;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using CongPhap = Skill.CongPhap;
using ThanThong = Skill.ThanThong;


public class TabPanel : PanelGroup , INetworkEvent.IEquipCongPhap , INetworkEvent.IUnEquipCongPhap,INetworkEvent.IEquipThanThong, INetworkEvent.IUnEquipThanThong , INetworkEvent.IUseDan , INetworkEvent.IEquipEquipment , INetworkEvent.IUnEquipEquipment
{
    public enum ETypePanel { 
        
        None,
        ThuocTinh,
        NgoDao,
        CongPhap,
        ThanThong,
        VatPham,
        HeThong,
    
    
    }

    [System.Serializable]
    public class ButtonWithPanel
    {
        [SerializeField] public Button btn;
        [SerializeField] public GameObject panel;
        [SerializeField] public ETypePanel typePanel;


    }

    [SerializeField] private ButtonWithPanel[] btns;
    [SerializeField] private Button btnClose;
    [SerializeField] private ButtonWithPanel currentBtnWithPanel;
    [SerializeField] public ToolTipItems toolTipItems => ToolTipItems.Instance;
    [ContextMenu(nameof(Get))]
    public void Get()
    {
        var q = CoreConfig.Instance.Qualities;
        spriteQualityFulls = q.Select(x => x.Background).ToArray();
        spriteFrames = q.Select(x => x.IconUp).ToArray();
        colorQualitys = q.Select(x => x.Color).ToArray();
        EditorUtility.SetDirty(this);
    }
    void SelectFilter()
    {
        lianziAnim.SetTrigger("Hide");

    }
    void DoneFilter()
    {
        lianziAnim.SetTrigger("Show");

    }
    public override void Init()
    {
        base.Init();
     
        foreach (var item in btns)
        {
            SetEventButton(item.btn, () =>
            {
                StartCoroutine(DelayFrame());

                if (currentBtnWithPanel.typePanel == item.typePanel)
                {
                    return;
                }
                currentBtnWithPanel = item;
                bag.gameObject.SetActive(false);

                foreach (var item2 in btns)
                {
                    item2.btn.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    item2.panel.SetActive(false);
                }
                item.btn.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                item.panel.SetActive(true);
                if(item.typePanel == ETypePanel.CongPhap)
                {
                    InitCongPhap();
                

                }
                else if (item.typePanel == ETypePanel.ThanThong)
                {
                    InitThanThong();
                  
                }
                else if (item.typePanel == ETypePanel.VatPham)
                {
                  
                    InitInventory();
                   
                }
                else if (item.typePanel == ETypePanel.NgoDao)
                {

                    InitNgoDao();


                }
                

            });
        }
        Observer.AddObserver(EventList.GamePlay.UPDATE_PLAYER_STATS, UpdatePlayerStats);
        SetEventButton(btnClose, () =>
        {
            gameObject.SetActive(false);
        });
        InitOnlyNgoDao();
    }
    private void OnEnable()
    {
        btns[0]?.btn?.onClick.Invoke();
        if(dictBtnWithNgoDaos?.Count>0)
        {
            dictBtnWithNgoDaos?.ElementAt(0).Key?.onClick.Invoke();
        }    
    }
    IEnumerator DelayFrame()
    {
        yield return null;
        yield return null;

        Observer.AddObserver(FilterControl.SELECT_FILTER, SelectFilter);
        Observer.AddObserver(FilterControl.DONE_FILTER, DoneFilter);
    }

    [Header("--- THUỘC TÍNH ---")]
    [SerializeField] private Image iconLevel;
    [SerializeField] private Text hpText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text tuviText;
    [SerializeField] private Text nameText;
    [SerializeField] private Sprite[] listSpriteIconLevel;
    [SerializeField] private Text tuoitacText;
    [SerializeField] private Text thonguyenText;
    [SerializeField] private Text tuChatText;
    [SerializeField] private Text thanThucText;
    [SerializeField] private Text ngoTinhText;
    [SerializeField] private Text donTocText;
    [SerializeField] private Text tocDoTuLuyenText;
    [SerializeField] private Text tamCanhText;
    [SerializeField] private Text tamCanhValue;
    [SerializeField] private Image tamCanhIcon;

    [SerializeField] private Text danDocText;
    [SerializeField] private Text danDocValue;
    [SerializeField] private Image danDocIcon;

    [SerializeField] private Text kimText;
    [SerializeField] private Text mocText;
    [SerializeField] private Text thuyText;
    [SerializeField] private Text hoaText;
    [SerializeField] private Text thoText;
    [SerializeField] private Sprite[] listSpriteIconTamCanh;
    [SerializeField] private Color[] listColorTamCanh;

    [SerializeField] private Sprite[] listSpriteIconDanDoc;
    [SerializeField] private Color[] listColorDanDoc;
    [SerializeField] public Tooltip toolTipBtnStats => Tooltip.Instance;
    private CharacterData CharData => Client.Instance.CharacterData;
    private void UpdatePlayerStats()
    {
        var CharacterDatas = CharData;

        var CharacterStats = CharacterDatas.Stats;
        var gameConfig = Pool.Get<GameManager>();
        hpText.text = $"{CharacterStats.Hp}/{CharacterStats.SinhMenh}";

        tuviText.text = $"{CharacterDatas.Progression.CurrentExp}/{gameConfig.inforLevels[CharacterDatas.Progression.Level - 1].tuviRequireNextLevel}";
        iconLevel.sprite = listSpriteIconLevel[CharacterDatas.Progression.Level - 1];
        iconLevel.SetNativeSize();
        nameText.text = CharacterDatas.Name;

        levelText.text = GameManager.levelWithText[CharacterDatas.Progression.Level];
        tuoitacText.text = GameManager.GetIntTuoiTac(CharacterDatas.Stats.TuoiTac).ToString();
        thonguyenText.text = CharacterDatas.Stats.ThoNguyen.ToString();
        tuChatText.text = CharacterDatas.Stats.TuChat.ToString();
        thanThucText.text = CharacterDatas.Stats.ThanThuc.ToString();
        ngoTinhText.text = CharacterDatas.Stats.NgoTinh.ToString();
        donTocText.text = CharacterDatas.Stats.TocDoBay.ToString();
        //tocDoTuLuyenText.text = player.characterStatsBase.tocDoBay.Value.ToString();
        var getTamCanh = GameManager.GetTamCanhValue(CharacterDatas.Stats.TamCanh);
        var getDanDoc = GameManager.GetDanDocValue(CharacterDatas.Stats.DanDoc);

        tamCanhText.text = getTamCanh.name;
        tamCanhText.color = listColorTamCanh[getTamCanh.index];
        tamCanhValue.text = $"{CharacterDatas.Stats.TamCanh}/{getTamCanh.nextMilestone}";
        tamCanhIcon.sprite = listSpriteIconTamCanh[getTamCanh.index];

        danDocText.text = getDanDoc.text;
        danDocText.color = listColorDanDoc[getDanDoc.index];
        danDocValue.text = $"{CharacterDatas.Stats.DanDoc}/{getDanDoc.nextMilestone}";
        danDocIcon.sprite = listSpriteIconDanDoc[getDanDoc.index];

        kimText.text = $"{(int)CharacterDatas.Stats.Kim}";
        mocText.text = $"{(int)CharacterDatas.Stats.Moc}";
        thuyText.text = $"{(int)CharacterDatas.Stats.Thuy}";
        hoaText.text = $"{(int)CharacterDatas.Stats.Hoa}";
        thoText.text = $"{(int)CharacterDatas.Stats.Tho}";

    }
    [Header("--- NGỘ ĐẠO ---")]
    [SerializeField] private ScrollRect ngoDaoSelect;
    [SerializeField] private Button ngoDaoBtnSelectPrefab;
    [SerializeField] private NgoDaoResource[] listNgoDaoResources;
    [SerializeField] private Transform ngoDaoSkillParent => ngoDaoSlotParents[0].transform.parent;
    [SerializeField] private RectTransform[] ngoDaoSlotParents = new RectTransform[4];
    [SerializeField] public Dictionary<Button,List<RectTransform>> dictBtnWithNgoDaos = new Dictionary<Button, List<RectTransform>>();
    [SerializeField] private List<Sprite> listSpriteCloudNgoDao;
    [SerializeField] private List<Transform> levelNgoDao = new List<Transform>(5);
    [SerializeField] private Image camNgoSlider;
    [SerializeField] private Text camNgoText;
    [SerializeField] private Text ngoDaoDiemText;
    [SerializeField]
    private NgoDaoToolTip ngoDaoToolTip;
    [SerializeField]
    public SecureInt indexThisPage;
    public void InitOnlyNgoDao()
    {
        // INit ngộ đạo skill
        Vector2 posStart = new Vector2(-500, 362);
        int perX = 260;
        foreach (var resource in listNgoDaoResources)
        {
            var ngoDaoBtnSelect = Instantiate(ngoDaoBtnSelectPrefab, ngoDaoSelect.content);
            var activeImg = ngoDaoBtnSelect.transform.GetChild(0).transform.GetChild(1).GetComponent<Image>();
            var unActiveImg = ngoDaoBtnSelect.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
            activeImg.sprite = resource.spriteItem;
            activeImg.SetNativeSize();
            ngoDaoBtnSelect.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().text = resource.nameItem;
            unActiveImg.sprite = resource.spriteItem;
            unActiveImg.SetNativeSize();
            ngoDaoBtnSelect.transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = resource.nameItem;
            List<RectTransform> list = new List<RectTransform>();
            for (int i = 0; i < resource.ngoDao.Count; i++)
            {
                var item = resource.ngoDao[i];
                var ngoDaoSlotParentPrefab = ngoDaoSlotParents[item.list.Count - 1];
                var ngoDaoSlotParent = Instantiate(ngoDaoSlotParentPrefab, ngoDaoSkillParent);
                ngoDaoSlotParent.gameObject.SetActive(true);
                ngoDaoSlotParent.localPosition = new Vector3(posStart.x + perX * i,ngoDaoSlotParent.localPosition.y);

                for (int j = 0; j < item.list.Count; j++)
                {
                    ngoDaoSlotParent.transform.GetChild(j).GetComponent<NgoDaoSlot>().Init(item.list[j]);
                }
                list.Add(ngoDaoSlotParent);
            }
            dictBtnWithNgoDaos.Add(ngoDaoBtnSelect,list);

            SetEventButton(ngoDaoBtnSelect, () =>
            {
                foreach (var item in dictBtnWithNgoDaos)
                {
                    item.Key.transform.GetChild(0).gameObject.SetActive(false);
                    item.Key.transform.GetChild(1).gameObject.SetActive(true);
                    foreach (var item2 in item.Value)
                    {
                        item2.gameObject.SetActive(false);
                    }

                }
                ngoDaoBtnSelect.transform.GetChild(0).gameObject.SetActive(true);
                ngoDaoBtnSelect.transform.GetChild(1).gameObject.SetActive(false);
                foreach (var slotParent in dictBtnWithNgoDaos[ngoDaoBtnSelect])
                {
                    slotParent.gameObject.SetActive(true);
                }

                int index = dictBtnWithNgoDaos.Keys.ToList().IndexOf(ngoDaoBtnSelect);
                indexThisPage = index;
                InitCamNgoLevel(listNgoDaoResources[index]);
                InitNgoDaoSlotCanLearn(dictBtnWithNgoDaos[ngoDaoBtnSelect]);
            });


        }
        
    
    }
    public void InitNgoDao()
    {
        // INit ngộ đạo level
        for (int i = 0; i < levelNgoDao.Count; i++)
        {
            var item = levelNgoDao[i];
            // name 2 // value 3
            item.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().text = NgoDaoResource.CamNgoText[i];
            item.transform.GetChild(0).transform.GetChild(3).GetComponent<Text>().text = NgoDaoResource.CamNgoRequire[i].ToString();
            item.transform.GetChild(1).transform.GetChild(2).GetComponent<Text>().text = NgoDaoResource.CamNgoText[i];
            item.transform.GetChild(1).transform.GetChild(3).GetComponent<Text>().text = NgoDaoResource.CamNgoRequire[i].ToString();
            item.transform.GetChild(0).gameObject.SetActive(false);
            item.transform.GetChild(1).gameObject.SetActive(true);


        }
        SetNgoDaoDiem();
        dictBtnWithNgoDaos.Keys.ElementAt(0).onClick?.Invoke();
    }    
    public void SetNgoDaoDiem()
    {
        ngoDaoDiemText.text = CharData.Progression.NgoDaoDiem.ToString();
    }
    public void InitNgoDaoSlotCanLearn(List<RectTransform> listSkill)
    {
        var CharacterDatas = CharData;
        // reset
        foreach (var item in listSkill)
        {
            for (int i = 0; i < item.transform.childCount; i++)
            {
                NgoDaoSlot ngoDaoSlot = item.transform.GetChild(i).GetComponent<NgoDaoSlot>();
                ngoDaoSlot.UnLearnVisual();
                ngoDaoSlot.canLearn = false;
            }
        }
        

        // check learned in data
        foreach (var item in listSkill)
        {
            for (int i = 0; i < item.transform.childCount; i++)
            {
                NgoDaoSlot ngoDaoSlot = item.transform.GetChild(i).GetComponent<NgoDaoSlot>();
                if (CharacterDatas.InventoryManager.NgoDaos.Any(x => x.Id == ngoDaoSlot.ngoDaoSO.Id))
                {
                    ngoDaoSlot.LearnedVisual();
                }
            }
        }

        var camNgoThisPage = CharacterDatas.InventoryManager.CamNgos.FirstOrDefault(x => x.Id == listNgoDaoResources[indexThisPage].eTypeSkill);
        int camNgoValue = 0;
        if (camNgoThisPage != null)
        {
            camNgoValue = camNgoThisPage.Value;
        }
        camNgoText.text = camNgoValue.ToString();
        for (int i = 0; i < NgoDaoResource.GetLevelCamNgo(camNgoValue); i++)
        {
            var item = listSkill[i];
            bool isAllUnLearn = true;
            // check từng slot hàng hiện tại xem đc phép can learn không
            for (int j = 0; j < item.transform.childCount; j++)
            {
                NgoDaoSlot ngoDaoSlot = item.transform.GetChild(j).GetComponent<NgoDaoSlot>();
                if (ngoDaoSlot.isLearned == true)
                {
                    isAllUnLearn = false;
                }
                if (CharacterDatas.Progression.NgoDaoDiem >= ngoDaoSlot.ngoDaoSO.require)
                {
                    bool allDone = true;
                    foreach (var eRequire in ngoDaoSlot.ngoDaoSO.eTypeRequire)
                    {
                        var resource = listNgoDaoResources.FirstOrDefault(x => x.eTypeSkill == eRequire);
                        CamNgo camNgo = null;
                        if (resource != null)
                        {
                            camNgo = CharacterDatas.InventoryManager.CamNgos.FirstOrDefault(x => x.Id == resource.eTypeSkill);

                            // Tiếp tục logic xử lý...
                        }
                        else
                        {
                            Debug.LogError($"Hệ {eRequire} không tồn tại trong danh sách listNgoDaoResources!");
                        }
                        if (camNgo == null) continue;
                        int levelCamNgo = NgoDaoResource.GetLevelCamNgo(camNgo.Value);
                        if (levelCamNgo < i + 1) // i là level index hàng hiện tại
                        {
                            allDone = false;
                            break;
                        }
                    }
                    ngoDaoSlot.canLearn = allDone;
                }
            }
            // hàng đầu chưa học tất cả thằng sau k cần check = là chưa đc phép học
            if (isAllUnLearn)
            {
                break;
            }
        }


    }
    private void InitCamNgoLevel(NgoDaoResource ngoDaoResource)
    {
        for (int i = 0; i < levelNgoDao.Count; i++)
        {
            var item = levelNgoDao[i];
            // name 2 // value 3
           
            item.transform.GetChild(0).gameObject.SetActive(false);
            item.transform.GetChild(1).gameObject.SetActive(true);


        }
        var CharacterDatas = CharData;
        var itemCamNgo = CharacterDatas.InventoryManager.CamNgos.FirstOrDefault(x=>x.Id == ngoDaoResource.eTypeSkill);
        if(itemCamNgo != null)
        {
            int levelIndex = NgoDaoResource.GetLevelCamNgo(itemCamNgo.Value) - 1;

            int totalStages = NgoDaoResource.CamNgoRequire.Length; // 5 mốc: 1k, 8k, 30k, 70k, 150k

            // Kiểm tra nếu đã vượt qua hoặc bằng mốc cuối cùng
            Debug.Log("dcmmm level index =  " + levelIndex);

            if (levelIndex >= totalStages - 1)
            {
                camNgoSlider.fillAmount = 1f;
            }
            else if (levelIndex < 0)
            {
                camNgoSlider.fillAmount = 0;

            }
            else
            {
                int currentLevel = levelIndex + 1;
                float currentExp = (float)itemCamNgo.Value;

                // Mốc hiện tại và mốc kế tiếp
                float minExpOfLevel = NgoDaoResource.CamNgoRequire[levelIndex];
                float maxExpOfLevel = NgoDaoResource.CamNgoRequire[levelIndex + 1];

                // 1. Tính % trong giai đoạn hiện tại
                float progressInCurrentLevel = (currentExp - minExpOfLevel) / (maxExpOfLevel - minExpOfLevel);

                // 2. Tính tổng Fill Amount
                // Chia cho totalStages để đảm bảo thanh trượt khớp với số lượng "lư hương" trên UI
                camNgoSlider.fillAmount = (currentLevel + progressInCurrentLevel) / totalStages;
            }


            if (levelIndex != -1)
            {

                Debug.Log("dcmmm ngộ đạo có vào đây ");

                for (int k = 0; k <= levelIndex; k++)
                {
                    levelNgoDao[k].transform.GetChild(0).gameObject.SetActive(true);
                    var cloud = levelNgoDao[k].transform.GetChild(0).transform.GetChild(1).GetComponent<Image>();
                    cloud.sprite = listSpriteCloudNgoDao[ngoDaoResource.ngoDao[k].list.Count - 1];
                    cloud.SetNativeSize();
                    levelNgoDao[k].transform.GetChild(1).gameObject.SetActive(false);
                }
            }

        }
        else
        {
            camNgoSlider.fillAmount = 0;

        }
        


    }
    public void ShowNgoDaoToolTip(NgoDaoSlot ngoDaoSlot)
    {
        
        ngoDaoToolTip.gameObject.SetActive(true);
        // 1. Find the first resource that actually has a value for this level
        var targetResource = listNgoDaoResources.FirstOrDefault(x => x.NgoDaoAtLevel(ngoDaoSlot.ngoDaoSO) != 0);

        // 2. Only initialize if we actually found something
        if (targetResource != null)
        {
            int levelValue = targetResource.NgoDaoAtLevel(ngoDaoSlot.ngoDaoSO);
            ngoDaoToolTip.Init(ngoDaoSlot, levelValue);
        }
        else
        {
            // Optional: Handle the case where no resource is found (e.g., hide the tooltip)
            ngoDaoToolTip.gameObject.SetActive(false);
        }
    }
    [Header("--- Bag ---")]
    [SerializeField] private Bag1ItemPrefabInventory bag;
    [SerializeField] private GameObject prefab5Item;
    [SerializeField] private Text moneyText;
    [SerializeField] private Text moneyVipText;

    [SerializeField] private Sprite[] spriteQualityFulls;
    [SerializeField] private Sprite[] spriteFrames;
    [SerializeField] private Color[] colorQualitys;
    [SerializeField] private ItemSelect itemSelect;
     [SerializeField]
     private Animator lianziAnim;
     public void InitBag()
     {
        bag.isLoadingMore = false;
        bag.gameObject.SetActive(true);
        moneyText.text = CharData.Stats.LinhThach.ToString();
        moneyVipText.text = CharData.MoneyVip.ToString();
        lianziAnim.SetTrigger("Show");

    }

    void DisableHighlight(ItemInventory item)
    {
        HighlightAnim anim = item.GetComponentInChildren<HighlightAnim>(true);

        if (anim != null)
        {
            anim.gameObject.SetActive(false);

            // Nếu bạn muốn dùng cái shader "con rắn" hồi nãy để bật bằng code:
            // anim.EnableBorder(true); 
        }
    }
    [Header("--- Công Pháp ---")]
    [SerializeField] private ItemInventory donThuat;
    [SerializeField] private ItemInventory chuTu;
    [SerializeField] private ItemInventory nguyenAnh;
    [SerializeField] private ItemInventory phuTu1;
    [SerializeField] private ItemInventory phuTu2;
    [SerializeField] private ItemInventory phuTu3;
    [SerializeField] private ItemInventory phuTu4;
    [SerializeField]
    private List<ItemInventory> listItemsCongPhapSlot => new List<ItemInventory>()
    {
        donThuat,
        chuTu,
        nguyenAnh,
        phuTu1,
        phuTu2,
        phuTu3,
        phuTu4,
    };
    private Coroutine coroutineInitCongPhap;
    ETypeSlotCP GetTypeSlotCP(RectTransform hitRect)
    {
        var CharacterDatas = CharData;

        if (hitRect == donThuat.GetComponent<RectTransform>()) return ETypeSlotCP.DonThuat;
        else if (hitRect == phuTu1.GetComponent<RectTransform>()) return ETypeSlotCP.PhuTu1;
        else if (hitRect == phuTu2.GetComponent<RectTransform>()) return ETypeSlotCP.PhuTu2;
        else if (hitRect == phuTu3.GetComponent<RectTransform>()) return ETypeSlotCP.PhuTu3;
        else if (hitRect == phuTu4.GetComponent<RectTransform>()) return ETypeSlotCP.PhuTu4;
        else if (hitRect == chuTu.GetComponent<RectTransform>()) return ETypeSlotCP.ChuTu1;
        else if (hitRect == nguyenAnh.GetComponent<RectTransform>()) return ETypeSlotCP.ChuTu2;

        // ... thêm các ô khác tương tự
        return ETypeSlotCP.None;
    }
    public void InitCongPhap(int countDelay = 10)
    {
        Observer.RemoveObserver(FilterControl.SELECT_FILTER,SelectFilter);
        var CharacterDatas = CharData;
        var GameConfig = Pool.Get<GameManager>();
        InitBag();
        bag.filterControl.InitFilterType(new List<ItemType> { ItemType.CongPhap });



        // load data 
        var equipment = CharacterDatas.EquipmentManager;
        Pool.Get<UIManager>().InitVisualItemSkill(donThuat, GameConfig.GetCongPhapWithId(equipment.DonThuatUsed));
        Pool.Get<UIManager>().InitVisualItemSkill(chuTu, GameConfig.GetCongPhapWithId(equipment.ChuTu1Used));
        Pool.Get<UIManager>().InitVisualItemSkill(nguyenAnh, GameConfig.GetCongPhapWithId(equipment.ChuTu2Used));
        Pool.Get<UIManager>().InitVisualItemSkill(phuTu1, GameConfig.GetCongPhapWithId(equipment.PhuTu1Used));
        Pool.Get<UIManager>().InitVisualItemSkill(phuTu2, GameConfig.GetCongPhapWithId(equipment.PhuTu2Used));
        Pool.Get<UIManager>().InitVisualItemSkill(phuTu3, GameConfig.GetCongPhapWithId(equipment.PhuTu3Used));
        Pool.Get<UIManager>().InitVisualItemSkill(phuTu4, GameConfig.GetCongPhapWithId(equipment.PhuTu4Used));

        // unEquip
        foreach (var currentItem in listItemsCongPhapSlot)
        {
            currentItem.InitAction(null,null, () =>
            {
                Debug.Log("có vào đây");
                if (currentItem.itemData == null)
                {
                    Debug.Log("data null");

                    return;
                }
                Pool.Get<UIManager>().InitVisualItemSkill(currentItem, null);

                Observer.Notify(EventList.Network.UNEQUIP_CONGPHAP, this, GetTypeSlotCP(currentItem.GetComponent<RectTransform>()));

            });
        }
        var gameConfig = Pool.Get<GameManager>();
        List<ItemRef> itemDatas = new List<ItemRef>();
        foreach (var item in CharacterDatas.InventoryManager.CongPhapOwners)
        {
            itemDatas.Add(new ItemRef { Id = (ushort)item.Id, Type = ItemType.CongPhap, Count = 1 });
        }

        bag.InitDataItem(itemDatas, (currentItem) =>
        {

            currentItem.InitAction(null, null, () =>
            {
                CongPhap congPhap = currentItem.itemData as CongPhap;
                // 1. Lấy RectTransform của ô Độn Thuật để so sánh chính xác
                RectTransform donThuatRect = donThuat.GetComponent<RectTransform>();



                itemSelect.Show(currentItem, listItemsCongPhapSlot.ConvertAll(x => x.GetComponent<RectTransform>()), (resultRect) =>
                {
                    if (resultRect != null)
                    {

                        var itemSlot = resultRect.GetComponent<ItemInventory>();


                        // 2. Kiểm tra điều kiện chặn
                        bool isDroppingIntoDonThuat = (resultRect == donThuatRect);
                        if (itemSlot != null && congPhap != null)
                        {// ĐIỀU KIỆN: Nếu thả vào ô Độn Thuật MÀ kỹ năng KHÔNG PHẢI Độn Thuật thì BỎ QUA
                            bool isCPEquiped = listItemsCongPhapSlot.Any(x => x.itemData == congPhap);
                            if (isDroppingIntoDonThuat && !congPhap.isDonThuat || itemSlot.itemData != null || isCPEquiped)
                            {
                                if (isCPEquiped)
                                {
                                    NotifyGroup.Instance.Show("Công pháp đã được trang bị");

                                }
                                else
                                {
                                    NotifyGroup.Instance.Show("Công pháp không đúng loại");

                                }

                                return; // Thoát ra, không chạy code InitAsFurnace bên dưới
                            }
                            Debug.Log("Item slot sẵn sàng");

                            // Sử dụng biến tạm 'currentItem' hoặc 'index' thay vì 'i'
                            Pool.Get<UIManager>().InitVisualItemSkill(itemSlot, congPhap);
                            //equip
                            //congPhap.EquipCongPhap();
                            //save player cong phap equip id
                            Observer.Notify(EventList.Network.EQUIP_CONGPHAP, this, GetTypeSlotCP(resultRect), (ushort)congPhap.Id);
                            // Cách viết chuẩn


                        }
                    }
                    else
                    {
                        Debug.Log("Thả trượt rồi!");
                    }
                });

                foreach (var item in listItemsCongPhapSlot)
                {
                    if (item.itemData != null)
                    {
                        DisableHighlight(item);
                    }
                }
                if (!congPhap.isDonThuat)
                {
                    DisableHighlight(donThuat);

                }
                else
                {
                    foreach (var item in listItemsCongPhapSlot)
                    {
                        if (item != donThuat)
                        {
                            DisableHighlight(item);
                        }
                    }
                }

            });
        }, true);
        bag.InitBag(false);
      

    }
 
    [Header("--- Thần Thông ---")]
    [SerializeField] private List<ItemInventory> listItemSlotThanThongs;
    // private Coroutine coroutineInitThanThong = null;
    public void InitThanThong(int countDelay = 10)
    {
        Observer.RemoveObserver(FilterControl.SELECT_FILTER, SelectFilter);
        InitBag();
        bag.filterControl.InitFilterType(new List<ItemType> { ItemType.ThanThong });
        var CharacterDatas = CharData;
        var GameConfig = Pool.Get<GameManager>();

        // load data 
        var thanThongEquips = CharacterDatas.EquipmentManager.ThanThongEquips;
        for (int i = 0; i < listItemSlotThanThongs.Count; i++)
        {
            var item = listItemSlotThanThongs[i];
            ThanThong thanThongData = null;
            
            if(i < thanThongEquips.Count)
            {
                thanThongData = GameConfig.GetThanThongWithId(thanThongEquips.FirstOrDefault(x=>x.SlotIndex == (byte)i).Id);
            }    

            Pool.Get<UIManager>().InitVisualItemSkill(item, thanThongData);
        }
        // unEquip
        foreach (var currentItem in listItemSlotThanThongs)
        {

            currentItem.InitAction(null, null, () =>
            {
                ThanThong thanThong = currentItem.itemData as ThanThong;
                Debug.Log("có vào đây");
                if (thanThong == null)
                {
                    Debug.Log("data null");

                    return;
                }
                Observer.Notify(EventList.Network.UNEQUIP_THANTHONG, this, thanThong.Id);
                Pool.Get<UIManager>().InitVisualItemSkill(currentItem, null);


            });
        }
        var gameConfig = Pool.Get<GameManager>();
        List<ItemRef> itemDatas = new List<ItemRef>();
        foreach (var item in CharacterDatas.InventoryManager.ThanThongOwners)
        {
            itemDatas.Add(new ItemRef { Id = item.Id,Type = ItemType.ThanThong,Count = 1});
        } 
            
        bag.InitDataItem(itemDatas, (currentItem) =>
        {

            currentItem.InitAction(null, null, () =>
            {


                ThanThong thanThong = currentItem.itemData as ThanThong;


                itemSelect.Show(currentItem, listItemSlotThanThongs.ConvertAll(x => x.GetComponent<RectTransform>()), (resultRect) =>
                {
                    if (resultRect != null)
                    {

                        var itemSlot = resultRect.GetComponent<ItemInventory>();


                        if (itemSlot != null && thanThong != null)
                        {// ĐIỀU KIỆN: Nếu thả vào ô Độn Thuật MÀ kỹ năng KHÔNG PHẢI Độn Thuật thì BỎ QUA
                            if (itemSlot.itemData != null || listItemSlotThanThongs.Any(x => x.itemData == thanThong))
                            {
                                NotifyGroup.Instance.Show("Thần thông đã được trang bị");
                                return; // Thoát ra, không chạy code InitAsFurnace bên dưới
                            }
                            Debug.Log("Item slot sẵn sàng");

                            // Sử dụng biến tạm 'currentItem' hoặc 'index' thay vì 'i'
                            Pool.Get<UIManager>().InitVisualItemSkill(itemSlot, thanThong);
                            //equip
                            //congPhap.EquipCongPhap();
                            //save player thần thông equip id
                            Observer.Notify(EventList.Network.EQUIP_THANTHONG, this, (ushort)thanThong.Id ,
                                (byte)listItemSlotThanThongs.IndexOf(itemSlot));
                            // Cách viết chuẩn


                        }
                    }
                    else
                    {
                        Debug.Log("Thả trượt rồi!");
                    }
                });
                foreach (var item in listItemSlotThanThongs)
                {
                    if (item.itemData != null)
                    {
                        DisableHighlight(item);
                    }
                }




            });
        } , true);
        bag.InitBag(false);
        
    }
 
    [Header("--- Inventory ---")]
    [SerializeField] public ItemInventory itemWeapon;
    [SerializeField] public ItemInventory itemPhongCu;
    [SerializeField] public ItemInventory itemTrangSuc;

    [SerializeField] private CharacterGraphic charPageInventory;
    [SerializeField] public SelectNum selectNum;
    public void LoadEquipment(ItemInventory currentItem, ItemType itemType, ushort id)
    {
        var equipment = ItemProviderFactory.GetProvider<CreateCharacter.Equipment>(itemType).GetDataItemById(id);
        if (equipment == null) return;
        Pool.Get<UIManager>().InitVisualEquipment(currentItem, null, equipment.CreateRef());
        SetActionUnEquipEquipment(itemWeapon);
        SetActionUnEquipEquipment(itemPhongCu);
        SetActionUnEquipEquipment(itemTrangSuc);
    }
    void SetActionUnEquipEquipment(ItemInventory currentItem)
    {

        currentItem.InitAction(null, null, () =>
        {
            Debug.Log("có vào đây");
            if (currentItem.itemRef == null || currentItem.itemData == null || currentItem.itemRef.Type != ItemType.VuKhi && currentItem.itemRef.Type != ItemType.PhongCu && currentItem.itemRef.Type != ItemType.VuKhi)
            {
                Debug.Log("data null");

                return;
            }
            EquipmentType equipmentType = GameManager.ItemTypeToEquipemtType.GetValueOrDefault(currentItem.itemRef.Type);

            Observer.Notify(EventList.Network.UNEQUIP_EQUIPMENT, this, currentItem.itemRef.Id, equipmentType); // id equipment này là self id
            Pool.Get<UIManager>().InitVisualEquipment(currentItem, null);

        });
    }
    public void InitInventory()
    {
        InitBag();
        bag.filterControl.InitAllFilterType();
      
       
        LoadEquipment(itemWeapon,ItemType.VuKhi,Client.Instance.CharacterData.EquipmentManager.VuKhi);
        LoadEquipment(itemPhongCu, ItemType.PhongCu, Client.Instance.CharacterData.EquipmentManager.PhongCu);
        LoadEquipment(itemTrangSuc, ItemType.TrangSuc, Client.Instance.CharacterData.EquipmentManager.TrangSuc);

        // unEquip Acion
        

        var characterData = CharData;
        var gameConfig = Pool.Get<GameManager>();
        List<ItemRef> itemDatas = new List<ItemRef>();
        itemDatas.AddRange(characterData.InventoryManager.Herbs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.Recipes.ToList());
        itemDatas.AddRange(characterData.InventoryManager.ThanThongs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.CongPhaps.ToList());
        itemDatas.AddRange(characterData.InventoryManager.Materials.ToList());
      

        // itemDatas.AddRange(characterData.InventoryManager.VuKhis.ToList());
        // itemDatas.AddRange(characterData.InventoryManager.PhongCus.ToList());
        // itemDatas.AddRange(characterData.InventoryManager.TrangSucs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.LinhDans.ToList());
        // Sử dụng LINQ để trích xuất Ref từ mỗi phần tử trong furnaceDatas
        var furnaceRefs = characterData.InventoryManager.Furnaces.Select(f => f.CreateRefSelfID()).ToList();


        // Thêm danh sách Ref vừa lấy được vào itemDatas
        itemDatas.AddRange(furnaceRefs);
        itemDatas.AddRange(characterData.InventoryManager.VuKhis.Select(f => f.CreateRef()).ToList());
        itemDatas.AddRange(characterData.InventoryManager.PhongCus.Select(f => f.CreateRef()).ToList());
        itemDatas.AddRange(characterData.InventoryManager.TrangSucs.Select(f => f.CreateRef()).ToList());
        bag.InitDataItem(itemDatas, (ItemInventory) =>
        {
            ItemInventory.InitAction(null,  () =>
            {
                ItemProviderFactory.GetUseItemProvider(ItemInventory.itemRef.Type).UseItem(ItemInventory);
               
             
            },null);
        });
        bag.InitBag(false);

        charPageInventory.SetGender(CharData.Gender);
        charPageInventory.SetSkin(CharData.Appearance.Skins);

    }
   
    public void SuccessEquipCongPhap()
    {
        Observer.Notify(EventList.GamePlay.UPDATE_PLAYER_STATS);
    }

    public void FailEquipCongPhap(string message)
    {
        Debug.LogError(message);
    }

    public void SuccessUnEquipCongPhap()
    {
        Observer.Notify(EventList.GamePlay.UPDATE_PLAYER_STATS);
        Debug.Log("equip success");

    }

    public void FailUnEquipCongPhap(string message)
    {
        Debug.LogError(message);
    }

    public void SuccessUnEquipThanThong()
    {       
        Debug.Log("unequip ThanThong");

    }

    public void FailUnEquipThanThong(string message)
    {
        Debug.LogError(message);
    }

    public void SuccessEquipThanThong()
    {
        Debug.Log("EquipThanThong");
    }

    public void FailEquipThanThong(string message)
    {
        Debug.LogError(message);
    }

    public void SuccessUseDan()
    {
        Observer.Notify(EventList.GamePlay.UPDATE_PLAYER_STATS);

    }

    public void FailUseDan(string message)
    {
    }

    public void SuccessEquipEquipment(List<TaskDailySnapshot> listTask)
    {
    }

    public void FailEquipEquipment(string message)
    {
    }

    public void SuccessUnEquipEquipment()
    {
    }

    public void FailUnEquipEquipment(string message)
    {
    }
}
