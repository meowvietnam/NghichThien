using Base;
using LuyenDan;
using LuyenDan.Inventory;
using MemoryPack;
using UnityEngine;

namespace CreateCharacter
{
    [MemoryPackable]
    public partial class MailRewardFurnace : IMailReward
    {
        public FurnaceData FurnaceData { get; set; }
        public void ClaimReward(CharacterData characterData)
        {
            characterData.InventoryManager.Furnaces.Add(FurnaceData);
        }
        public void ShowUI(MailRewardUIContext context)
        {
            var itemData = GetItemData();
            context.RewardUI.SetData(itemData);
        }
        private IItemData GetItemData()
        {
            return LuyenDanResources.Instance.FindItem(FurnaceData);
        }
        public string GetRichText()
        {
            var core = CoreConfig.Instance;
            var item = GetItemData();
            var text = $"<color=#{ColorUtility.ToHtmlStringRGBA(core.FindQuality(item.Tier).Color)}>{item.Name}</color>";
            return text;
        }
    }
}