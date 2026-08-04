using System;
using System.Collections.Generic;
using AllMaps.ItemProvider;
using AllMaps.ui_rank;
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
                ItemType.ThanThong => new ThanThongBookProvider(),
                ItemType.CongPhap => new CongPhapBookProvider(),
                ItemType.Herb => new HerbProvider(),
                ItemType.Furnace => new FurnaceProvider(),
                ItemType.Recipe => new RecipeProvider(),
                ItemType.Material => new MaterialProvider(),
                _ => null,
            };
        }
        public static IItemProvider<T> GetProvider<T>(ItemType itemType)
        {
            return GetRawProvider(itemType) as IItemProvider<T>;
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
                ItemType.ThanThong => (tabPanel.gameObject.activeSelf && tabPanel.currentBtnWithPanel.typePanel == TabPanel.ETypePanel.ThanThong) || IsBaseSkillVisual()? new ThanThongProvider()  : new ThanThongBookProvider(),
                ItemType.CongPhap => (tabPanel.gameObject.activeSelf && tabPanel.currentBtnWithPanel.typePanel == TabPanel.ETypePanel.CongPhap)|| IsBaseSkillVisual() ? new CongPhapProvider() : new CongPhapBookProvider(),
                _ => null,
            };
        }
        public static bool IsBaseSkillVisual()
        {
            var beQuan = Pool.Get<UIBeQuan>();
            return !Pool.Get<TabPanel>().gameObject.activeSelf&& (Pool.Get<UIManager>().currentPopup is PopupInfor || (beQuan != null && beQuan.popBeQuan.gameObject.activeSelf && beQuan.currentBtnWithPage.typePage == UIBeQuan.ETypePage.Update) || Pool.Get<RankPanel>().popupInfor.gameObject.activeSelf);
        }
    }
}
