using System.Linq;

namespace CreateCharacter
{
    /// <summary>
    /// Concrete factory cung cấp PhongCu (Phòng Cụ) từ InventoryManager
    /// </summary>
    public class PhongCuEquipmentProvider : IEquipmentProvider
    {
        private readonly InventoryManager _inventoryManager;

        public PhongCuEquipmentProvider(InventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
        }

        public Equipment GetEquipmentById(ushort equipmentId)
        {
            return _inventoryManager.PhongCus.FirstOrDefault(x => x.SelfId == equipmentId);
        }
    }
}
