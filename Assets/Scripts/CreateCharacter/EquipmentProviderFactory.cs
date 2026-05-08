using System;
using System.Collections.Generic;
using LuyenDan;
using Network;

namespace CreateCharacter
{
    /// <summary>
    /// Factory class để tạo IEquipmentProvider theo ItemType
    /// Loại bỏ if-else chain trong UIManager
    /// </summary>
    public static class EquipmentProviderFactory
    {
        public static IEquipmentProvider GetProvider(ItemType itemType)
        {
            InventoryManager inventoryManager = Client.Instance.CharacterData.InventoryManager;
            return itemType switch
            {
                ItemType.VuKhi => new VuKhiEquipmentProvider(inventoryManager),
                ItemType.PhongCu => new PhongCuEquipmentProvider(inventoryManager),
                ItemType.TrangSuc => new TrangSucEquipmentProvider(inventoryManager),
                _ => throw new NotImplementedException(),
            };

        }
        
    }
}
