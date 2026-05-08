using System.Linq;

namespace CreateCharacter
{
    /// <summary>
    /// Concrete factory cung cấp VuKhi (Vũ Khí) từ InventoryManager
    /// </summary>
    public class VuKhiEquipmentProvider : IEquipmentProvider
    {
        private readonly InventoryManager _inventoryManager;

        public VuKhiEquipmentProvider(InventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
        }

        public Equipment GetEquipmentById(ushort equipmentId)
        {
            return _inventoryManager.VuKhis.FirstOrDefault(x => x.SelfId == equipmentId);
        }
    }
}
