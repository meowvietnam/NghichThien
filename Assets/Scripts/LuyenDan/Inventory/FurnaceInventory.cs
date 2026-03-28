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
            var result = furnaceDatas.Join(
                itemList.Items,
                x => x.Ref,
                y => y.ItemRef,
                (x, y) => new { x, y }
            );
            foreach (var item in result)
            {
                item.y.SetFurnaceId(item.x.SelfId);
            }
        }
    }
}