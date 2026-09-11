using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class GachaModel
    {
        public ETypeGacha TypeGacha { get; set; }
        public bool IsGacha10 { get; set; }
        public BaseSkill.ETypeSkill TypeSkill { get; set; }
    }
}