
using LuyenDan;
using LuyenKhi;
using Network;
using System.Linq;
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
        public override int GetCountOwner(ushort id)
        {
            var item = Client.Instance.CharacterData.InventoryManager.Materials.FirstOrDefault(x => x.Id == id);
            return item != null ? item.Count : base.GetCountOwner(id);
        }
        public override LuyenKhi.Material GetDataItemByIdRef(ushort Id)
        {
            return LuyenKhiResources.Instance.FindMaterial(Id);

        }

        public override int GetPriceItemSell(ushort Id)
        {
            return  GameManager.PriceVatLieuWithTier[GetDataItemByIdRef(Id).Tier];

        }
    }
}