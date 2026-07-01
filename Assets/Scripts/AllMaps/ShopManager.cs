using System;
using System.Collections.Generic;
using System.Linq;
using LuyenDan;
using MemoryPack;
using Sirenix.OdinInspector;

namespace AllMaps
{
    [MemoryPackable]
    [Serializable]
    public partial class ShopManager
    {
        [ShowInInspector] public Shop ShopDuocLieu { get; set; } = new HerbsShop();
        [ShowInInspector] public Shop ShopVatLieu { get; set; } = new MaterialShop();
        [ShowInInspector] public Shop ShopSkill { get; set; } = new SkillShop();

        public Shop GetShop(EShopType shopType)
        {
            var shops = new Shop[] {ShopDuocLieu, ShopVatLieu, ShopSkill};
            return shops.First(x => x.GetTypeShop() == shopType);
        }

        public void SetShop(Shop shop)
        {
            var shopType = shop.GetTypeShop();
            switch (shopType)
            {
                case EShopType.ShopDuocLieu:
                    ShopDuocLieu = shop;
                    break;
                case EShopType.ShopVatLieu:
                    ShopVatLieu = shop;
                    break;
                case EShopType.Skill:
                    ShopSkill = shop;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shopType), shopType, null);
            }
        }
    }
}