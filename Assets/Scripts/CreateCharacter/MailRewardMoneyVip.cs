using Base;
using LuyenDan.Inventory;
using MemoryPack;
using UnityEngine;

namespace CreateCharacter
{
    public class MoneyVipItem : IItemData
    {
        public int Id => 999999;
        public Sprite Icon { get; private set; }
        public string Name => "Tiên ngọc";
        public Tier Tier => Tier.LucPham;
        public string Description => string.Empty;

        public MoneyVipItem(Sprite icon)
        {
            Icon = icon;
        }

        public static string GetRichText(int count)
        {
            var icon = MoneyResources.Instance.MoneyVipIcon;
            var item = new MoneyVipItem(icon);
            var core = CoreConfig.Instance;
            return
                $"<color=#{ColorUtility.ToHtmlStringRGBA(core.FindQuality(item.Tier).Color)}>{item.Name} x{count}</color>";
        }

        public static void ShowNotification(int count)
        {
            var text =
                $"Bạn nhận được: {GetRichText(count)}";
            NotifyGroup.Instance.Show(text);
        }
    }

    [MemoryPackable]
    public partial class MailRewardMoneyVip : IMailReward
    {
        public int MoneyVip { get; set; }

        public void ClaimReward(CharacterData characterData)
        {
            characterData.MoneyVip += MoneyVip;
        }

        public void ShowUI(MailRewardUIContext context)
        {
            var itemData = GetItemData();
            var ui = context.RewardUI;
            ui.SetData(itemData);
            ui.SetCount(MoneyVip);
        }

        private IItemData GetItemData()
        {
            var icon = MoneyResources.Instance.MoneyVipIcon;
            return new MoneyVipItem(icon);
        }

        public string GetRichText()
        {
            return MoneyVipItem.GetRichText(MoneyVip);
        }
    }
}