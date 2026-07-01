using MemoryPack;

namespace AllMaps
{
    [MemoryPackable]
    public partial class HerbsShop : Shop
    {
        public override EShopType GetTypeShop()
        {
            return EShopType.ShopDuocLieu;
        }
    }
}