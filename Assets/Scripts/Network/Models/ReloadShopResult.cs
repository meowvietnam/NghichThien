using AllMaps;
using Base;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ReloadShopResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Shop Shop { get; set; }
        public EMonPhai MonPhai { get; set; }
    }
}