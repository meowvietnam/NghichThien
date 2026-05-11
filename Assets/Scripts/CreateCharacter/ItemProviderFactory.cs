using System;
using System.Collections.Generic;
using AllMaps.TabPanel.ToolTipDisplayers;
using LuyenDan;
using Network;

namespace CreateCharacter
{
    /// <summary>
    /// Factory class để tạo IEquipmentProvider theo ItemType
    /// Loại bỏ if-else chain trong UIManager
    /// </summary>
    public static class ItemProviderFactory
    {
        public static IEquipmentProvider GetEquipmentProvider(ItemType itemType)
        {
            InventoryManager inventoryManager = Client.Instance.CharacterData.InventoryManager;
            return itemType switch
            {
                ItemType.VuKhi => new VuKhiEquipmentProvider(inventoryManager),
                ItemType.PhongCu => new PhongCuEquipmentProvider(inventoryManager),
                ItemType.TrangSuc => new TrangSucEquipmentProvider(inventoryManager),
                _ => null,
            };

        }
        public static ItemDisplayer GetDisplay(ItemType itemType)
        {
            InventoryManager inventoryManager = Client.Instance.CharacterData.InventoryManager;
            return itemType switch
            {
                ItemType.VuKhi => new VuKhiEquipmentProvider(inventoryManager),
                ItemType.PhongCu => new PhongCuEquipmentProvider(inventoryManager),
                ItemType.TrangSuc => new TrangSucEquipmentProvider(inventoryManager),
                ItemType.Herb => new HerbDisplayer(),
                ItemType.Furnace => new FurnaceDisplayer(),
                ItemType.LinhDan => new LinhDanDisplayer(),
                ItemType.Recipe => new RecipeDisplayer(),
                ItemType.Material => new MaterialDisplayer(),
                ItemType.ThanThong => new ThanThongBookDisplayer(),
                ItemType.CongPhap => new CongPhapBookDisplayer(),

                _ => null,
            };

        }
    }
}
