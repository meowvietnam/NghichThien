using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using System.Collections.Generic;

public class InventoryUIFacade
{
    private readonly UIManager uiManager;
    public List<ItemInventory> listItemUI;

    public InventoryUIFacade()
    {
        uiManager = Pool.Get<UIManager>();
        listItemUI = new List<ItemInventory>();
    }

    public void RenderBagItem(ItemInventory currentItem, ItemRef itemRef, bool isIconSkillBase,
        EActionShowItemInfor eActionShowItemInfor = EActionShowItemInfor.None)
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
        handler.Handle(currentItem, itemRef, isIconSkillBase, eActionShowItemInfor);
    }
}