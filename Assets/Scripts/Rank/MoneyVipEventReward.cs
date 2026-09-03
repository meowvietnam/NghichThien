using Base;
using CreateCharacter;
using LuyenDan.Inventory;
using MemoryPack;
using UnityEngine;

namespace Rank
{
    [MemoryPackable]
    public partial class MoneyVipEventReward : IEventReward
    {
        public int MoneyVip { get; set; }
        public void Claim(CharacterData charData)
        {
            var newMoneyVip = charData.MoneyVip + MoneyVip;
            charData.MoneyVip = newMoneyVip;
        }
        private IItemData GetItemData()
        {
            var icon = MoneyResources.Instance.MoneyVipIcon;
            return new MoneyVipItem(icon);
        }
        public string GetRichText()
        {
            var icon = MoneyResources.Instance.MoneyVipIcon;
            var item = new MoneyVipItem(icon);
            var core = CoreConfig.Instance;
            return
                $"<color=#{ColorUtility.ToHtmlStringRGBA(core.FindQuality(item.Tier).Color)}>{item.Name} x{MoneyVip}</color>";
        }
        public void ShowUI(RewardUIContext context)
        {
            var item = Object.Instantiate(context.MoneyVipPrefab, context.Container);
            item.SetText(GetDescription());
        }

        private string GetDescription()
        {
            return $"x{Utils.FormatCount(MoneyVip)} Tiên ngọc";
        }
    }
}