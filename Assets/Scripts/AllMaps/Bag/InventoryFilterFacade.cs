using System.Collections.Generic;
using System.Linq;
using Base;
using LuyenDan;
using LuyenDan.Inventory;

public class InventoryFilterFacade
{
    private readonly Bag1ItemPrefabInventory bag;

    public InventoryFilterFacade(Bag1ItemPrefabInventory bag)
    {
        this.bag = bag;
    }

    public List<ItemRef> ApplyFilter(
        List<ItemRef> itemFullDatas,
        bool isAllFilterA,
        bool isAllFilterB,
        ItemType typeCurrent,
        Tier tierCurrent,
        BaseSkill.ETierSkill eTierSkillCurrent,
        EquipmentTier eTierEquipmentCurrent)
    {
        if (itemFullDatas == null)
        {
            return new List<ItemRef>();
        }

        if (isAllFilterA)
        {
            return itemFullDatas;
        }

        if (isAllFilterB)
        {
            return itemFullDatas
                .Where(item => item.Type == typeCurrent)
                .ToList();
        }

        var filterProvider = FilterProviderFactory.Create(typeCurrent);
        return itemFullDatas
            .Where(item => item.Type == typeCurrent &&
                filterProvider.Matches(item, typeCurrent, tierCurrent, eTierSkillCurrent, eTierEquipmentCurrent))
            .ToList();
    }

    public List<string> GetSecondaryFilterOptions(ItemType typeCurrent)
    {
        var filterProvider = FilterProviderFactory.Create(typeCurrent);
        var options = new List<string>();
        for (int i = 0; i < filterProvider.OptionCount; i++)
        {
            options.Add(filterProvider.GetOptionText(i));
        }
        return options;
    }
}
