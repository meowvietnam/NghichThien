using Base;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial struct LoadShopRequest
    {
        public EShopType ShopType { get; set; }
        public EMonPhai MonPhai { get; set; }
    }
}