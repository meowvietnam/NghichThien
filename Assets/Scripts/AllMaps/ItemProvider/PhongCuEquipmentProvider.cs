using System.Linq;
using AllMaps.ItemProvider;
using LuyenDan;
using LuyenKhi;
using Network;
using UnityEngine;
using CreateCharacter;

namespace AllMaps.ItemProvider
{
    /// <summary>
    /// Concrete factory cung cấp PhongCu (Phòng Cụ) từ InventoryManager
    /// </summary>
    public class PhongCuEquipmentProvider : EquipmentProvider
    {

        public override ItemType SupportedType => ItemType.PhongCu;

        public override EquipmentType EquipemtType => EquipmentType.Armor;

        protected override string GetTextNameEquipment(ArtifactType artifactType)
        {

            return $"Phòng Cụ {base.GetTextNameEquipment(artifactType)}";
        }

        public override CreateCharacter.Equipment GetDataItemByIdRef(ushort equipmentId)
        {
            return _inventoryManager.PhongCus.FirstOrDefault(x => x.SelfId == equipmentId);
        }
    }
}
