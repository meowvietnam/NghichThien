using System.Linq;
using AllMaps.ItemProvider;
using LuyenDan;
using LuyenKhi;
using Network;
using UnityEngine;
namespace CreateCharacter
{
    /// <summary>
    /// Concrete factory cung cấp VuKhi (Vũ Khí) từ InventoryManager
    /// </summary>
    public class VuKhiEquipmentProvider : EquipmentDisplayer
    {

        public override ItemType SupportedType => ItemType.VuKhi;

        public override EquipmentType EquipemtType => EquipmentType.Weapon;

        protected override string GetTextNameEquipment(ArtifactType artifactType)
        {

            return $"Vũ Khí {base.GetTextNameEquipment(artifactType)}";
        }

        public override Equipment GetDataItemById(ushort equipmentId)
        {
            return _inventoryManager.VuKhis.FirstOrDefault(x => x.SelfId == equipmentId);
        }
    }
}
