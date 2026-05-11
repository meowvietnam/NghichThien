using System.Linq;
using AllMaps.ItemProvider;
using CreateCharacter;
using LuyenDan;
using LuyenKhi;
using Network;
using UnityEngine;

public abstract class EquipmentProvider : ItemProvider<CreateCharacter.Equipment>
{
    public override abstract  ItemType SupportedType { get; }
    public abstract EquipmentType EquipemtType { get; }

    protected InventoryManager _inventoryManager => Client.Instance.CharacterData.InventoryManager;

    public abstract override CreateCharacter.Equipment GetDataItemById(ushort Id);

    public override  void Display(ToolTipContext ctx)
    {
        base.Display(ctx);
        ctx.TypeText.text = $"{GetTextNameEquipment(ctx.EquipmentData.Item.Type)}";
        ctx.DescriptionText.text = $"{GetTextNameEquipment(ctx.EquipmentData.Item.Type)} do {ctx.EquipmentData.Name} luyện thành";
        ctx.DescTextItem.transform.parent.gameObject.SetActive(true);
        ctx.DescTextItem.text = $"{GameManager.GetDescEquipment(ctx.EquipmentData.Effects.ToList(), EquipemtType)}";
        ctx.QualityText.text = ctx.EquipmentData.GradeConfig.Name;
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

    public override int GetPriceItemSell(ushort id)
    {
        var equipmentData = GetDataItemById(id);
        if (equipmentData == null) return 0;

        int totalPrice = 0;

        foreach (var item in equipmentData.Effects)
        {
            var materials = equipmentData.GetMaterialsWithEffectType(item.Type);
            int materialCost = materials.Sum(x => GameManager.PriceVatLieuWithTier[x.Tier]);

            // 1. Tính toán Percent từ chỉ số Craft (Từ -0.5 đến 0.5)
            var (min, max) = equipmentData.GetMinMaxValue(item.Type);
            float craftPercent = 0;
            if (max > min)
            {
                float mid = (min + max) / 2f;
                craftPercent = (item.Value - mid) / (max - min);
            }

            // 2. Tính toán "Giá trị cơ bản sau khi đã Bonus theo Tier"
            // GradeBonus đóng vai trò nâng tầm giá trị nguyên liệu lên
            float gradeBonus = equipmentData.GradeConfig.MinGrade / 100f;
            float basePriceWithTier = materialCost * (1 + gradeBonus);

            // 3. Nhân thêm phần Craft Percent vào CẢ CỤM đã có Bonus
            // Công thức: [Giá sau Tier] + [Giá sau Tier * CraftPercent]
            // Hoặc viết gọn: basePriceWithTier * (1 + craftPercent)
            totalPrice += Mathf.RoundToInt(basePriceWithTier * (1 + craftPercent));
        }

        return totalPrice;
    }

    public override int GetPriceItemBuy(ushort id)
    {
        return GetPriceItemSell(id);
    }
}



