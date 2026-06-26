using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;

public class InventoryUIFacade
{
    private readonly UIManager uiManager;
    public List<ItemInventory> listItemUI;

    public InventoryUIFacade()
    {
        uiManager = Pool.Get<UIManager>();
        listItemUI = new List<ItemInventory>();
    }

    public void RenderBagItem(ItemInventory currentItem, ItemRef itemRef, bool isIconSkillBase)
    {
        if (currentItem == null)
            return;

        if (itemRef == null)
        {
            currentItem.Init(null, null);
            currentItem.gameObject.SetActive(false);
            return;
        }
        listItemUI.Add(currentItem);
        var handler = ItemTypeHandlerBagFactory.Create(itemRef.Type);
        handler.Handle(currentItem, itemRef, isIconSkillBase);
    }
}
