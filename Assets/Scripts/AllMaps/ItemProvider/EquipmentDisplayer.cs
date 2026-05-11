using System.Linq;
using AllMaps.ItemProvider;
using CreateCharacter;
using LuyenDan;
using LuyenKhi;
using Network;
using UnityEngine;

public abstract class EquipmentDisplayer : ItemProvider<CreateCharacter.Equipment>
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

        var equipment = GetDataItemById(id);
        if (equipment == null) return 0;

        float finalPrice = 0;

        // Duyệt qua từng Effect để tính toán
        foreach (var effect in equipment.Effects)
        {
            // 1. Lấy danh sách nguyên liệu tạo ra Effect này
            var materials = equipment.GetMaterialsWithEffectType(effect.Type);
            if (materials == null || materials.Length == 0) continue;

            // 2. Tính tổng giá gốc của nhóm nguyên liệu này
            int groupMaterialPrice = materials.Sum(m => GameManager.PriceVatLieuWithTier[m.Tier]);

            // 3. Tính toán chất lượng (Quality Percent)
            var (min, max) = equipment.GetMinMaxValue(effect.Type);
            int actualValue = effect.Value;

            float qualityBonusPercent = 0;
            if (max > min)
            {
                float mid = (min + max) / 2f;
                // Tính độ lệch so với trung bình: Trả về từ -0.5 đến 0.5
                qualityBonusPercent = (float)(actualValue - mid) / (max - min);
                // Giới hạn: Tệ nhất giảm 40%, Xịn nhất tăng 60% giá trị nguyên liệu
                qualityBonusPercent = Mathf.Clamp(qualityBonusPercent, -0.4f, 0.6f);
            }

            // 4. Tính giá trị sử dụng (Value Bonus)
            // Hệ số: Lấy giá trung bình của các nguyên liệu trong nhóm chia 10 (tương đương 10%)
            // Điều này giúp Tier càng cao, mỗi điểm chỉ số càng đáng giá.
            float avgMaterialPrice = (float)groupMaterialPrice / materials.Length;
            int goldPerPoint = Mathf.Max(1, Mathf.RoundToInt(avgMaterialPrice / 10f));
            int valueBonus = actualValue * goldPerPoint;

            // 5. Cộng dồn vào tổng giá: (Giá NL đã điều chỉnh theo chất lượng) + (Tiền theo điểm chỉ số)
            finalPrice += (groupMaterialPrice * (1 + qualityBonusPercent)) + valueBonus;
        }

        // Đảm bảo không bao giờ trả về số âm hoặc quá nhỏ (Min là 10% tổng giá nguyên liệu gốc)
        int absoluteMinPrice = equipment.Effects.Sum(e =>
            equipment.GetMaterialsWithEffectType(e.Type).Sum(m => GameManager.PriceVatLieuWithTier[m.Tier])) / 10;

        return Mathf.Max(Mathf.RoundToInt(finalPrice), absoluteMinPrice);
    }

    public override int GetPriceItemBuy(ushort id)
    {
        return GetPriceItemSell(id);
    }
}



