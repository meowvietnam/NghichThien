using System.Collections.Generic;
using System.Linq;
using Base;
using UnityEngine;

namespace LuyenDan.Inventory
{
    public class FurnaceItemListUI : ItemListUI<FurnaceInventoryItemUI>
    {
        public void InitAsFurnace(IEnumerable<FurnaceData> furnaceDatas)
        {
            foreach (var furnaceInventoryItemUI in Items)
            {
                furnaceInventoryItemUI.Destroy();
            }

            Items.Clear();
            foreach (var data in furnaceDatas)
            {
                var instance = Instantiate(prefab, content);
                instance.Init(data);
                Items.Add(instance);
            }
        }

        public override void Filter(Tier tier)
        {
            var selectedFurnace = LuyenDanPanel.Instance.Alchermy.SelectedFurnace;
            foreach (var item in Items)
            {
                var active = item.IsHasItem && item.Tier == tier && item != selectedFurnace &&
                             item.FData.Durability > 0;
                item.SetActive(active);
            }
        }

        public override void NoFilter()
        {
            var selectedFurnace = LuyenDanPanel.Instance.Alchermy.SelectedFurnace;
            foreach (var item in Items)
            {
                item.SetActive(selectedFurnace != item && item.FData.Durability > 0);
            }
        }
    }
}