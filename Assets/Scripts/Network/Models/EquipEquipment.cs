using MemoryPack;
using LuyenKhi;

namespace Network.Models
{
    [MemoryPackable]
    public partial class EquipEquipment
    {
        public ushort Id { get; set; }
        public EquipmentType TypeEquipment { get; set; }
    }
}