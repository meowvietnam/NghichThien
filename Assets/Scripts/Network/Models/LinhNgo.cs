using LuyenDan;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class LinhNgo
    {
        public int Id { get; set; }
        public ItemType Type { get; set; }
    }
}