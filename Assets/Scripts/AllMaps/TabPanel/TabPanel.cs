using AllMaps;
using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using Network;
using System;
using System.Collections;
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
                    scrollRectBag.onValueChanged.RemoveAllListeners();
                    scrollRectBag.onValueChanged.AddListener((vec) => {
                        OnScrollChanged(vec, LoadBagCongPhap);
                    });

                }
                if (item.typePanel == ETypePanel.ThanThong)
                {
                    InitThanThong();
                    scrollRectBag.onValueChanged.RemoveAllListeners();
                    scrollRectBag.onValueChanged.AddListener((vec) => {
                        OnScrollChanged(vec, LoadBagThanThong);
                    });
                }
                if (item.typePanel == ETypePanel.VatPham)
                {
                  
                    InitInventory(3);
                    scrollRectBag.onValueChanged.RemoveAllListeners();
                    scrollRectBag.onValueChanged.AddListener((vec) => {
                        OnScrollChanged(vec, LoadBagVatPham);
                    });
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
        var gameConfig = Pool.Get<GameConfig>();
        hpText.text = $"{player.characterStatsBase.hp.Value}/{player.characterStatsBase.hpMax.Value}";

        tuviText.text = $"{player.characterStatsBase.currentExp.Value}/{gameConfig.inforLevels[Client.Instance.CharacterData.Level - 1].tuviRequireNextLevel}";

        iconLevel.sprite = listSpriteIconLevel[player.characterStatsBase.level - 1];
        iconLevel.SetNativeSize();
        nameText.text = player.characterStatsBase.nameUser;

        levelText.text = GameConfig.levelWithText[Client.Instance.CharacterData.Level];
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
    public bool isLoadingMore = false;
    public ScrollRect scrollRectBag => prefab5Item.GetComponentInParent<ScrollRect>();


    // Hàm OnScrollChanged sửa lại tham số truyền vào
    void OnScrollChanged(Vector2 value, Func<int, bool, IEnumerator> loadBagFunc)
    {
        Debug.Log("co vao onscrooll change");

        if (value.y < 0.05f && !isLoadingMore)
        {
            // Kiểm tra xem dữ liệu còn để load không trước khi gọi
            // (Ví dụ: compare số lượng UI hiện có với tổng số data thật)
            StartCoroutine(TriggerLoadMore(loadBagFunc));
        }
    }
    // Sử dụng delegate trả về IEnumerator thay vì Action để có thể yield return
    IEnumerator TriggerLoadMore(Func<int, bool, IEnumerator> loadBagFunc)
    {
        isLoadingMore = true;
        Debug.Log("co vao trigger load more");

        // Gọi trực tiếp hàm Coroutine và chờ nó chạy xong
        yield return StartCoroutine(loadBagFunc(10, true));

        // Đợi một chút để tránh việc gọi liên tục khi user giữ chuột ở đáy
        yield return new WaitForSeconds(0.5f);
        isLoadingMore = false;
    }
    private ItemInventory GetItemCurrentActiveFromBag()
    {
        for (int i = prefab5Item.transform.parent.childCount - 1; i >0; i--)
        {
            var item5 = prefab5Item.transform.parent.GetChild(i);
            if(!item5.gameObject.activeSelf)
            {
                continue;
            }
            for (int j = item5.transform.childCount - 1; j>0; j--)
            {
                var currentItem = item5.transform.GetChild(j).GetComponent<ItemInventory>();
                if (currentItem.itemData != null)
                {
                    Debug.Log("GetItemCurrentActiveFromBag: " + currentItem.itemData.nameItem);
                    return currentItem;
                }    
            }    
        }
        return null;
    }    
    public void InitItemBag(int count, ref List<ItemInventory> listItemInventory,bool isAdd = false)
    {
        int totalMax = 30;
        if(count >= totalMax)
        {
            count = totalMax;
        }    
        bag.gameObject.SetActive(true);
        if(!isAdd)
        {
            for (int i = 0; i < prefab5Item.transform.parent.childCount; i++)
            {
                prefab5Item.transform.parent.GetChild(i).gameObject.SetActive(false);
            }

        }    
        if (count <= 0) return; // Không có item thì không làm gì cả

        // Ví dụ: 3/5 = 0.6 -> làm tròn lên 1
        // Ví dụ: 7/5 = 1.4 -> làm tròn lên 2
        int spawnCount = Mathf.CeilToInt((float)count / 5);
         
        for (int i = 0; i < spawnCount; i++)
        {
            Transform item5 = GameConfig.GetItemPool(prefab5Item.transform);
            for (int j = 0; j < item5.transform.childCount; j++)
            {
                ItemInventory item = item5.transform.GetChild(j).GetComponent<ItemInventory>();
                item.itemData = null;
                item.itemRef = null;
                listItemInventory.Add(item);
               
            }

        }


    }
    private void InitVisualItemSkill(ItemInventory item, BaseSkill data , ETypeInventoryItem eTypeInventoryItem= ETypeInventoryItem.Skill)
    {
        Sprite frame = null;
        if(data != null)
        {
            frame = spriteFrames[((((int)data.eTierSkill - 1) / 3) * 2) + 1];

            item.SetColorName(colorQualitys[((((int)data.eTierSkill - 1) / 3) * 2) + 1]);
            item.SetQualityFull(spriteQualityFulls[((((int)data.eTierSkill - 1) / 3) * 2) + 1]);

        }

        item.Init(data, frame);

        item.InitInventory(eTypeInventoryItem,null,null);

       
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
    public void InitCongPhap(int countDelay = 10)
    {
        

        var Player = Pool.Get<Player>();
        var GameConfig = Pool.Get<GameConfig>();

      

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
                    
                itemSelect.Show(currentItem, new List<RectTransform> { bag.GetComponent<RectTransform>()}, (resultRect) =>
                {
                    if (resultRect != null)
                    {

                        Observer.Notify(EventList.Network.UNEQUIP_CONGPHAP,this, GetTypeSlotCP(currentItem.GetComponent<RectTransform>()));
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
        if (coroutineInitCongPhap != null)
        {
            StopCoroutine(coroutineInitCongPhap);
            coroutineInitCongPhap = null;
        }
        coroutineInitCongPhap = StartCoroutine(LoadBagCongPhap(countDelay));

    }
    private IEnumerator LoadBagCongPhap(int countDelay = 10,bool isAdd = false)
    {

        var Player = Pool.Get<Player>();
        var GameConfig = Pool.Get<GameConfig>();
        List<ItemInventory> itemInven = new List<ItemInventory>();
        List<CongPhap> listCongPhapData = new List<CongPhap>();
        int startIndex = 0;
       
        foreach (var item in Player.characterStatsBase.congPhapOwner)
        {
            listCongPhapData.Add(GameConfig.GetCongPhapWithId(item.id));
        }
        if (isAdd)
        {

            CongPhap cp = (CongPhap)GetItemCurrentActiveFromBag().itemData;
            if (cp == null) yield break;
         
            startIndex = listCongPhapData.IndexOf(cp) + 1;
            if(startIndex < listCongPhapData.Count-1)
            {
                InitItemBag(5, ref itemInven, isAdd);
            }    


        }
        else
        {
            InitItemBag(listCongPhapData.Count, ref itemInven, isAdd);

        }
        //equip
        for (int i = 0; i < itemInven.Count; i++)
        {// TẠO BIẾN TẠM Ở ĐÂY
            int index = i+startIndex;
            if (i % countDelay == 0) yield return null;
            var currentItem = itemInven[i];
            BaseSkill data = null;
            if (index <= listCongPhapData.Count - 1)
            {
                data = listCongPhapData[index];
            }
            InitVisualItemSkill(currentItem, data);
            if (data != null)
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

                                    return; // Thoát ra, không chạy code Init bên dưới
                                }
                                Debug.Log("Item slot sẵn sàng");

                                // Sử dụng biến tạm 'currentItem' hoặc 'index' thay vì 'i'
                                InitVisualItemSkill(itemSlot, congPhap);
                                //equip
                                //congPhap.EquipCongPhap();
                                //save player cong phap equip id
                                Observer.Notify(EventList.Network.EQUIP_CONGPHAP, this, GetTypeSlotCP(resultRect), congPhap.Id);
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
    private Coroutine coroutineInitThanThong = null;
    public void InitThanThong(int countDelay = 10)
    {
        
        var Player = Pool.Get<Player>();
        var GameConfig = Pool.Get<GameConfig>();
        
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

                itemSelect.Show(currentItem, new List<RectTransform> { bag.GetComponent<RectTransform>() }, (resultRect) =>
                {
                    if (resultRect != null)
                    {
                        Observer.Notify(EventList.Network.UNEQUIP_THANTHONG,this, listItemSlotThanThongs.IndexOf(currentItem));
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
        if (coroutineInitThanThong != null)
        {
            StopCoroutine(coroutineInitThanThong);
            coroutineInitThanThong = null;
        }
        coroutineInitThanThong = StartCoroutine(LoadBagThanThong(countDelay));
       
    }
    private IEnumerator LoadBagThanThong(int countDelay = 10, bool isAdd = false)
    {

        var Player = Pool.Get<Player>();
        var GameConfig = Pool.Get<GameConfig>();
        int startIndex = 0;

        List<ThanThong> listThanThongData = new List<ThanThong>();

        foreach (var item in Player.characterStatsBase.thanThongOwner)
        {
            listThanThongData.Add(GameConfig.GetThanThongWithId(item.Value));
        }
        List<ItemInventory> itemInven = new List<ItemInventory>();
        if (isAdd)
        {

            ThanThong tt = (ThanThong)GetItemCurrentActiveFromBag().itemData;
            if (tt == null) yield break;

            startIndex = listThanThongData.IndexOf(tt) + 1;
            if (startIndex < listThanThongData.Count - 1)
            {
                InitItemBag(5, ref itemInven, isAdd);
            }


        }
        else
        {
            InitItemBag(listThanThongData.Count, ref itemInven, isAdd);

        }
        //equip
        for (int i = 0; i < itemInven.Count; i++)
        {// TẠO BIẾN TẠM Ở ĐÂY
            int index = i+startIndex;
            // Cứ mỗi 10 item data thì nghỉ 1 frame
            if (i % countDelay == 0) yield return null;
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
                                    return; // Thoát ra, không chạy code Init bên dưới
                                }
                                Debug.Log("Item slot sẵn sàng");

                                // Sử dụng biến tạm 'currentItem' hoặc 'index' thay vì 'i'
                                InitVisualItemSkill(itemSlot, thanThong);
                                //equip
                                //congPhap.EquipCongPhap();
                                //save player thần thông equip id
                                Observer.Notify(EventList.Network.EQUIP_THANTHONG, this, listItemSlotThanThongs.IndexOf(currentItem), thanThong.Id);
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
    [Header("--- Inventory ---")]
    [SerializeField] private List<ItemInventory> listItemSlotEquipment;
    [SerializeField] private CharacterGraphic charPageInventory;
    [SerializeField] private Sprite spriteTTInven;
    [SerializeField] private Sprite spriteCPInven;
    private Coroutine coroutineInitVatPham;

    private void InitVisualItemInventory(ItemInventory item, IItemData data, ItemRef itemRef)
    {
        Sprite frame = null;
        if (data != null)
        {
            var quality = LuyenDanResources.Instance.FindQuality(data.Tier);
            frame = quality.IconUp;

            item.SetColorName(quality.Color);
            item.SetQualityFull(quality.Background);
            item.Init(new BaseItemData { nameItem = data.Name, spriteItem = data.Icon }, frame);
        }
        if (itemRef != null)
        {
            item.InitCount(itemRef.Count);
            if (itemRef.Type == ItemType.ThanThong)
            {
                item.InitIcon(spriteTTInven, frame);
            }
            if ( itemRef.Type == ItemType.CongPhap)
            {
                item.InitIcon(spriteCPInven, frame);
            }
        }    
       

        item.InitInventory(ETypeInventoryItem.Item,itemRef,data);



    }
    public void InitInventory(int countDelay = 10)
    {
        var GameConfig = Pool.Get<GameConfig>();
      
        // load data equipment 
        for (int i = 0; i < listItemSlotEquipment.Count; i++)
        {
            //var item = listItemSlotThanThongs[i];
            //ThanThong thanThongData = null;
            //if (i < Player.characterStatsBase.thanThongEquip.Count)
            //{
            //    thanThongData = GameConfig.GetThanThongWithId(Player.characterStatsBase.thanThongEquip[i]);
            //}

           // InitVisualItemSkill(item, thanThongData);
        }
        // unEquip Acion
        foreach (var currentItem in listItemSlotEquipment)
        {
            //currentItem.InitAction(null, null, () =>
            //{
            //    Debug.Log("có vào đây");
            //    if (currentItem.itemData == null)
            //    {
            //        Debug.Log("data null");

            //        return;
            //    }

            //    itemSelect.Show(currentItem.itemData, new List<RectTransform> { bag.GetComponent<RectTransform>() }, (resultRect) =>
            //    {
            //        if (resultRect != null)
            //        {
            //            Observer.Notify(EventList.Network.UNEQUIP_THANTHONG, listItemSlotThanThongs.IndexOf(currentItem), this);
            //            Debug.Log("updaet thần thông id unequip");
            //            // Cách viết chuẩn


            //        }
            //        else
            //        {
            //            Debug.Log("Thả trượt rồi!");

            //            InitVisualItemSkill(currentItem, (BaseSkill)itemSelect.itemData);

            //        }
            //    });
            //    InitVisualItemSkill(currentItem, null);

            //});
        }
        if (coroutineInitVatPham != null)
        {
            StopCoroutine(coroutineInitVatPham);
            coroutineInitVatPham = null;
        }
        coroutineInitVatPham = StartCoroutine(LoadBagVatPham(countDelay));

        charPageInventory.SetGender(Client.Instance.CharacterData.Gender);
        charPageInventory.SetSkin(Client.Instance.CharacterData.Skins);

    }
    private IEnumerator LoadBagVatPham(int countDelay = 10, bool isAdd = false)
    {
        var characterData = Client.Instance.CharacterData;

        var GameConfig = Pool.Get<GameConfig>();
        int startIndex = 0;

        List<ItemInventory> itemInven = new List<ItemInventory>();
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
        int totalItem = itemDatas.Count;

        if (isAdd)
        {

            ItemRef refItem = GetItemCurrentActiveFromBag().itemRef;
            if (refItem == null)
            {
                Debug.Log("ref item null");
                yield break;
            }

            startIndex = itemDatas.IndexOf(refItem) + 1;
            if (startIndex < itemDatas.Count - 1)
            {
                InitItemBag(5, ref itemInven,isAdd);
            }


        }
        else
        {
            InitItemBag(totalItem, ref itemInven,isAdd);

        }
        Debug.Log("co vao load bag vat pham " + $"{itemInven.Count}" + "start index: " + startIndex);

        //set item inventỏy và action
        for (int i = 0; i < itemInven.Count; i++)
        {// TẠO BIẾN TẠM Ở ĐÂY
            int index = i + startIndex;
            if (i % countDelay == 0) yield return null;
            var currentItem = itemInven[i];
            IItemData data = null;
            ItemRef itemRef = null;
            if (i < itemDatas.Count)
            {
                itemRef = itemDatas[index];

                data = LuyenDanResources.Instance.FindItem(itemRef);



            }
            if (data == null) // tức là k tìm đc hoặc nó là thanThong hoặc là CongPhap
            {
                if (itemRef != null)
                {
                    if (itemRef.Type == ItemType.ThanThong)
                    {
                        InitVisualItemSkill(currentItem, GameConfig.FindThanThongWithItemRef(itemRef), ETypeInventoryItem.Item);


                    }
                    else if (itemRef.Type == ItemType.CongPhap)
                    {
                        InitVisualItemSkill(currentItem, GameConfig.FindCongPhapWithItemRef(itemRef), ETypeInventoryItem.Item);

                    }
                }
            }
            InitVisualItemInventory(currentItem, data, itemRef);


            if (data != null)
            {

                currentItem.InitAction(null, null, () =>
                {


                    itemSelect.Show(currentItem, listItemSlotThanThongs.ConvertAll(x => x.GetComponent<RectTransform>()), (resultRect) =>
                    {

                    });
                    //foreach (var item in listItemSlotThanThongs)
                    //{
                    //    if (item.itemData != null)
                    //    {
                    //        DisableHighlight(item);
                    //    }
                    //}



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
