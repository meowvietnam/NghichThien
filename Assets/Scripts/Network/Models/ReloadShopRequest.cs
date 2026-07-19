using Base;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial struct ReloadShopRequest
    {
        public EShopType ShopType { get; set; }
        public EMonPhai MonPhai { get; set; }
    }
}