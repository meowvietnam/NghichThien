using System.Linq;
using AllMaps.ItemProvider;
using LuyenDan;
using LuyenKhi;
using Network;
using UnityEngine;
namespace CreateCharacter
{
    /// <summary>
    /// Concrete factory cung cấp TrangSuc (Trang Sức) từ InventoryManager
    /// </summary>
    public class TrangSucEquipmentProvider : EquipmentDisplayer
    {

        public override ItemType SupportedType => ItemType.TrangSuc;

        public override EquipmentType EquipemtType => EquipmentType.Jewelry;

        protected override string GetTextNameEquipment(ArtifactType artifactType)
        {

            return $"Trang Sức {base.GetTextNameEquipment(artifactType)}";
        }
        public override Equipment GetDataItemById(ushort equipmentId)
        {
            return _inventoryManager.TrangSucs.FirstOrDefault(x => x.SelfId == equipmentId);
        }
    }
}
