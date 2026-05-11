
using LuyenDan;
using LuyenKhi;
using UnityEngine;

namespace AllMaps.ItemProvider
{
    public class MaterialProvider : ItemProvider<LuyenKhi.Material>
    {
        public override ItemType SupportedType => ItemType.Material;
        public override void Display(ToolTipContext ctx)
        {
            base.Display(ctx);
            var material = (LuyenKhi.Material)ctx.IItemData;
            ctx.TypeText.text = material.Type;
            ctx.DescriptionText.text = material.Desc1;
            ctx.DescTextItem.text = material.Desc2;
        }

        public override LuyenKhi.Material GetDataItemById(ushort Id)
        {
            return LuyenKhiResources.Instance.FindMaterial(Id);

        }

        public override int GetPriceItemSell(ushort Id)
        {
            return  GameManager.PriceVatLieuWithTier[GetDataItemById(Id).Tier];

        }
    }
}