using AllMaps.ItemProvider;
using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenKhi;
using Network;
using System.Collections.Generic;
using System.Linq;
using LongDHModules;
using UnityEngine;

public abstract class EquipmentProvider : ItemProvider<CreateCharacter.Equipment>, INetworkEvent.IEquipEquipment,
    IUseItemProvider
{
    public override int BaseValueLevelWithTier => 6;

    public override abstract ItemType SupportedType { get; }
    public abstract EquipmentType EquipemtType { get; }

    protected InventoryManager _inventoryManager => Client.Instance.CharacterData.InventoryManager;

    public abstract override CreateCharacter.Equipment GetDataItemByIdRef(ushort Id);

    public override void Display(ToolTipContext ctx)
    {
        base.Display(ctx);
        ctx.TypeText.text = $"{GetTextNameEquipment(ctx.EquipmentData.Item.Type)}";
        ctx.DescriptionText.text =
            $"{GetTextNameEquipment(ctx.EquipmentData.Item.Type)} do {ctx.EquipmentData.Name} luyện thành";
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
        var equipmentData = GetDataItemByIdRef(id);
        if (equipmentData == null) return 0;

        int totalPrice = 0;

        foreach (var item in equipmentData.Effects)
        {
            var materials = equipmentData.GetMaterialsWithEffectType(item.Type);
            int materialCost = materials.Sum(x => GameManager.PriceVatLieuWithTier[x.Tier]);

            var (min, max) = equipmentData.GetMinMaxValue(item.Type);
            float craftPercent = 0;
            if (max > min)
            {
                float mid = (min + max) / 2f;
                craftPercent = (item.Value - mid) / (max - min);
            }

            float gradeBonus = equipmentData.GradeConfig.MinGrade / 100f;
            float basePriceWithTier = materialCost * (1 + gradeBonus);

            totalPrice += Mathf.RoundToInt(basePriceWithTier * (1 + craftPercent));
        }

        return totalPrice;
    }

    public override int GetPriceItemBuy(ushort id)
    {
        return GetPriceItemSell(id);
    }

    public void UseItem(ItemInventory itemInventory)
    {
        EquipmentType equipmentType = GameManager.ItemTypeToEquipemtType.GetValueOrDefault(itemInventory.itemRef.Type);
        ItemInventory itemEquipmentSlot = null;
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                itemEquipmentSlot = Pool.Get<TabPanel>().itemWeapon;
                break;
            case EquipmentType.Armor:
                itemEquipmentSlot = Pool.Get<TabPanel>().itemPhongCu;

                break;
            case EquipmentType.Jewelry:
                itemEquipmentSlot = Pool.Get<TabPanel>().itemTrangSuc;

                break;
            default:
                break;
        }

        Pool.Get<TabPanel>().LoadEquipment(itemEquipmentSlot, itemInventory.itemRef.Type, itemInventory.itemRef.Id);

        Observer.Notify(EventList.Network.EQUIP_EQUIPMENT, this, itemInventory.itemRef.Id,
            equipmentType); // id equipment này là self id
    }

    public void SuccessEquipEquipment()
    {
        NotifyGroup.Instance.Show("Trang bị thành công");
    }

    public void FailEquipEquipment(string message)
    {
        NotifyGroup.Instance.Show(message);
    }
}