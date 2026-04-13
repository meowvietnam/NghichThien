using System;
using Base;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan.Inventory
{
    public class HerbTierFilterItem : HerbFilterItem
    {
        [SerializeField] private Tier tier;
        [SerializeField] private HerbItemListUI target;

        protected override void OnClick()
        {
            target.Filter(tier);
        }
    }
}