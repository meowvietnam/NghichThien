using AllMaps;
using Base;
using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class TabPanel : PanelGroup , INetworkEvent.IEquipCongPhap , INetworkEvent.IUnEquipCongPhap,INetworkEvent.IEquipThanThong, INetworkEvent.IUnEquipThanThong
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
    public override void Init()
    {
        base.Init();
        foreach (var item in btns)
        {
            SetEventButton(item.btn, () =>
            {
                if(currentBtnWithPanel.typePanel == item.typePanel)
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
                if (item.typePanel == ETypePanel.ThanThong)
                {
                    InitThanThong();
                }
                
            });
        }
        Observer.AddObserver(EventList.GamePlay.UPDATE_PLAYER_STATS, UpdatePlayerStats);
        SetEventButton(btnClose, () =>
        {
            gameObject.SetActive(false);
        });
        InitNgoDao();
    }
    private void OnEnable()
    {
        btns[0]?.btn?.onClick.Invoke();
        if(dictBtnWithNgoDaos?.Count>0)
        {
            dictBtnWithNgoDaos?.ElementAt(0).Key?.onClick.Invoke();
        }    
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


    private void UpdatePlayerStats()
    {
        var player = Pool.Get<Player>();
        hpText.text = $"{player.characterStatsBase.hp.Value}/{player.characterStatsBase.hpMax.Value}";

        tuviText.text = $"{player.characterStatsBase.currentExp.Value}/{GameConfig.GetTuviRequired(player.characterStatsBase.level)}";

        iconLevel.sprite = listSpriteIconLevel[player.characterStatsBase.level - 1];
        iconLevel.SetNativeSize();
        nameText.text = player.characterStatsBase.nameUser;

        levelText.text = GameConfig.tuviText[player.characterStatsBase.level-1];
        tuoitacText.text = player.characterStatsBase.tuoiTac.Value.ToString();
        thonguyenText.text = player.characterStatsBase.thoNguyen.Value.ToString();
        tuChatText.text = player.characterStatsBase.tuChat.Value.ToString();
        thanThucText.text = player.characterStatsBase.thanThuc.Value.ToString();
        ngoTinhText.text = player.characterStatsBase.ngoTinh.Value.ToString();
        donTocText.text = player.characterStatsBase.tocDoBay.Value.ToString();
        //tocDoTuLuyenText.text = player.characterStatsBase.tocDoBay.Value.ToString();
        var getTamCanh = GameConfig.GetTamCanhValue(player.characterStatsBase.tamCanh);
        var getDanDoc = GameConfig.GetDanDocValue(player.characterStatsBase.danDoc);

        tamCanhText.text = getTamCanh.name;
        tamCanhText.color = listColorTamCanh[getTamCanh.index];
        tamCanhValue.text = $"{player.characterStatsBase.tamCanh.Value}/{getTamCanh.nextMilestone}";
        tamCanhIcon.sprite = listSpriteIconTamCanh[getTamCanh.index];

        danDocText.text = getDanDoc.text;
        danDocText.color = listColorDanDoc[getDanDoc.index];
        danDocValue.text = $"{player.characterStatsBase.danDoc.Value}/{getDanDoc.nextMilestone}";
        danDocIcon.sprite = listSpriteIconDanDoc[getDanDoc.index];

        kimText.text = player.characterStatsBase.percentKim.Value.ToString();
        mocText.text = player.characterStatsBase.percentMoc.Value.ToString();
        thuyText.text = player.characterStatsBase.percentThuy.Value.ToString();
        hoaText.text = player.characterStatsBase.percentHoa.Value.ToString();
        thoText.text = player.characterStatsBase.percentTho.Value.ToString();

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
    public void InitNgoDao()
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

    }
    public void SetNgoDaoDiem()
    {
        ngoDaoDiemText.text = Pool.Get<Player>().characterStatsBase.ngoDaoDiem.Value.ToString();

    }
    public void InitNgoDaoSlotCanLearn(List<RectTransform> listSkill)
    {
        var player = Pool.Get<Player>();

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
                if (player.characterStatsBase.ngoDaoSaves.Any(x => x == ngoDaoSlot.ngoDaoSO.Id))
                {
                    ngoDaoSlot.LearnedVisual();
                }
            }
        }

        var camNgoThisPage = player.characterStatsBase.camNgoSaves.FirstOrDefault(x => x.id == listNgoDaoResources[indexThisPage].Id);
        for (int i = 0; i < NgoDaoResource.GetLevelCamNgo(camNgoThisPage.value.Value); i++)
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
                if (player.characterStatsBase.ngoDaoDiem >= ngoDaoSlot.ngoDaoSO.require)
                {
                    bool allDone = true;
                    foreach (var eRequire in ngoDaoSlot.ngoDaoSO.eTypeRequire)
                    {
                        var camNgo = player.characterStatsBase.camNgoSaves.FirstOrDefault(x => x.id == listNgoDaoResources[(int)eRequire-1].Id);
                        int levelCamNgo = NgoDaoResource.GetLevelCamNgo(camNgo.value.Value);
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
        var player = Pool.Get<Player>();
        foreach (var item in player.characterStatsBase.camNgoSaves)
        {
            if(item.id == ngoDaoResource.Id)
            {
                int levelIndex = NgoDaoResource.GetLevelCamNgo(item.value.Value) -1;
               
                int totalStages = NgoDaoResource.CamNgoRequire.Length; // 5 mốc: 1k, 8k, 30k, 70k, 150k

                // Kiểm tra nếu đã vượt qua hoặc bằng mốc cuối cùng
                if (levelIndex >= totalStages - 1)
                {
                    camNgoSlider.fillAmount = 1f;
                }
                else if(levelIndex <= 0)
                {
                    camNgoSlider.fillAmount = 0;

                }
                else
                {
                    int currentLevel = levelIndex + 1;
                    float currentExp = (float)item.value.Value;

                    // Mốc hiện tại và mốc kế tiếp
                    float minExpOfLevel = NgoDaoResource.CamNgoRequire[levelIndex];
                    float maxExpOfLevel = NgoDaoResource.CamNgoRequire[levelIndex + 1];

                    // 1. Tính % trong giai đoạn hiện tại
                    float progressInCurrentLevel = (currentExp - minExpOfLevel) / (maxExpOfLevel - minExpOfLevel);

                    // 2. Tính tổng Fill Amount
                    // Chia cho totalStages để đảm bảo thanh trượt khớp với số lượng "lư hương" trên UI
                    camNgoSlider.fillAmount = (currentLevel + progressInCurrentLevel) / totalStages;
                }


                camNgoText.text = item.value.Value.ToString();
                if(levelIndex != -1)
                {

                    Debug.Log("dcmmm ngộ đạo có vào đây " + levelIndex);

                    for (int k = 0; k <= levelIndex; k++)
                    {
                        levelNgoDao[k].transform.GetChild(0).gameObject.SetActive(true);
                        var cloud = levelNgoDao[k].transform.GetChild(0).transform.GetChild(1).GetComponent<Image>();
                        cloud.sprite = listSpriteCloudNgoDao[ngoDaoResource.ngoDao[k].list.Count - 1];
                        cloud.SetNativeSize();
                        levelNgoDao[k].transform.GetChild(1).gameObject.SetActive(false);
                    }    
                }    



                break;
            }    
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
    [SerializeField] private GameObject bag;
    [SerializeField] private GameObject prefab5Item;
    [SerializeField] private Text moneyText;
    [SerializeField] private Sprite[] spriteQualityFulls;
    [SerializeField] private Sprite[] spriteFrames;
    [SerializeField] private Color[] colorQualitys;
    [SerializeField] private ItemSelect itemSelect;
    public void InitItemBag(int count, ref List<ItemInventory> listItemInventory)
    {
       
            
        if (count <= 0) return; // Không có item thì không làm gì cả

        // Ví dụ: 3/5 = 0.6 -> làm tròn lên 1
        // Ví dụ: 7/5 = 1.4 -> làm tròn lên 2
        int spawnCount = Mathf.CeilToInt((float)count / 5);
        for (int i = 0; i < prefab5Item.transform.parent.childCount; i++)
        {
            prefab5Item.transform.parent.GetChild(i).gameObject.SetActive(false);
        }   
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject item5 = null;
              
            if (prefab5Item.transform.parent.childCount > 1 && i + 1 < prefab5Item.transform.parent.childCount) // vì cái đầu tiên là prefab
            {
                item5 = prefab5Item.transform.parent.GetChild(i+1).gameObject;
            }
            if(item5 == null)
            {
                item5 = Instantiate(prefab5Item, prefab5Item.transform.parent); // Spawn prefab vào vị trí mong muốn

            }

            item5.gameObject.SetActive(true);
            for (int j = 0; j < item5.transform.childCount; j++)
            {
                ItemInventory item = item5.transform.GetChild(j).GetComponent<ItemInventory>();
                listItemInventory.Add(item);
               
            }

        }


    }
    private void InitVisualItemSkill(ItemInventory item, BaseSkill data)
    {
        Sprite frame = null;
        if(data != null)
        {
            frame = spriteFrames[((((int)data.eTierSkill - 1) / 3) * 2) + 1];

            item.SetColorName(colorQualitys[((((int)data.eTierSkill - 1) / 3) * 2) + 1]);
            item.SetQualityFull(spriteQualityFulls[((((int)data.eTierSkill - 1) / 3) * 2) + 1]);

        }

        item.Init(data, frame);

        item.InitInventory(ETypeInventoryItem.Skill);

       
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
  
    public void InitCongPhap()
    {
        ETypeSlotCP GetTypeSlotCP(RectTransform hitRect)
        {
            var stats = Pool.Get<Player>().characterStatsBase;

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

        bag.gameObject.SetActive(true);
        var Player = Pool.Get<Player>();
        var GameConfig = Pool.Get<GameConfig>();
        List<ItemInventory> itemInven = new List<ItemInventory>();
        List<CongPhap> listCongPhapData = new List<CongPhap>();

        foreach (var item in Player.characterStatsBase.congPhapOwner)
        {
            listCongPhapData.Add(GameConfig.GetCongPhapWithId(item.id));
        }
        InitItemBag(listCongPhapData.Count, ref itemInven);

        // load data 
        InitVisualItemSkill(donThuat, GameConfig.GetCongPhapWithId(Player.characterStatsBase.donThuatUsed));
        InitVisualItemSkill(chuTu, GameConfig.GetCongPhapWithId(Player.characterStatsBase.chuTu1Used));
        InitVisualItemSkill(nguyenAnh, GameConfig.GetCongPhapWithId(Player.characterStatsBase.chuTu2Used));
        InitVisualItemSkill(phuTu1, GameConfig.GetCongPhapWithId(Player.characterStatsBase.phuTu1Used));
        InitVisualItemSkill(phuTu2, GameConfig.GetCongPhapWithId(Player.characterStatsBase.phuTu2Used));
        InitVisualItemSkill(phuTu3, GameConfig.GetCongPhapWithId(Player.characterStatsBase.phuTu3Used));
        InitVisualItemSkill(phuTu4, GameConfig.GetCongPhapWithId(Player.characterStatsBase.phuTu4Used));

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
                    
                itemSelect.Show(currentItem.itemData, new List<RectTransform> { bag.GetComponent<RectTransform>()}, (resultRect) =>
                {
                    if (resultRect != null)
                    {

                        Observer.Notify(EventList.Network.UNEQUIP_CONGPHAP, currentItem.itemData.Id ,this, GetTypeSlotCP(currentItem.GetComponent<RectTransform>()));

                        // Cách viết chuẩn


                    }
                    else
                    {
                        Debug.Log("Thả trượt rồi!");

                        InitVisualItemSkill(currentItem, (BaseSkill)itemSelect.itemData);

                    }
                });
                InitVisualItemSkill(currentItem, null);

            });
        }

        //equip
        for (int i = 0; i < itemInven.Count; i++)
        {// TẠO BIẾN TẠM Ở ĐÂY
            int index = i;
            var currentItem = itemInven[i];
            BaseSkill data = null;
            if(i<= listCongPhapData.Count-1)
            {
                data = listCongPhapData[i];
            }
            InitVisualItemSkill(currentItem, data);
            if (data != null)
            {

                currentItem.InitAction(null,null, () =>
                {
                    CongPhap congPhap = currentItem.itemData as CongPhap;
                    // 1. Lấy RectTransform của ô Độn Thuật để so sánh chính xác
                    RectTransform donThuatRect = donThuat.GetComponent<RectTransform>();

                        

                    itemSelect.Show(currentItem.itemData, listItemsCongPhapSlot.ConvertAll(x => x.GetComponent<RectTransform>()), (resultRect) =>
                    {
                        if (resultRect != null)
                        {

                            var itemSlot = resultRect.GetComponent<ItemInventory>();
                            

                            // 2. Kiểm tra điều kiện chặn
                            bool isDroppingIntoDonThuat = (resultRect == donThuatRect);
                            if (itemSlot != null && congPhap != null)
                            {// ĐIỀU KIỆN: Nếu thả vào ô Độn Thuật MÀ kỹ năng KHÔNG PHẢI Độn Thuật thì BỎ QUA
                                if (isDroppingIntoDonThuat && !congPhap.isDonThuat || itemSlot.itemData != null || listItemsCongPhapSlot.Any(x=>x.itemData == congPhap))
                                {
                                    Debug.Log("Không thể trang bị: Kỹ năng này không phải Độn Thuật!");
                                    return; // Thoát ra, không chạy code Init bên dưới
                                }
                                Debug.Log("Item slot sẵn sàng");

                                // Sử dụng biến tạm 'currentItem' hoặc 'index' thay vì 'i'
                                InitVisualItemSkill(itemSlot, congPhap);
                                //equip
                                //congPhap.EquipCongPhap();
                                //save player cong phap equip id
                                Observer.Notify(EventList.Network.EQUIP_CONGPHAP, congPhap.Id , this,GetTypeSlotCP(resultRect));
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
            }
            

        }

    }
    [Header("--- Thần Thông ---")]
    [SerializeField] private List<ItemInventory> listItemSlotThanThongs;
    public void InitThanThong()
    {
        
        bag.gameObject.SetActive(true);
        var Player = Pool.Get<Player>();
        var GameConfig = Pool.Get<GameConfig>();
        List<ItemInventory> itemInven = new List<ItemInventory>();
        List<ThanThong> listThanThongData = new List<ThanThong>();

        foreach (var item in Player.characterStatsBase.thanThongOwner)
        {
            listThanThongData.Add(GameConfig.GetThanThongWithId(item.Value));
        }
        InitItemBag(listThanThongData.Count, ref itemInven);
        // load data 
        for (int i = 0; i < listItemSlotThanThongs.Count; i++)
        {
            var item = listItemSlotThanThongs[i];
            ThanThong thanThongData = null;
            if(i < Player.characterStatsBase.thanThongEquip.Count)
            {
                thanThongData = GameConfig.GetThanThongWithId(Player.characterStatsBase.thanThongEquip[i]);
            }    

            InitVisualItemSkill(item, thanThongData);
        }
        // unEquip
        foreach (var currentItem in listItemSlotThanThongs)
        {
            currentItem.InitAction(null, null, () =>
            {
                Debug.Log("có vào đây");
                if (currentItem.itemData == null)
                {
                    Debug.Log("data null");

                    return;
                }

                itemSelect.Show(currentItem.itemData, new List<RectTransform> { bag.GetComponent<RectTransform>() }, (resultRect) =>
                {
                    if (resultRect != null)
                    {
                        Observer.Notify(EventList.Network.UNEQUIP_THANTHONG, listItemSlotThanThongs.IndexOf(currentItem), this);
                        Debug.Log("updaet thần thông id unequip");
                        // Cách viết chuẩn


                    }
                    else
                    {
                        Debug.Log("Thả trượt rồi!");

                        InitVisualItemSkill(currentItem, (BaseSkill)itemSelect.itemData);

                    }
                });
                InitVisualItemSkill(currentItem, null);

            });
        }

        //equip
        for (int i = 0; i < itemInven.Count; i++)
        {// TẠO BIẾN TẠM Ở ĐÂY
            int index = i;
            var currentItem = itemInven[i];
            BaseSkill data = null;
            if (i <= listThanThongData.Count - 1)
            {
                data = listThanThongData[i];
            }
            InitVisualItemSkill(currentItem, data);
            if (data != null)
            {

                currentItem.InitAction(null, null, () =>
                {
                    ThanThong thanThong = currentItem.itemData as ThanThong;
                  

                    itemSelect.Show(currentItem.itemData, listItemSlotThanThongs.ConvertAll(x => x.GetComponent<RectTransform>()), (resultRect) =>
                    {
                        if (resultRect != null)
                        {

                            var itemSlot = resultRect.GetComponent<ItemInventory>();


                            if (itemSlot != null && thanThong != null)
                            {// ĐIỀU KIỆN: Nếu thả vào ô Độn Thuật MÀ kỹ năng KHÔNG PHẢI Độn Thuật thì BỎ QUA
                                if (itemSlot.itemData != null || listItemSlotThanThongs.Any(x => x.itemData == thanThong))
                                {
                                    Debug.Log("Không thể trang bị: Kỹ năng này không phải Độn Thuật!");
                                    return; // Thoát ra, không chạy code Init bên dưới
                                }
                                Debug.Log("Item slot sẵn sàng");

                                // Sử dụng biến tạm 'currentItem' hoặc 'index' thay vì 'i'
                                InitVisualItemSkill(itemSlot, thanThong);
                                //equip
                                //congPhap.EquipCongPhap();
                                //save player thần thông equip id
                                Observer.Notify(EventList.Network.EQUIP_THANTHONG, listItemSlotThanThongs.IndexOf(currentItem), this , thanThong.Id);
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
            }


        }
    }

    public void SuccessEquipCongPhap()
    {
       // reload lại tab congphap
    }

    public void FailEquipCongPhap(string message)
    {
        Debug.LogError(message);
    }

    public void SuccessUnEquipCongPhap()
    {
        // reload lại tab congphap

    }

    public void FailUnEquipCongPhap(string message)
    {
        Debug.LogError(message);
    }

    public void SuccessUnEquipThanThong()
    {       // reload lại tab thanThong

    }

    public void FailUnEquipThanThong(string message)
    {
        Debug.LogError(message);
    }

    public void SuccessEquipThanThong()
    {
        // reload lại tab thanThong
    }

    public void FailEquipThanThong(string message)
    {
        Debug.LogError(message);
    }
}
