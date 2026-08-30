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
    public partial class SkillEventReward : IEventReward
    {
        public ItemRef Item { get; set; }
        public BaseSkill.ETierSkill Tier { get; set; }
        public ItemType Type { get; set; }
        public ushort Count { get; set; }
        public void Claim(CharacterData charData)
        {
            charData.InventoryManager.Add(Item);
        }
        public void ShowUI(RewardUIContext context)
        {
            var itemUi = Object.Instantiate(context.SkillItemPrefab, context.Container);
            itemUi.SetText(GetDescription());
        }

        private string GetDescription()
        {
            var tierText = BaseSkill.GetTextTierSkill(Tier);
            var typeItemText = Type switch
            {
                ItemType.ThanThong => "Thần thông",
                ItemType.CongPhap => "Công pháp",
            };
            return $"x{Utils.FormatCount(Count)} {tierText} {typeItemText}";
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