using System.Collections.Generic;
using System.Linq;

namespace LuyenDan.Inventory
{
    public class FurnaceInventory : Inventory
    {
        public void Init(HashSet<FurnaceData> furnaceDatas)
        {
            var itemList = ItemListUI as FurnaceItemListUI;
            itemList?.InitAsFurnace(furnaceDatas);
            InitFilterGroup();
        }
    }
}