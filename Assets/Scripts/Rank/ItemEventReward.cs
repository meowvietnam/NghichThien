using System;
using Base;
using CreateCharacter;
using LuyenDan;
using LuyenDan.Inventory;
using LuyenKhi;
using MemoryPack;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rank
{
    [MemoryPackable]
    public partial class ItemEventReward : IEventReward
    {
        public ItemRef Item { get; set; }

        public void Claim(CharacterData charData)
        {
            charData.InventoryManager.Add(Item);
        }

        public void ShowUI(RewardUIContext context)
        {
            var itemData = GetData();
            var itemUI = Object.Instantiate(context.ItemUIPrefab, context.Container);
            itemUI.SetData(itemData, Item);
        }

        private IItemData GetData()
        {
            var id = Item.Id;
            var type = Item.Type;
            switch (type)
            {
                case ItemType.LinhDan:
                    return LuyenDanResources.Instance.FindLinhDan(id);
                case ItemType.Herb:
                    return LuyenDanResources.Instance.FindHerb(id);
                case ItemType.Material:
                    return LuyenKhiResources.Instance.FindMaterial(id);
                default:
                    throw new Exception("Không hỗ trợ item type này");
            }
        }

        public string GetRichText()
        {
            var item = GetData();
            var core = CoreConfig.Instance;
            return
                $"<color=#{ColorUtility.ToHtmlStringRGBA(core.FindQuality(item.Tier).Color)}>{item.Name} x{Item.Count}</color>";
        }
    }
}