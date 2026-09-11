using Base;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using LuyenKhi;
using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Bag1ItemPrefabInventory : BaseBehaviour
{
    private const int InitialLoadCount = 30;
    private const int LoadMoreCount = 10;

    public bool isLoadingMore = false;


    [SerializeField] private ItemInventory prefabItemBag;
    [SerializeField] private Sprite[] spriteQualityFulls => Pool.Get<UIManager>().spriteQualityFulls;
    [SerializeField] private Sprite[] spriteFrames => Pool.Get<UIManager>().spriteFrames;
    [SerializeField] private Color[] colorQualitys => Pool.Get<UIManager>().colorQualitys;
    public ScrollRect scrollRectBagUpdate => prefabItemBag.GetComponentInParent<ScrollRect>();
    public List<ItemRef> itemFullDatas = new List<ItemRef>();
    public List<ItemRef> itemDataShows = new List<ItemRef>();
    public Action<ItemInventory> actionItemInventory;
    public bool isIconSkillBase = false;
    public FilterControl filterControl;

    private InventoryUIFacade inventoryUIFacade;
    public InventoryUIFacade InventoryUIFacade => inventoryUIFacade ?? (inventoryUIFacade = new InventoryUIFacade());

    private bool isTriggerShowTextCount = false;
    private int loadedCount;
    private EActionShowItemInfor eActionShowItemInfor;

    void OnScrollChanged(Vector2 value, Action<bool> loadBagFunc)
    {
        if (value.y < 0.05f && !isLoadingMore && CanLoadMore())
        {
            StartCoroutine(TriggerLoadMore(loadBagFunc));
        }
    }

    private void OnEnable()
    {
        isLoadingMore = false;
        scrollRectBagUpdate.onValueChanged.RemoveAllListeners();
        scrollRectBagUpdate.onValueChanged.AddListener((vec) => { OnScrollChanged(vec, InitBag); });
    }

    private ItemInventory GetItemCurrentActiveFromBag(GameObject prefab)
    {
        for (int j = prefab.transform.parent.childCount - 1; j > 0; j--)
        {
            Transform child = prefab.transform.parent.GetChild(j);
            if (!child.gameObject.activeSelf)
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

    IEnumerator TriggerLoadMore(Action<bool> loadBagFunc)
    {
        if (!CanLoadMore())
        {
            yield break;
        }

        isLoadingMore = true;
        Debug.Log("co vao trigger load more");

        var beforeLoadCount = loadedCount;
        loadBagFunc?.Invoke(true);

        yield return new WaitForSeconds(0.5f);
        isLoadingMore = CanLoadMore() && loadedCount == beforeLoadCount;
    }

    private bool CanLoadMore()
    {
        return loadedCount < itemDataShows.Count;
    }

    public void SetItemDataShow(List<ItemRef> itemDatas)
    {
        var equipment = Client.Instance.CharacterData.EquipmentManager;
        ushort vuKhiEquip = equipment.VuKhi;
        ushort phongCuEquip = equipment.PhongCu;
        ushort trangSucEquip = equipment.TrangSuc;
        this.itemDataShows = itemDatas.Where(item =>
        {
            // Nếu là Vũ Khí và trùng ID đang mặc -> Loại bỏ
            if (item.Type == ItemType.VuKhi && item.Id == vuKhiEquip) return false;

            // Nếu là Phòng Cụ và trùng ID đang mặc -> Loại bỏ
            if (item.Type == ItemType.PhongCu && item.Id == phongCuEquip) return false;

            // Nếu là Trang Sức và trùng ID đang mặc -> Loại bỏ
            if (item.Type == ItemType.TrangSuc && item.Id == trangSucEquip) return false;

            // Các item khác hoặc không trùng ID thì giữ lại
            return true;
        }).ToList();
    }

    public void InitDataItem(List<ItemRef> itemDatas, Action<ItemInventory> actionItemInventory,
        bool isIconSkillBase = false, bool isShowTextCount = false,
        EActionShowItemInfor eActionShowItemInfor = EActionShowItemInfor.None)
    {
        this.itemFullDatas = itemDatas;


        SetItemDataShow(itemDatas);


        this.actionItemInventory = actionItemInventory;
        this.isIconSkillBase = isIconSkillBase;
        this.InventoryUIFacade.listItemUI.Clear();
        this.isTriggerShowTextCount = isShowTextCount;
        this.eActionShowItemInfor = eActionShowItemInfor;
    }

    public void InitBag(bool isAdd = false)
    {
        var gameConfig = Pool.Get<GameManager>();

        if (!isAdd)
        {
            for (int i = 0; i < prefabItemBag.transform.parent.childCount; i++)
            {
                prefabItemBag.transform.parent.GetChild(i).gameObject.SetActive(false);
            }

            loadedCount = 0;
            InventoryUIFacade.listItemUI.Clear();
        }

        if (!CanLoadMore())
        {
            isLoadingMore = false;
            return;
        }

        var startIndex = loadedCount;
        var currentLoad = Math.Min(isAdd ? LoadMoreCount : InitialLoadCount, itemDataShows.Count - startIndex);
        var renderedCount = 0;

        for (int i = 0; i < currentLoad; i++)
        {
            int index = i + startIndex;
            var currentItem = GameManager.GetItemPoolWithPrefabInParent(prefabItemBag);
            if (index >= itemDataShows.Count)
            {
                break;
            }

            var itemRef = itemDataShows[index];

            InventoryUIFacade.RenderBagItem(currentItem, itemRef, isIconSkillBase, eActionShowItemInfor);
            if (isTriggerShowTextCount)
            {
                currentItem.textCount.gameObject.SetActive(true);
            }

            actionItemInventory?.Invoke(currentItem);
            renderedCount++;
        }

        loadedCount += renderedCount;
        if (!isAdd)
        {
            isLoadingMore = false;
        }
    }
}