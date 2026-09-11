using System;
using Auction;
using Base;
using ConvertElementalSkill;
using CreateCharacter;
using LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using LuyenKhi;
using MemoryPack;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rank
{
    [MemoryPackable]
    public partial class SkillEventReward : IEventReward
    {
        public ItemRef Item { get; set; }

        public void Claim(CharacterData charData)
        {
            charData.InventoryManager.Add(Item);
        }

        public void ShowUI(RewardUIContext context)
        {
            var itemData = GetData();
            var itemUI = Object.Instantiate(context.SkillItemUIPrefab, context.Container);
            itemUI.SetData(itemData);
        }

        private SkillData GetData()
        {
            var type = Item.Type;
            var gameManager = Pool.Get<GameManager>();
            var ui = Pool.Get<UIManager>();
            ISkillItemData skillItemData = type switch
            {
                ItemType.ThanThong => new ThanThongItemData(Item, gameManager, ui),
                ItemType.CongPhap => new CongPhapItemData(Item, gameManager, ui)
            };
            return new SkillData(Item, skillItemData);
        }

        public string GetRichText()
        {
            var item = GetData();
            return
                $"<color=#{ColorUtility.ToHtmlStringRGBA(item.ItemData.VisualQuality.Color)}>{item.ItemData.Name} x{Item.Count}</color>";
        }
    }
}