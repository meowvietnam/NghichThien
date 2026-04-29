using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using LuyenDan;
using Base;
using System.Linq;
using LongDH.LongDHModules;
using DG.Tweening;
using System.Collections;
using LuyenKhi;
using LuyenDan.Inventory;
using Network;

public class FilterControl : BaseBehaviour
{
    public static string SELECT_FILTER => "SELECT_FILTER";
    public static string DONE_FILTER => "DONE_FILTER";

    public static string GetTextItemType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Herb:

                return "Dược Liệu";
            case ItemType.Furnace:
                return "Đan Lô";
            case ItemType.LinhDan:
                return "Đan Dược";
            case ItemType.ThanThong:
                return "Thần Thông";
            case ItemType.CongPhap:
                return "Công Pháp";

            case ItemType.Recipe:
                return "Đan Phương";
            case ItemType.Material:
                return "Vật Liệu";
            case ItemType.VuKhi:
                return "Vũ Khí";
            case ItemType.PhongCu:
                return "Phòng Cụ";
            case ItemType.TrangSuc:
                return "Trang Sức";
            default:
                break;
        }

        return "";
    }


    [SerializeField] private Button filterLeftA; // loại
    [SerializeField] private Button filterLeftB; // phẩm cấp
    [SerializeField] private List<Button> filtersRight;
    [SerializeField] private Animator filterRightAnim;
    [SerializeField] private ItemType typeCurrent;
    [SerializeField] private bool isAllFilterA = true;
    [SerializeField] private bool isAllFilterB = true;
    [SerializeField] private Button currentFilterLeftSelect;
    [SerializeField] private Tier tierCurrent;
    [SerializeField] private BaseSkill.ETierSkill eTierSkillCurrent;
    [SerializeField] private List<ItemType> listFilterType = new List<ItemType>();
    [SerializeField] private EquipmentTier eTierEquipmentCurrent;

    private void OnEnable()
    {
        Init();
        Observer.AddObserver(DONE_FILTER, OnDoneFilter);

    }
    private void OnDisable()
    {
        Observer.RemoveObserver(DONE_FILTER, OnDoneFilter);
    }
    private void OnDoneFilter()
    {
        filterRightAnim.gameObject.SetActive(false);

    }
    public void InitFilterType(List<ItemType> listFilterType)
    {
        this.listFilterType = listFilterType;
        Init();
    }

    public void InitAllFilterType()
    {
        InitFilterType(new List<ItemType>
        {
            ItemType.Herb,
            ItemType.Furnace,
            ItemType.LinhDan,
            ItemType.ThanThong,
            ItemType.CongPhap,
            ItemType.Recipe,
            ItemType.Material,
            ItemType.VuKhi,
            ItemType.PhongCu,
            ItemType.TrangSuc,


        });
    }

    public void Init()
    {
        filtersRight[0].transform.GetChild(0).GetComponent<Text>().text = "Toàn bộ";

        SetEventButton(filterLeftA, () =>
        {
            Observer.Notify(SELECT_FILTER);

            for (int i = 1; i < filtersRight.Count; i++)
            {
                int index = i;
                filtersRight[index].gameObject.SetActive(false);
                SetEventButton(filtersRight[index], () =>
                {
                    int indexType = index - 1;
                    if (indexType <= listFilterType.Count - 1)
                    {
                        if (typeCurrent == listFilterType[indexType] && !isAllFilterA)
                        {
                            Observer.Notify(DONE_FILTER);
                            return;
                        }
                        typeCurrent = listFilterType[indexType];
                    }
                       
                    filterLeftB.gameObject.SetActive(true);
                    isAllFilterA = false;
                   

                    filterLeftA.transform.GetChild(0).GetComponent<Text>().text =
                        filtersRight[index].transform.GetChild(0).GetComponent<Text>().text;
                    filterRightAnim.gameObject.SetActive(false);
                    isAllFilterB = true;
                    filterLeftB.transform.GetChild(0).GetComponent<Text>().text = filtersRight[0].transform.GetChild(0).GetComponent<Text>().text;
                    Filter();
                });
            }

            for (int i = 0; i < listFilterType.Count; i++)
            {
                filtersRight[i + 1].gameObject.SetActive(true);
                filtersRight[i + 1].transform.GetChild(0).GetComponent<Text>().text =
                    GetTextItemType(listFilterType[i]);
            }

            filterRightAnim.gameObject.SetActive(false);

            filterRightAnim.gameObject.SetActive(true);
            filtersRight[0].gameObject.SetActive(true);

            currentFilterLeftSelect = filterLeftA;
        });
        StartCoroutine(DelayFrame());

        List<Tier> listTier = new List<Tier>
        {
            Tier.NhatPham,
            Tier.NhiPham,
            Tier.TamPham,
            Tier.TuPham,
            Tier.NguPham,
            Tier.LucPham
        };
        List<BaseSkill.ETierSkill> listTierSkill = new List<BaseSkill.ETierSkill>
        {
            BaseSkill.ETierSkill.NhanGiaiHa,
            BaseSkill.ETierSkill.NhanGiaiTrung,
            BaseSkill.ETierSkill.NhanGiaiThuong,
            BaseSkill.ETierSkill.DiaGiaiHa,
            BaseSkill.ETierSkill.DiaGiaiTrung,
            BaseSkill.ETierSkill.DiaGiaiThuong,
            BaseSkill.ETierSkill.ThienGiaiHa,
            BaseSkill.ETierSkill.ThienGiaiTrung,
            BaseSkill.ETierSkill.ThienGiaiThuong
        };
        List<EquipmentTier> listTierEquipment = new List<EquipmentTier>
        {
           EquipmentTier.PhuKhi,
           EquipmentTier.PhapKhi,
           EquipmentTier.PhapBao,
           EquipmentTier.ThuanDuong,
           EquipmentTier.ThongThien,

        };
        SetEventButton(filterLeftB, () =>
        {
            Observer.Notify(SELECT_FILTER);

            for (int i = 1; i < filtersRight.Count; i++)
            {
                int index = i;
                filtersRight[index].gameObject.SetActive(false);
                SetEventButton(filtersRight[index], () =>
                {
                    int indexType = index - 1;

                    if (typeCurrent == ItemType.ThanThong || typeCurrent == ItemType.CongPhap)
                    {
                        
                        if (indexType <= listTierSkill.Count - 1)
                        {
                            if (eTierSkillCurrent == listTierSkill[indexType] && !isAllFilterB)
                            {
                                Observer.Notify(DONE_FILTER);
                                return;
                            }
                            eTierSkillCurrent = listTierSkill[indexType];
                        }
                    }
                    else if (typeCurrent == ItemType.VuKhi || typeCurrent == ItemType.PhongCu || typeCurrent == ItemType.TrangSuc)
                    {
                        if (indexType <= listTierSkill.Count - 1)
                        {
                            if (eTierEquipmentCurrent == listTierEquipment[indexType] && !isAllFilterB)
                            {
                                Observer.Notify(DONE_FILTER);
                                return;
                            }
                            eTierEquipmentCurrent = listTierEquipment[indexType];
                        }
                    }
                    else
                    {
                        if (indexType <= listTier.Count - 1)
                        {
                            if (tierCurrent == listTier[indexType] && !isAllFilterB)
                            {
                                Observer.Notify(DONE_FILTER);
                                return;
                            }
                            tierCurrent = listTier[indexType];
                        }
                    }
                    isAllFilterB = false;

                    filterLeftB.transform.GetChild(0).GetComponent<Text>().text =
                        filtersRight[index].transform.GetChild(0).GetComponent<Text>().text;
                    filterRightAnim.gameObject.SetActive(false);
                    Filter();
                });
            }
            int countFilter = 0;
            if (typeCurrent == ItemType.ThanThong || typeCurrent == ItemType.CongPhap)
            {
                countFilter = listTierSkill.Count;

            }
            else if (typeCurrent == ItemType.VuKhi || typeCurrent == ItemType.PhongCu || typeCurrent == ItemType.TrangSuc)
            {
                countFilter = listTierEquipment.Count;

            }
            else
            {
                countFilter = listTier.Count;

            }
            

            for (int i = 0; i < countFilter; i++)
            {
                filtersRight[i + 1].gameObject.SetActive(true);
                if(typeCurrent == ItemType.ThanThong || typeCurrent == ItemType.CongPhap)
                {
                    filtersRight[i + 1].transform.GetChild(0).GetComponent<Text>().text = BaseSkill.GetTextTierSkill(listTierSkill[i]);

                }
                else if (typeCurrent == ItemType.VuKhi || typeCurrent == ItemType.PhongCu || typeCurrent == ItemType.TrangSuc)
                {
                    filtersRight[i + 1].transform.GetChild(0).GetComponent<Text>().text = GameManager.TextWithTierEquirement.GetValueOrDefault(listTierEquipment[i]);

                }
                else
                {
                    filtersRight[i + 1].transform.GetChild(0).GetComponent<Text>().text = GameManager.TextWithTier.GetValueOrDefault(listTier[i]);

                }
               
            }

            filterRightAnim.gameObject.SetActive(false);

            filterRightAnim.gameObject.SetActive(true);
            filtersRight[0].gameObject.SetActive(true);

            currentFilterLeftSelect = filterLeftB;
        });

        SetEventButton(filtersRight[0], () =>
        {
            
            if (currentFilterLeftSelect == filterLeftA)
            {
                if(isAllFilterA)
                {
                    Observer.Notify(DONE_FILTER);

                    return;
                }    
                isAllFilterA = true;
                filterLeftB.gameObject.SetActive(false);
            }
            else if (currentFilterLeftSelect == filterLeftB)
            {
                if (isAllFilterB)
                {
                    Observer.Notify(DONE_FILTER);

                    return;
                }
                isAllFilterB = true;
            }

            currentFilterLeftSelect.transform.GetChild(0).GetComponent<Text>().text =
                filtersRight[0].transform.GetChild(0).GetComponent<Text>().text;
            filterRightAnim.gameObject.SetActive(false);
            Filter();
        });
        filterLeftA.transform.GetChild(0).GetComponent<Text>().text =
            filtersRight[0].transform.GetChild(0).GetComponent<Text>().text;
        filterLeftB.transform.GetChild(0).GetComponent<Text>().text =
            filtersRight[0].transform.GetChild(0).GetComponent<Text>().text;
        filterLeftB.gameObject.SetActive(false);
    }

    IEnumerator DelayFrame()
    {
        yield return null;
        if (listFilterType.Count <= 1)
        {
            filterLeftA.onClick?.Invoke();
            filtersRight[1].onClick?.Invoke();
            filterLeftA.onClick?.RemoveAllListeners();
        }
    }

    public void Filter()
    {
        var bag = transform.GetComponentInParent<Bag1ItemPrefabInventory>();
        List<ItemRef> itemDataShows = new List<ItemRef>();
        if (bag != null)
        {
            Observer.Notify(DONE_FILTER);

            if (isAllFilterA)
            {
                // Nếu là True: Lấy toàn bộ danh sách gốc
                itemDataShows = bag.itemFullDatas;
            }
            else
            {
                // Nếu là False: Lọc danh sách theo typeCurrent
                if (isAllFilterB)
                {
                    itemDataShows = bag.itemFullDatas
                        .Where(item => item.Type == typeCurrent)
                        .ToList();
                }
                else
                {
                    itemDataShows = bag.itemFullDatas.Where(item =>
                    {
                        // Chỉ xét những item cùng Type trước
                        if (item.Type != typeCurrent) return false;

                        if (typeCurrent == ItemType.ThanThong)
                        {
                            var skill = Pool.Get<GameManager>().FindThanThongWithItemRef(item);
                            // Giả sử skill có biến .tierSkill kiểu ETierSkill
                            return skill != null && skill.eTierSkill == eTierSkillCurrent;
                        }
                        else if (typeCurrent == ItemType.CongPhap)
                        {
                            var skill = Pool.Get<GameManager>().FindCongPhapWithItemRef(item);
                            return skill != null && skill.eTierSkill == eTierSkillCurrent;
                        }
                        else if(typeCurrent == ItemType.VuKhi )
                        {
                            var vuKhi = Client.Instance.CharacterData.InventoryManager.VuKhis.FirstOrDefault(x => x.SelfId == item.Id);
                            return vuKhi != null && vuKhi.Item.Tier == eTierEquipmentCurrent;

                        }   
                        else if (typeCurrent == ItemType.PhongCu)
                        {
                            var phongCu = Client.Instance.CharacterData.InventoryManager.PhongCus.FirstOrDefault(x => x.SelfId == item.Id);
                            return phongCu != null && phongCu.Item.Tier == eTierEquipmentCurrent;
                        }   
                        else if (typeCurrent == ItemType.TrangSuc)
                        {
                            var trangSuc = Client.Instance.CharacterData.InventoryManager.TrangSucs.FirstOrDefault(x => x.SelfId == item.Id);
                            return trangSuc != null && trangSuc.Item.Tier == eTierEquipmentCurrent;
                        }    
                        else
                        {
                            IItemData itemData = null;

                            if (item.Type == ItemType.Material)
                            {
                                itemData = LuyenKhiResources.Instance.FindItem(item);
                            }    
                            else
                            {
                                itemData = LuyenDanResources.Instance.FindItem(item);
                            }    
                              
                            // Giả sử itemData có biến .tier kiểu Tier
                            return itemData != null && itemData.Tier == tierCurrent;
                        }
                    }).ToList();
                }
            }

            bag.itemDataShows = itemDataShows;
            bag.InitBag();
            // Sau khi lọc xong, bạn có thể gọi hàm cập nhật UI tại đây
            // UpdateInventoryUI(); 
        }
    }
}