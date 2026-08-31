using System.Collections.Generic;
using System.Linq;
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
    public partial class SpecialSkillEventReward : IEventReward
    {
        public List<ItemRef> Items { get; set; }
        public BaseSkill.ETierSkill[] Tiers { get; set; }
        public ItemType Type { get; set; }
        public BaseSkill.ETypeSkill TypeSkill { get; set; }
        public ushort Count { get; set; }

        public void Claim(CharacterData charData)
        {
            foreach (var itemRef in Items)
            {
                charData.InventoryManager.Add(itemRef);
            }
        }

        public void ShowUI(RewardUIContext context)
        {
            var itemUi = Object.Instantiate(context.SkillItemPrefab, context.Container);
            itemUi.SetText(GetDescription());
        }

        private string GetDescription()
        {
            var tierText = BaseSkill.GetTextTiersSkill(Tiers);
            var typeItemText = Type switch
            {
                ItemType.ThanThong => "Thần thông",
                ItemType.CongPhap => "Công pháp",
            };
            return $"x{Utils.FormatCount(Count)} {tierText} {typeItemText}" +
                   (TypeSkill == BaseSkill.ETypeSkill.None
                       ? string.Empty
                       : $" hệ {BaseSkill.GetTextTypeSkill(TypeSkill)}");
        }

        private SkillData[] GetItemDatas()
        {
            var ui = Pool.Get<UIManager>();
            var gameManager = Pool.Get<GameManager>();
            var skillDatas = Items.Select(x =>
            {
                ISkillItemData skillItemData = x.Type switch
                {
                    ItemType.ThanThong => new ThanThongItemData(x, gameManager, ui),
                    ItemType.CongPhap => new CongPhapItemData(x, gameManager, ui)
                };
                return new SkillData(x, skillItemData);
            }).ToArray();
            return skillDatas;
        }

        private string GetRichTextOf(SkillData skillData)
        {
            return
                $"<color=#{ColorUtility.ToHtmlStringRGBA(skillData.ItemData.VisualQuality.Color)}>{skillData.ItemData.Name} x{skillData.ItemRef.Count}</color>";
        }
        public string GetRichText()
        {
            var skillDatas = GetItemDatas();
            return string.Join(", ", skillDatas.Select(GetRichTextOf));
        }
    }
}