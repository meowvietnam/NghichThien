using System.Collections.Generic;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using LuyenKhi;
using UnityEngine;

public interface IBagItemTypeHandler
{
    void Handle(ItemInventory currentItem, ItemRef itemRef, bool isIconSkillBase,
        EActionShowItemInfor eActionShowItemInfor = EActionShowItemInfor.None);
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
    public void Handle(ItemInventory currentItem, ItemRef itemRef, bool isIconSkillBase,
        EActionShowItemInfor eActionShowItemInfor = EActionShowItemInfor.None)
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
        uiManager.InitVisualItemInventory(currentItem, null, itemRef, isIconSkillBase, eActionShowItemInfor);
    }
}

public class EquipmentBagItemTypeHandler : IBagItemTypeHandler
{
    public void Handle(ItemInventory currentItem, ItemRef itemRef, bool isIconSkillBase,
        EActionShowItemInfor eActionShowItemInfor = EActionShowItemInfor.None)
    {
        var uiManager = Pool.Get<UIManager>();
        uiManager.InitVisualEquipment(currentItem, null, itemRef);
        uiManager.InitVisualItemInventory(currentItem, null, itemRef, isIconSkillBase, eActionShowItemInfor);
    }
}

public class LuyenKhiBagItemTypeHandler : IBagItemTypeHandler
{
    public void Handle(ItemInventory currentItem, ItemRef itemRef, bool isIconSkillBase,
        EActionShowItemInfor eActionShowItemInfor = EActionShowItemInfor.None)
    {
        var uiManager = Pool.Get<UIManager>();
        var data = LuyenKhiResources.Instance.FindItem(itemRef);
        uiManager.InitVisualItemInventory(currentItem, data, itemRef, isIconSkillBase, eActionShowItemInfor);
    }
}

public class LuyenDanBagItemTypeHandler : IBagItemTypeHandler
{
    public void Handle(ItemInventory currentItem, ItemRef itemRef, bool isIconSkillBase,
        EActionShowItemInfor eActionShowItemInfor = EActionShowItemInfor.None)
    {
        var uiManager = Pool.Get<UIManager>();
        if(itemRef.Type == ItemType.Furnace)
        {
            if (itemRef.TempId == 0)
            {
                itemRef = new ItemRef
                {
                    Id = itemRef.Id,
                    TempId = itemRef.Id,
                    Count = itemRef.Count,
                    Type = ItemType.Furnace
                };
            }
        }    
        var data = LuyenDanResources.Instance.FindItem(itemRef);
        uiManager.InitVisualItemInventory(currentItem, data, itemRef, isIconSkillBase, eActionShowItemInfor);
    }
}
