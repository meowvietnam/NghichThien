using System.Linq;
using AllMaps.TabPanel.ToolTipDisplayers;
using LuyenDan;
using LuyenKhi;
using Network;
using UnityEngine;

namespace CreateCharacter
{
    /// <summary>
    /// Concrete factory cung cấp PhongCu (Phòng Cụ) từ InventoryManager
    /// </summary>
    public class PhongCuEquipmentProvider : EquipmentDisplayer, IEquipmentProvider
    {
        private readonly InventoryManager _inventoryManager;

        public PhongCuEquipmentProvider(InventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
        }
        public override ItemType SupportedType => ItemType.PhongCu;

        public override EquipmentType EquipemtType => EquipmentType.Armor;

        protected override string GetTextNameEquipment(ArtifactType artifactType)
        {

            return $"Phòng Cụ {base.GetTextNameEquipment(artifactType)}";
        }

        public Equipment GetEquipmentById(ushort equipmentId)
        {
            return _inventoryManager.PhongCus.FirstOrDefault(x => x.SelfId == equipmentId);
        }
    }
}
