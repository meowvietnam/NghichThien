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
    public partial class RandomSpecialSkillEventReward : IEventReward
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
            if (Type == ItemType.ThanThong)
            {
                var itemUi = Object.Instantiate(context.RandomSkillItemPrefab, context.Container);
                itemUi.SetText(GetDescription(Count));
            }
            else
            {
                var countPerOne = 6;
                int totalRandomItem = Count / countPerOne;
                var remaining = Count % countPerOne;
                for (int i = 0; i < totalRandomItem; i++)
                {
                    var itemUi = Object.Instantiate(context.RandomSkillItemPrefab, context.Container);
                    itemUi.SetText(GetDescription(countPerOne));
                }
                if (remaining > 0)
                {
                    var itemUi = Object.Instantiate(context.RandomSkillItemPrefab, context.Container);
                    itemUi.SetText(GetDescription(remaining));
                }  
            }
 
        }

        private string GetDescription(int count)
        {
            var tierText = BaseSkill.GetTextTiersSkill(Tiers);
            var typeItemText = Type switch
            {
                ItemType.ThanThong => "Thần thông",
                ItemType.CongPhap => "Công pháp",
            };
            return $"x{Utils.FormatCount(count)} {tierText} {typeItemText}" +
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
