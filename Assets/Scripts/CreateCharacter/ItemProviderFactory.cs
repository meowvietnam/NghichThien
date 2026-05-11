using System;
using System.Collections.Generic;
using AllMaps.ItemProvider;
using LuyenDan;
using Network;

namespace CreateCharacter
{
    /// <summary>
    /// Factory class để tạo IEquipmentProvider theo ItemType
    /// Loại bỏ if-else chain trong UIManager
    /// </summary>
    public static class ItemProviderFactory
    {
        // 1. Hàm dành cho hiển thị (Không cần <T>, không lo bị null do ép kiểu)
        public static IItemProvider GetProvider(ItemType itemType)
        {
            return GetRawProvider(itemType) as IItemProvider;
        }

        // 2. Hàm dành cho việc lấy Data cụ thể (Cần truyền <T>)
        public static ItemProvider<T> GetProvider<T>(ItemType itemType)
        {
            return GetRawProvider(itemType) as ItemProvider<T>;
        }
        private static object GetRawProvider(ItemType itemType)
        {
            return itemType switch
            {
                ItemType.VuKhi => new VuKhiEquipmentProvider(),
                ItemType.PhongCu => new PhongCuEquipmentProvider(),
                ItemType.TrangSuc => new TrangSucEquipmentProvider(),
                ItemType.Herb => new HerbProvider(),
                ItemType.Furnace => new FurnaceProvider(),
                ItemType.LinhDan => new LinhDanProvider(),
                ItemType.Recipe => new RecipeProvider(),
                ItemType.Material => new MaterialProvider(),
                ItemType.ThanThong => new ThanThongBooProvider(),
                ItemType.CongPhap => new CongPhapBookProvider(),

                _ => null,
            };
        }
    }
}
