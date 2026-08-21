using Auction;
using Base;
using ConvertElementalSkill;
using CreateCharacter;
using LongDHModules;
using LuyenDan;
using MemoryPack;
using UnityEngine;

namespace Rank
{
    [MemoryPackable]
    public partial class ItemMonsterRankReward : IMonsterRankReward
    {
        public ItemRef Item { get; set; }
        public void Claim(CharacterData charData)
        {
            charData.InventoryManager.Add(Item);
        }
        public void ShowUI(MonsterRankRewardUIContext context)
        {
            var skillData = GetItemData();
            var itemUi = Object.Instantiate(context.ItemUIPrefab, context.Container);
            itemUi.SetSkill(skillData);
        }

        private SkillData GetItemData()
        {
            var type = Item.Type;
            var ui = Pool.Get<UIManager>();
            var gameManager = Pool.Get<GameManager>();
            ISkillItemData skillItemData = type switch
            {
                ItemType.ThanThong => new ThanThongItemData(Item, gameManager, ui),
                ItemType.CongPhap => new CongPhapItemData(Item, gameManager, ui)
            };
            var skillData = new SkillData(Item, skillItemData);
            return skillData;
        }
        public string GetRichText()
        {
            var skillData = GetItemData();
            return
            $"<color=#{ColorUtility.ToHtmlStringRGBA(skillData.ItemData.VisualQuality.Color)}>{skillData.ItemData.Name} x{Item.Count}</color>";
        }
    }
}