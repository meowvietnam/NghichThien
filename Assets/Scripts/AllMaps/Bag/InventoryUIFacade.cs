using LuyenDan;
using LuyenDan.Inventory;
using UnityEngine;

public class InventoryUIFacade
{
    private readonly UIManager uiManager;

    public InventoryUIFacade()
    {
        uiManager = Pool.Get<UIManager>();
    }

    public void RenderBagItem(ItemInventory currentItem, ItemRef itemRef, bool isIconSkillBase)
    {
        if (currentItem == null)
            return;

        if (itemRef == null)
        {
            uiManager.InitVisualItemInventory(currentItem, null, null, isIconSkillBase);
            return;
        }

        var handler = ItemTypeHandlerFactory.Create(itemRef.Type);
        handler.Handle(currentItem, itemRef, isIconSkillBase);
    }
}
