using System.Linq;

namespace CreateCharacter
{
    /// <summary>
    /// Concrete factory cung cấp TrangSuc (Trang Sức) từ InventoryManager
    /// </summary>
    public class TrangSucEquipmentProvider : IEquipmentProvider
    {
        private readonly InventoryManager _inventoryManager;

        public TrangSucEquipmentProvider(InventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
        }

        public Equipment GetEquipmentById(ushort equipmentId)
        {
            return _inventoryManager.TrangSucs.FirstOrDefault(x => x.SelfId == equipmentId);
        }
    }
}
