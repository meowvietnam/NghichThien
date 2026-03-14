using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using UnityEngine;

namespace LuyenDan.Inventory
{
    public class HerbItemListUI : ItemListUI<HerbInventoryItemUI>
    {
        [SerializeField] private HerbTierFilterSelected filterSelected;
        private bool isFilterTier;
        private Tier currentFilterTier;
        private HerbType currentFilterType;
        public override void Init(IEnumerable<ItemRef> datas)
        {
            base.Init(datas);
            NoFilter();
        }

        public override void Filter(Tier tier)
        {
            isFilterTier = true;
            currentFilterTier = tier;
            filterSelected.Select(tier);
            var sorted = Items
                .Where(x => x.IsHasItem)
                .OrderBy(x => x.Data.Id);
            foreach (var item in sorted)
            {
                var active = item.IsHasItem && item.Data.Tier == tier && item.Data.Is(currentFilterType);
                item.SetActive(active);
            }
        }

        public void Filter(HerbType herbType)
        {
            currentFilterType = herbType;
            if (isFilterTier)
            {
                var sorted = Items
                    .Where(x => x.IsHasItem)
                    .OrderBy(x => x.Data.Id);
                foreach (var item in sorted)
                {
                    var active = item.IsHasItem && item.Data.Tier == currentFilterTier && item.Data.Is(currentFilterType);
                    item.SetActive(active);
                }
            }
            else
            {
                var sorted = Items
                    .Where(x => x.IsHasItem)
                    .OrderBy(x => x.Data.Tier)
                    .ThenBy(x => x.Data.Id);
                foreach (var item in sorted)
                {
                    var active = item.IsHasItem && item.Data.Is(currentFilterType);
                    item.SetActive(active);
                }
            }
        }
        public override void NoFilter()
        {
            isFilterTier = false;
            filterSelected.NoFilter();
            var sorted = Items
                .Where(x => x.IsHasItem)
                .OrderBy(x => x.Data.Tier)
                .ThenBy(x => x.Data.Id);
            foreach (var item in sorted)
            {
                var active = item.IsHasItem && item.Data.Is(currentFilterType);
                item.SetActive(active);
            }
        }
    }
}
