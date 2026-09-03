using DP;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ReloadDpShopResult : Result
    {
        public DongPhuShop Shop { get; set; }
    }
}