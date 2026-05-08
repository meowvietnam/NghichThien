using System;
using System.Collections.Generic;
using LuyenDan;

namespace CreateCharacter
{
    /// <summary>
    /// Factory class để tạo IEquipmentProvider theo ItemType
    /// Loại bỏ if-else chain trong UIManager
    /// </summary>
    public class EquipmentProviderFactory
    {
        private readonly Dictionary<ItemType, IEquipmentProvider> _providers;

        public EquipmentProviderFactory(InventoryManager inventoryManager)
        {
            _providers = new Dictionary<ItemType, IEquipmentProvider>
            {
                { ItemType.VuKhi, new VuKhiEquipmentProvider(inventoryManager) },
                { ItemType.PhongCu, new PhongCuEquipmentProvider(inventoryManager) },
                { ItemType.TrangSuc, new TrangSucEquipmentProvider(inventoryManager) }
            };
        }

        /// <summary>
        /// Lấy provider tương ứng với ItemType
        /// </summary>
        public IEquipmentProvider GetProvider(ItemType itemType)
        {
            if (_providers.TryGetValue(itemType, out var provider))
            {
                return provider;
            }

            throw new ArgumentException($"Unsupported ItemType: {itemType}");
        }

        /// <summary>
        /// Lấy equipment theo ItemType và ID
        /// </summary>
        public Equipment GetEquipment(ItemType itemType, ushort equipmentId)
        {
            return GetProvider(itemType).GetEquipmentById(equipmentId);
        }
    }
}
