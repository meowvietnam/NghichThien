using LuyenDan;
using LuyenDan.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps.TabPanel.ToolTipDisplayers
{
    public class ToolTipContext
    {
        public Text TypeText { get; set; }
        public Text QualityText { get; set; }
        public Text DescriptionText { get; set; }
        public Text DescTextItem { get; set; }
        public Image PrefabIconRequire { get; set; }
        public Image PrefabLeftInfor { get; set; }
        public Image PrefabRightInfor { get; set; }
        public Sprite[] SpriteLinhKhis { get; set; }
        public ItemRef ItemRef { get; set; }
        public IItemData IItemData { get; set; }
        public FurnaceData FurnaceData { get; set; }
        public EActionShowItemInfor ActionShowItem;
        public System.Action<int> SetPriceText;
    }

    public interface IToolTipDisplayer
    {
        ItemType SupportedType { get; }
        void Display(ToolTipContext ctx);
    }
}
