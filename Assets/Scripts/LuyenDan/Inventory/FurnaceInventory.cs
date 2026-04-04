using System.Collections.Generic;
using System.Linq;

namespace LuyenDan.Inventory
{
    public class FurnaceInventory : Inventory
    {
        public void Init(HashSet<FurnaceData> furnaceDatas)
        {
            var datas = furnaceDatas.Select(x => x.Ref);
            var itemList = ItemListUI as FurnaceItemListUI;
            itemList.Init(datas);
            InitFilterGroup();
        }
    }
}