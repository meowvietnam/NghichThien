using AllMaps;
using Base;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class LoadShopResult : Result
    {
        public Shop Shop { get; set; }
        public EMonPhai MonPhai { get; set; }
    }
}