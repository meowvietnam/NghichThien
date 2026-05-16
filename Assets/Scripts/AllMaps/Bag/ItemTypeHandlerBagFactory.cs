using System.Collections.Generic;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using LuyenKhi;
using UnityEngine;

public interface IBagItemTypeHandler
{
    void Handle(ItemInventory currentItem, ItemRef itemRef, bool isIconSkillBase);
}

public static class ItemTypeHandlerBagFactory
{
    public static IBagItemTypeHandler Create(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.ThanThong => new SkillBagItemTypeHandler(),
            ItemType.CongPhap => new SkillBagItemTypeHandler(),
            ItemType.VuKhi => new EquipmentBagItemTypeHandler(),
            ItemType.PhongCu => new EquipmentBagItemTypeHandler(),
            ItemType.TrangSuc => new EquipmentBagItemTypeHandler(),
            ItemType.Material => new LuyenKhiBagItemTypeHandler(),
            _ => new LuyenDanBagItemTypeHandler(),
        };

    }
}

public class SkillBagItemTypeHandler : IBagItemTypeHandler
{
    public void Handle(ItemInventory currentItem, ItemRef itemRef, bool isIconSkillBase)
    {
        var uiManager = Pool.Get<UIManager>();
        var gameManager = Pool.Get<GameManager>();
        BaseSkill data = null;

        if (itemRef.Type == ItemType.ThanThong)
        {
            data = gameManager.FindThanThongWithItemRef(itemRef);
        }
        else if (itemRef.Type == ItemType.CongPhap)
        {
            data = gameManager.FindCongPhapWithItemRef(itemRef);
        }

        uiManager.InitVisualItemSkill(currentItem, data, itemRef);
        uiManager.InitVisualItemInventory(currentItem, null, itemRef, isIconSkillBase);
    }
}

public class EquipmentBagItemTypeHandler : IBagItemTypeHandler
{
    public void Handle(ItemInventory currentItem, ItemRef itemRef, bool isIconSkillBase)
    {
        var uiManager = Pool.Get<UIManager>();
        uiManager.InitVisualEquipment(currentItem, null, itemRef);
        uiManager.InitVisualItemInventory(currentItem, null, itemRef, isIconSkillBase);
    }
}

public class LuyenKhiBagItemTypeHandler : IBagItemTypeHandler
{
    public void Handle(ItemInventory currentItem, ItemRef itemRef, bool isIconSkillBase)
    {
        var uiManager = Pool.Get<UIManager>();
        var data = LuyenKhiResources.Instance.FindItem(itemRef);
        uiManager.InitVisualItemInventory(currentItem, data, itemRef, isIconSkillBase);
    }
}

public class LuyenDanBagItemTypeHandler : IBagItemTypeHandler
{
    public void Handle(ItemInventory currentItem, ItemRef itemRef, bool isIconSkillBase)
    {
        var uiManager = Pool.Get<UIManager>();
        var data = LuyenDanResources.Instance.FindItem(itemRef);
        uiManager.InitVisualItemInventory(currentItem, data, itemRef, isIconSkillBase);
    }
}
