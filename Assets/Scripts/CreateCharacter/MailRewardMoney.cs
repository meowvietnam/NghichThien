using Base;
using LuyenDan.Inventory;
using MemoryPack;
using UnityEngine;

namespace CreateCharacter
{
    public class MoneyItem : IItemData
    {
        public int Id => 99999;
        public Sprite Icon { get; private set; }
        public string Name => "Linh thạch";
        public Tier Tier => Tier.NhiPham;
        public string Description => string.Empty;

        public MoneyItem(Sprite icon)
        {
            Icon = icon;
        }
    }

    [MemoryPackable]
    public partial class MailRewardMoney : IMailReward
    {
        public int Money { get; set; }

        public void ClaimReward(CharacterData characterData)
        {
            characterData.Stats.LinhThach += Money;
        }

        public void ShowUI(MailRewardUIContext context)
        {
            var itemData = GetItemData();
            var ui = context.RewardUI;
            ui.SetData(itemData);
            ui.SetCount(Money);
        }

        private IItemData GetItemData()
        {
            var icon = MoneyResources.Instance.MoneyIcon;
            return new MoneyItem(icon);
        }

        public string GetRichText()
        {
            var core = CoreConfig.Instance;
            var item = GetItemData();
            return
                $"<color=#{ColorUtility.ToHtmlStringRGBA(core.FindQuality(item.Tier).Color)}>{item.Name} x{Money}</color>";
        }
    }
}