using System;
using System.Collections.Generic;
using AllMaps.ItemProvider;
using LongDHModules;
using LuyenDan;
using Network;

namespace AllMaps.ItemProvider
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
        public static IUseItemProvider GetUseItemProvider(ItemType itemType)
        {
            return itemType switch
            {
                ItemType.VuKhi => new VuKhiEquipmentProvider(),
                ItemType.PhongCu => new PhongCuEquipmentProvider(),
                ItemType.TrangSuc => new TrangSucEquipmentProvider(),
               
                ItemType.LinhDan => new LinhDanProvider(),
               

                _ => null,
            };
        }
        // 2. Hàm dành cho việc lấy Data cụ thể (Cần truyền <T>)
        public static ItemProvider<T> GetProvider<T>(ItemType itemType)
        {
            return GetRawProvider(itemType) as ItemProvider<T>;
        }
        private static object GetRawProvider(ItemType itemType)
        {
            var tabPanel = Pool.Get<TabPanel>();
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
                ItemType.ThanThong => tabPanel.gameObject.activeSelf && tabPanel.currentBtnWithPanel.typePanel == TabPanel.ETypePanel.ThanThong? new ThanThongProvider()  :new ThanThongBookProvider(),
                ItemType.CongPhap => tabPanel.gameObject.activeSelf && tabPanel.currentBtnWithPanel.typePanel == TabPanel.ETypePanel.CongPhap ? new CongPhapProvider() :new CongPhapBookProvider(),

                _ => null,
            };
        }
    }
}
