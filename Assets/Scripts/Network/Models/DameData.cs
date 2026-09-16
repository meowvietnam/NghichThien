using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class DameData
    {
        public long Dame { get; set; }
        public long DameHoTrao { get; set; }
        public bool IsNeTranh { get; set; }
        public bool IsDef { get; set; }
    }
}