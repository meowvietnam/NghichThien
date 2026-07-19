using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using LuyenDan;
using MemoryPack;
using Network.Models;
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
        [ShowInInspector] public Shop ShopSkillAll { get; set; } = new SkillShop();
        public Shop GetShop(LoadShopRequest request)
        {
            var shopType = request.ShopType;
            var monPhai = request.MonPhai;
            switch (shopType)
            {
                case EShopType.ShopDuocLieu:
                    return ShopDuocLieu;
                case EShopType.ShopVatLieu:
                    return ShopVatLieu;
                case EShopType.Skill:
                    if (monPhai == EMonPhai.None)
                    {
                        return ShopSkillAll;
                    }
                    return ShopSkill;
                default:
                    throw new();
            }
        }
        public void SetShop(Shop shop, EMonPhai monPhai)
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
                    if (monPhai == EMonPhai.None)
                    {
                        ShopSkillAll = shop;
                    }
                    else
                    {
                        ShopSkill = shop;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shopType), shopType, null);
            }
        }
    }
}