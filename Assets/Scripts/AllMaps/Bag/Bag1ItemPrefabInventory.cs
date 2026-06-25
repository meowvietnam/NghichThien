using Base;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using LuyenKhi;
using System;
using System.Collections;
using System.Collections.Generic;
using LongDHModules;
using UnityEngine;
using UnityEngine.UI;

public class Bag1ItemPrefabInventory : BaseBehaviour
{
    public bool isLoadingMore = false;
    
   
    [SerializeField] private ItemInventory prefabItemBag;
    [SerializeField] private Sprite[] spriteQualityFulls => Pool.Get<UIManager>().spriteQualityFulls;
    [SerializeField] private Sprite[] spriteFrames => Pool.Get<UIManager>().spriteFrames;
    [SerializeField] private Color[] colorQualitys => Pool.Get<UIManager>().colorQualitys;
    public ScrollRect scrollRectBagUpdate => prefabItemBag.GetComponentInParent<ScrollRect>();
    public List<ItemRef> itemFullDatas = new List<ItemRef>();
    public List<ItemRef> itemDataShows= new List<ItemRef>();
    public Action<ItemInventory> actionItemInventory;
    public bool isIconSkillBase = false;
    public FilterControl filterControl;

    private InventoryUIFacade inventoryUIFacade;
    public InventoryUIFacade InventoryUIFacade => inventoryUIFacade ?? (inventoryUIFacade = new InventoryUIFacade());
    void OnScrollChanged(Vector2 value, Action<bool> loadBagFunc)
    {
        Debug.Log("co vao onscrooll change");

        if (value.y < 0.05f && !isLoadingMore)
        {
            // Ki?m tra xem d? li?u còn ?? load không tr??c khi g?i
            // (Ví d?: compare s? l??ng UI hi?n có v?i t?ng s? data th?t)
            StartCoroutine(TriggerLoadMore(loadBagFunc));
        }
    }
    private void OnEnable()
    {
        isLoadingMore = false;
        scrollRectBagUpdate.onValueChanged.RemoveAllListeners();
        scrollRectBagUpdate.onValueChanged.AddListener((vec) => {
            OnScrollChanged(vec, InitBag);
        });

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
        isLoadingMore = true;
        Debug.Log("co vao trigger load more");

        // G?i tr?c ti?p hàm Coroutine và ch? nó ch?y xong
        loadBagFunc?.Invoke(true);

        // ??i m?t chút ?? tránh vi?c g?i liên t?c khi user gi? chu?t ? ?áy
        yield return new WaitForSeconds(0.5f);
        isLoadingMore = false;
    }
  
    public void InitDataItem(List<ItemRef> itemDatas , Action<ItemInventory> actionItemInventory , bool isIconSkillBase = false)
    {
        this.itemFullDatas = itemDatas;
        this.itemDataShows = itemDatas;
        this.actionItemInventory = actionItemInventory;
        this.isIconSkillBase = isIconSkillBase;
        this.InventoryUIFacade.listItemUI.Clear();

    }
    public void ShowTextCount(bool isShow)
    {
        foreach (var item in this.InventoryUIFacade.listItemUI)
        {
            item.textCount.gameObject.SetActive(isShow);
        }    
    }    
    public void InitBag(bool isAdd=false)
    {
        var gameConfig = Pool.Get<GameManager>();

        if (!isAdd)
        {
            for (int i = 0; i < prefabItemBag.transform.parent.childCount; i++)
            {
                prefabItemBag.transform.parent.GetChild(i).gameObject.SetActive(false);
            }
        }
        int totalLoad = 30;
        int countAdd = 10;
        int currentLoad = itemDataShows.Count;
        int startIndex = 0;

        if (!isAdd)
        {
            if (currentLoad >= totalLoad)
            {
                currentLoad = totalLoad;
            }
        }
        else
        {
            currentLoad = countAdd;
            var currentItem = GetItemCurrentActiveFromBag(prefabItemBag.gameObject);
            if (currentItem == null) return;
            ItemRef refItem = currentItem.itemRef;
            if (refItem == null)
            {
                Debug.Log("ref item null");
                return;
            }

            startIndex = itemDataShows.IndexOf(refItem) + 1;
            if (startIndex >= itemDataShows.Count)
            {
                return;
            }

        }
        for (int i = 0; i < currentLoad; i++)
        {// TẠO BIẾN TẠM Ở ĐÂY
            int index = i + startIndex;
            var currentItem = GameManager.GetItemPoolWithPrefabInParent(prefabItemBag);
            ItemRef itemRef = null;
            if (index >= itemDataShows.Count)
            {
                break;
            }
            if (i < itemDataShows.Count)
            {
                itemRef = itemDataShows[index];
            }

            InventoryUIFacade.RenderBagItem(currentItem, itemRef, isIconSkillBase);
            actionItemInventory?.Invoke(currentItem);


        }
    }
}
