using Base;
using LuyenDan;
using LuyenDan.Inventory;
using LuyenKhi;
using MemoryPack;
using UnityEngine;

namespace CreateCharacter
{
    [MemoryPackable]
    public partial class MailRewardItem : IMailReward
    {
        public ItemRef Item { get; set; }

        public void ClaimReward(CharacterData characterData)
        {
            var inventory = characterData.InventoryManager;
            inventory.Add(Item);
        }

        public void ShowUI(MailRewardUIContext context)
        {
            var itemData = GetItemData();
            var ui = context.RewardUI;
            ui.SetData(itemData);
            ui.SetCount(Item.Count);
        }

        private IItemData GetItemData()
        {
            switch (Item.Type)
            {
                case ItemType.Material:
                    return LuyenKhiResources.Instance.FindItem(Item);
                default:
                    return LuyenDanResources.Instance.FindItem(Item);
            }
        }

        public string GetRichText()
        {
            var core = CoreConfig.Instance;
            var item = GetItemData();
            return
                $"<color=#{ColorUtility.ToHtmlStringRGBA(core.FindQuality(item.Tier).Color)}>{item.Name} x{Item.Count}</color>";
        }
    }
}