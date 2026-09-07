using Base;
using CreateCharacter;
using LuyenDan;
using LuyenDan.Inventory;
using MemoryPack;
using Network.Models;
using UnityEngine;

namespace Rank
{
    [MemoryPackable]
    public partial class LinhDanEventReward : IEventReward
    {
        public ushort Id { get; set; }
        public ushort Count { get; set; }

        public void Claim(CharacterData charData)
        {
            charData.InventoryManager.Add(CreateRef());
        }

        private ItemRef CreateRef()
        {
            var itemRef = new ItemRef()
            {
                Id = Id,
                Count = Count,
                Type = ItemType.LinhDan
            };
            return itemRef;
        }

        public void ShowUI(RewardUIContext context)
        {
            var itemData = GetData();
            var itemRef = CreateRef();
            var itemUI = Object.Instantiate(context.ItemUIPrefab, context.Container);
            itemUI.SetData(itemData, itemRef);
        }

        private LinhDan GetData()
        {
            return LuyenDanResources.Instance.FindLinhDan(Id);
        }

        public string GetRichText()
        {
            var item = GetData();
            var core = CoreConfig.Instance;
            return
                $"<color=#{ColorUtility.ToHtmlStringRGBA(core.FindQuality(item.Tier).Color)}>{item.Name} x{Count}</color>";
        }
    }
}