using MemoryPack;

namespace AllMaps
{
    [MemoryPackable]
    public partial class MaterialShop : Shop
    {
        public override EShopType GetTypeShop()
        {
            return EShopType.ShopVatLieu;
        }
    }
}