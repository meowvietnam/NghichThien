using LuyenDan;
using Network;
using System.Linq;
using CreateCharacter;
using static UnityEngine.Analytics.IAnalytic;

namespace AllMaps.ItemProvider
{
    public class HerbProvider : ItemProvider<HerbData>, IUseItemProvider
    {
        public override ItemType SupportedType => ItemType.Herb;

        public override void Display(ToolTipContext ctx)
        {
            base.Display(ctx);

            ctx.TypeText.text = "Dược Liệu";
            ctx.DescriptionText.text = "Thảo dược có thể dùng luyện chế đan dược";
            if (ctx.IItemData is HerbData duocLieu)
            {
                ctx.DescTextItem.transform.parent.gameObject.SetActive(true);
                ctx.DescTextItem.text =
                    "<color=#ffb143>【Chủ Dược】 </color>" + $"{duocLieu.ChuDuoc} " +
                    "<color=#ffb143>【Phụ Dược】 </color>" + $"{duocLieu.PhuDuoc} " +
                    "<color=#ffb143>【Dược Dẫn】 </color>" + $"{duocLieu.ThuocDan}";
            }
            ctx.QualityText.text = GameManager.TextWithTier[ctx.IItemData.Tier];
        }   
        public override HerbData GetDataItemByIdRef(ushort Id)
        {
            return LuyenDanResources.Instance.FindHerb(Id);
        }

        public override int GetPriceItemSell(ushort Id)
        {
            return GameManager.PriceHerbWithTier[GetDataItemByIdRef(Id).Tier];
        }
        public override int GetCountOwner(ushort id)
        {
            var item = Client.Instance.CharacterData.InventoryManager.Herbs.FirstOrDefault(x => x.Id == id);
            return item != null?item.Count : base.GetCountOwner(id);
        }


        public void UseItem(ItemInventory itemInventory)
        {
            NotifyGroup.Instance?.Show("Sử dụng để luyện đan tại Phòng luyên đan!");
        }
    }
}
