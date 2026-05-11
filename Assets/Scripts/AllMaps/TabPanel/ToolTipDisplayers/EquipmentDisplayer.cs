using System.Linq;
using AllMaps.TabPanel.ToolTipDisplayers;
using LuyenDan;
using LuyenKhi;
using Network;
using UnityEngine;

public abstract class EquipmentDisplayer : ItemDisplayer
{
    public override abstract  ItemType SupportedType { get; }
    public abstract EquipmentType EquipemtType { get; }

    public override  void Display(ToolTipContext ctx)
    {
        ctx.TypeText.text = $"{GetTextNameEquipment(ctx.EquipmentData.Item.Type)}";
        ctx.DescriptionText.text = $"{GetTextNameEquipment(ctx.EquipmentData.Item.Type)} do {ctx.EquipmentData.Name} luyện thành";
        ctx.DescTextItem.transform.parent.gameObject.SetActive(true);
        ctx.DescTextItem.text = $"{GameManager.GetDescEquipment(ctx.EquipmentData.Effects.ToList(), EquipemtType)}";
        ctx.QualityText.text = ctx.EquipmentData.GradeConfig.Name;
        ctx.SetPriceText?.Invoke(GetPrice(ctx));
    }
    protected virtual string GetTextNameEquipment(ArtifactType artifactType)
    {
        return $" - {GetArtifactName(artifactType)}";
    }
    public static string GetArtifactName(ArtifactType artifactType)
    {
        string artifactName = "";
        switch (artifactType)
        {
            case ArtifactType.PhiKiem:
                artifactName = "Phi Kiếm";
                break;
            case ArtifactType.BaoChung:
                artifactName = "Bảo Chung";

                break;
            case ArtifactType.ChiHoan:
                artifactName = "Chỉ Hoàn";

                break;
            case ArtifactType.PhiCham:
                artifactName = "Phi Châm";

                break;
            case ArtifactType.BaoHap:
                artifactName = "Bảo Hạp";

                break;
            case ArtifactType.TruongBao:
                artifactName = "Trường Bào";

                break;
            case ArtifactType.GiapTru:
                artifactName = "Giáp Trụ";

                break;
            case ArtifactType.Linh:
                artifactName = "Linh";

                break;
            case ArtifactType.BaoLenh:
                artifactName = "Bảo Lệnh";
                break;
            case ArtifactType.BaoAn:
                artifactName = "Bảo Ấn";

                break;
            default:
                break;
        }
        return artifactName;
    }
   
    public override int GetPriceItemSell(DataItemSnapshot ctx)
    {
        int totalPrice = 0;
        foreach (var item in ctx.EquipmentData.Effects)
        {
            var materials = ctx.EquipmentData.GetMaterialsWithEffectType(item.Type);
            int currentPrice = materials.Sum(x=> GameManager.PriceVatLieuWithTier[x.Tier]);
            // 1. Lấy khoảng Min/Max và giá trị thực tế của Equipment
            var (min, max) = ctx.EquipmentData.GetMinMaxValue(item.Type);
            int actualValue = item.Value; // Giả sử đây là hàm lấy tổng value thực tế sau khi random

            // 2. Tính toán Percent
            float percent = 0;
            if (max > min)
            {
                float mid = (min + max) / 2f;
                // Công thức: (Giá trị thực - Điểm giữa) / (Khoảng cách từ giữa ra biên) * 0.5 (tức 50%)
                percent = (actualValue - mid) / (max - min);
                // percent lúc này sẽ nằm trong khoảng [-0.5 đến 0.5]
            }

            // 3. Tính giá cuối cùng
            // Công thức: Total + Total * percent
            totalPrice += Mathf.RoundToInt(currentPrice * (1 + percent));
        }



        return totalPrice;
    }

    public override int GetPriceItemBuy(DataItemSnapshot ctx)
    {
        return GetPriceItemSell(ctx);
    }
}



