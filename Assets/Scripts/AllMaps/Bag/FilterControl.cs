using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using LuyenDan;
using Base;
using System.Linq;
using LongDH.LongDHModules;
using DG.Tweening;
using System.Collections;

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

    private void OnEnable()
    {
        Init();
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
                    filterLeftB.gameObject.SetActive(true);
                    int indexType = index - 1;
                    isAllFilterA = false;
                    if (indexType <= listFilterType.Count - 1)
                    {
                        typeCurrent = listFilterType[indexType];
                    }

                    filterLeftA.transform.GetChild(0).GetComponent<Text>().text =
                        filtersRight[index].transform.GetChild(0).GetComponent<Text>().text;
                    filterRightAnim.gameObject.SetActive(false);
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

                    isAllFilterB = false;
                    if (typeCurrent == ItemType.ThanThong || typeCurrent == ItemType.CongPhap)
                    {
                        if (indexType <= listTierSkill.Count - 1)
                        {
                            eTierSkillCurrent = listTierSkill[indexType];
                        }
                    }
                    else
                    {
                        if (indexType <= listTier.Count - 1)
                        {
                            tierCurrent = listTier[indexType];
                        }
                    }

                    filterLeftB.transform.GetChild(0).GetComponent<Text>().text =
                        filtersRight[index].transform.GetChild(0).GetComponent<Text>().text;
                    filterRightAnim.gameObject.SetActive(false);
                    Filter();
                });
            }

            int countFilter = typeCurrent == ItemType.ThanThong || typeCurrent == ItemType.CongPhap
                ? listTierSkill.Count
                : listTier.Count;

            for (int i = 0; i < countFilter; i++)
            {
                filtersRight[i + 1].gameObject.SetActive(true);
                filtersRight[i + 1].transform.GetChild(0).GetComponent<Text>().text =
                    typeCurrent == ItemType.ThanThong || typeCurrent == ItemType.CongPhap
                        ? BaseSkill.GetTextTierSkill(listTierSkill[i])
                        : GameManager.TextWithTier.GetValueOrDefault(listTier[i]);
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
                isAllFilterA = true;
                filterLeftB.gameObject.SetActive(false);
            }
            else if (currentFilterLeftSelect == filterLeftB)
            {
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
                        else
                        {
                            var itemData = LuyenDanResources.Instance.FindItem(item);
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