using Base;
using ConvertElementalSkill;
using LongDHModules;
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
            if (itemData is IItemData i)
            {
                ui.SetData(i);
                ui.SetCount(Item.Count);
            }
            else if (itemData is SkillData skillData)
            {
                ui.SetSkill(skillData);
            }
        }

        private object GetItemData()
        {
            switch (Item.Type)
            {
                case ItemType.Material:
                    return LuyenKhiResources.Instance.FindMaterial(Item.Id);
                case ItemType.LinhDan:
                case ItemType.Herb:
                case ItemType.Recipe:
                    return LuyenDanResources.Instance.FindItem(Item);
                case ItemType.CongPhap:
                    return new SkillData(Item, new CongPhapItemData(Item, Pool.Get<GameManager>(), Pool.Get<UIManager>()));
                case ItemType.ThanThong:
                    return new SkillData(Item, new ThanThongItemData(Item,Pool.Get<GameManager>(), Pool.Get<UIManager>()));
                default:
                    return null;
            }
        }

        public string GetRichText()
        {
            var core = CoreConfig.Instance;
            var item = GetItemData();
            if (item is IItemData i)
            {
                return
                    $"<color=#{ColorUtility.ToHtmlStringRGBA(core.FindQuality(i.Tier).Color)}>{i.Name} x{Item.Count}</color>";
            }
            if (item is SkillData skillData)
            {
                return
                    $"<color=#{ColorUtility.ToHtmlStringRGBA(skillData.ItemData.VisualQuality.Color)}>{skillData.ItemData.Name} x{Item.Count}</color>";
            }
            return string.Empty;
        }
    }
}