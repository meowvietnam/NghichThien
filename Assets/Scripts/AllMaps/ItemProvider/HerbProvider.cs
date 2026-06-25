using LuyenDan;
using static UnityEngine.Analytics.IAnalytic;

namespace AllMaps.ItemProvider
{
    public class HerbProvider : ItemProvider<HerbData>
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

     
    }
}
