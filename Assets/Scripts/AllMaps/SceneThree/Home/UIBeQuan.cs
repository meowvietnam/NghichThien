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
                    InitPageUpdate(false);
                    scrollRectBagUpdate.onValueChanged.RemoveAllListeners();
                    scrollRectBagUpdate.onValueChanged.AddListener((vec) => {
                        OnScrollChanged(vec, InitPageUpdate);
                    });
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
    
    public bool isLoadingMore = false;
    void OnScrollChanged(Vector2 value, Action<bool> loadBagFunc)
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
    IEnumerator TriggerLoadMore(Action<bool> loadBagFunc)
    {
        isLoadingMore = true;
        Debug.Log("co vao trigger load more");

        // Gọi trực tiếp hàm Coroutine và chờ nó chạy xong
        loadBagFunc?.Invoke(true);

        // Đợi một chút để tránh việc gọi liên tục khi user giữ chuột ở đáy
        yield return new WaitForSeconds(0.5f);
        isLoadingMore = false;
    }
    private ItemInventory GetItemCurrentActiveFromBag(GameObject prefab)
    {
        for (int j = prefab.transform.parent.childCount - 1; j > 0; j--)
        {
            Transform child = prefab.transform.parent.GetChild(j);
            if(!child.gameObject.activeSelf)
            {
                continue;
            }    
            var currentItem = child.GetComponent<ItemInventory>();
            if (currentItem.itemData != null)
            {
                Debug.Log("GetItemCurrentActiveFromBag: " + currentItem.itemData.nameItem);
                return currentItem;
            }
        }
        return null;
    }
    [Header("Lĩnh Hội")]
    [SerializeField] private ItemInventory prefabItemInvenPanelLinhHoi;

    [SerializeField] private Text priceTextLinhHoi;
    [SerializeField] private Text textTimer;
    [SerializeField] private Text textInfor;
    [SerializeField] private Button btnLinhHoi;
    [SerializeField] private Button btnUnLinhHoi;
    [SerializeField] private Text textNameLinhHoi;
    [SerializeField] private Bag1ItemPrefabInventory bagLinhHoi;
    public ScrollRect scrollRectBagLinhHoi => prefabItemInvenPanelLinhHoi.GetComponentInParent<ScrollRect>();

 
    private void InitPageLinhNgo()
    {
        //reset 
     
        textNameLinhHoi.transform.parent.gameObject.SetActive(false);
        
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

        // 1. Tạo danh sách chứa cặp (SkillData và ItemRef)

       
        var listItemRefSkill = new List<ItemRef>();
       listItemRefSkill.AddRange(filteredThanThong);
       listItemRefSkill.AddRange(filteredCongPhap);

        bagLinhHoi.InitDataItem(listItemRefSkill, (item) => {
            item.InitAction(null, null, () =>
            {
                var data = item.itemData;
                var itemRef = item.itemRef;
                textNameLinhHoi.text = data.nameItem;
                textNameLinhHoi.transform.parent.gameObject.SetActive(true);

                int cost = (int)GameConfig.GetPriceItemBuy(itemRef, null) / 2;
                priceTextLinhHoi.text = $"{cost}";
                if (itemRef.Type == ItemType.ThanThong)
                {
                    ThanThong thanThong = (ThanThong)data;
                    textInfor.text = thanThong.GetDesc(ThanThong.GetDaiCanhGioi(Client.Instance.CharacterData.Level));
                }
                else if (itemRef.Type == ItemType.CongPhap)
                {
                    CongPhap cp = (CongPhap)data;
                    int daiCanhGioiCP = Client.Instance.CharacterData.CongPhapOwner.GetValueOrDefault(itemRef.Id);
                    if (daiCanhGioiCP == 0)
                    {
                        daiCanhGioiCP = 1;
                    }
                    textInfor.text = cp.GetDesc(daiCanhGioiCP);


                }
                SetEventButton(btnLinhHoi, () =>
                {
                    if (Client.Instance.CharacterData.Stats.LinhThach >= cost)
                    {
                        Observer.Notify(EventList.Network.LINH_NGO, this, itemRef.Id, itemRef.Type);
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
           
        });
        bagLinhHoi.InitBag(false);
      


    }
    [Header("Nâng cấp Công Pháp")]
    [SerializeField] private ItemInventory prefabItemInvenPanelUpdateCongPhap;

    [SerializeField] private Text priceTextUpdateCongPhap;
    [SerializeField] private ItemInventory priceItemCongPhap;
    [SerializeField] private Text textTimerUpdateCongPhap;
    [SerializeField] private Text[] textInforUpdateCongPhap;
    [SerializeField] private Button btnUpdateCongPhap;
    [SerializeField] private Button btnUnUpdateCongPhap;
    [SerializeField] private Text textNameDotPha;
    [SerializeField] private Bag1ItemPrefabInventory bagCongPhap;

    public ScrollRect scrollRectBagUpdate => prefabItemInvenPanelUpdateCongPhap.GetComponentInParent<ScrollRect>();

    private void InitPageUpdate(bool isAdd)
    {
        //reset 
        if (!isAdd)
        {
            for (int i = 0; i < prefabItemInvenPanelUpdateCongPhap.transform.parent.childCount; i++)
            {
                prefabItemInvenPanelUpdateCongPhap.transform.parent.GetChild(i).gameObject.SetActive(false);
            }
            textNameDotPha.transform.parent.gameObject.SetActive(false);

        }


        var dictCPOwner = Client.Instance.CharacterData.CongPhapOwner;

        var gameConfig = Pool.Get<GameConfig>();

        // 1. Tạo danh sách chứa cặp (SkillData và ItemRef)
        List<CongPhap> listSkillsWithRef = new List<CongPhap>();
        List<ItemRef> listSkillsWithItemRef = new List<ItemRef>();

        foreach (var item in dictCPOwner)
        {
            CongPhap skill = gameConfig.GetCongPhapWithId(item.Key);

            listSkillsWithRef.Add(skill); // Lưu cả 2 vào cặp
            listSkillsWithItemRef.Add(new ItemRef { Id = skill.Id, Count = 1, Type = ItemType.CongPhap });
        
        }

        

      
        bagCongPhap.InitDataItem(listSkillsWithItemRef, (item) =>
        {
            item.InitAction(null, null, () =>
            {
                var data=item.itemData;
                var itemRef = item.itemRef;

                textNameDotPha.transform.parent.gameObject.SetActive(true);

                textNameDotPha.text = data.nameItem;

                Pool.Get<UIManager>().InitVisualItemSkill(priceItemCongPhap, (BaseSkill)data, itemRef);
                int daiCanhGioiCP = Client.Instance.CharacterData.CongPhapOwner.GetValueOrDefault(itemRef.Id);
                if (daiCanhGioiCP == 0)
                {
                    daiCanhGioiCP = 1;
                }
                int cost = (int)GameConfig.GetPriceItemBuy(itemRef, null) * (daiCanhGioiCP + 1);
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
                    if (Client.Instance.CharacterData.Stats.LinhThach >= cost && Client.Instance.CharacterData.InventoryManager.CongPhaps.Count(x => x.Id == itemRef.Id) >= cost2)
                    {
                        Observer.Notify(EventList.Network.DOT_PHA_CONGPHAP, this, itemRef.Id, itemRef.Type);
                    }
                });
                SetEventButton(btnUnUpdateCongPhap, () =>
                {
                    if (Client.Instance.CharacterData.Stats.LinhThach < cost)
                    {
                        NotifyGroup.Instance.Show("Không đủ linh thạch");
                    }
                    else if (Client.Instance.CharacterData.InventoryManager.CongPhaps.Count(x => x.Id == itemRef.Id) < cost2)
                    {
                        NotifyGroup.Instance.Show("Không đủ số lượng công pháp yêu cầu");

                    }

                });
                SetUpBtnUpdate();
                void SetUpBtnUpdate()
                {
                    if (Client.Instance.CharacterData.Stats.LinhThach < cost || Client.Instance.CharacterData.InventoryManager.CongPhaps.Count(x => x.Id == itemRef.Id) < cost2)
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

