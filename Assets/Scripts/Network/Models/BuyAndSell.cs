using System.Collections.Generic;
using Base;
using LuyenDan;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class BuyAndSell
    {
        public List<ItemRef> BuyItems { get; set; }
        public List<ItemRef> SellItems { get; set; }
        public EShopType ShopType { get; set; }
        public EMonPhai MonPhai { get; set; }
    }
}