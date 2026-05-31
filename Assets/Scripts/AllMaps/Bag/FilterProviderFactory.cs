using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using LuyenDan;
using LuyenKhi;
using Network;
using Base;
using LongDH.LongDHModules;
using LongDHModules;

public interface IFilterProvider
{
    int OptionCount { get; }
    string GetOptionText(int index);
    bool Matches(ItemRef item, ItemType currentType, Base.Tier tierCurrent, BaseSkill.ETierSkill eTierSkillCurrent, EquipmentTier eTierEquipmentCurrent);
}

public static class FilterProviderFactory
{
    public static IFilterProvider Create(ItemType itemType)
    {
        if (itemType == ItemType.ThanThong || itemType == ItemType.CongPhap)
            return new SkillFilterProvider();

        if (itemType == ItemType.VuKhi || itemType == ItemType.PhongCu || itemType == ItemType.TrangSuc)
            return new EquipmentFilterProvider();

        return new DefaultFilterProvider();
    }
}

public class SkillFilterProvider : IFilterProvider
{
    private static readonly List<BaseSkill.ETierSkill> listTierSkill = new List<BaseSkill.ETierSkill>
    {
        BaseSkill.ETierSkill.NhanGiaiHa,
        BaseSkill.ETierSkill.NhanGiaiTrung,
        BaseSkill.ETierSkill.NhanGiaiThuong,
        BaseSkill.ETierSkill.DiaGiaiHa,
        BaseSkill.ETierSkill.DiaGiaiTrung,
        BaseSkill.ETierSkill.DiaGiaiThuong,
        BaseSkill.ETierSkill.ThienGiaiHa,
        BaseSkill.ETierSkill.ThienGiaiTrung,
        BaseSkill.ETierSkill.ThienGiaiThuong
    };

    public int OptionCount => listTierSkill.Count;

    public string GetOptionText(int index)
    {
        return BaseSkill.GetTextTierSkill(listTierSkill[index]);
    }

    public bool Matches(ItemRef item, ItemType currentType, Tier tierCurrent, BaseSkill.ETierSkill eTierSkillCurrent, EquipmentTier eTierEquipmentCurrent)
    {
        if (currentType != ItemType.ThanThong && currentType != ItemType.CongPhap)
            return false;

        BaseSkill skill = currentType == ItemType.ThanThong
            ? Pool.Get<GameManager>().FindThanThongWithItemRef(item)
            : Pool.Get<GameManager>().FindCongPhapWithItemRef(item);

        return skill != null && skill.eTierSkill == eTierSkillCurrent;
    }
}

public class EquipmentFilterProvider : IFilterProvider
{
    private static readonly List<EquipmentTier> listTierEquipment = new List<EquipmentTier>
    {
        EquipmentTier.PhuKhi,
        EquipmentTier.PhapKhi,
        EquipmentTier.PhapBao,
        EquipmentTier.ThuanDuong,
        EquipmentTier.ThongThien,
    };

    public int OptionCount => listTierEquipment.Count;

    public string GetOptionText(int index)
    {
        return GameManager.TextWithTierEquirement.GetValueOrDefault(listTierEquipment[index]);
    }

    public bool Matches(ItemRef item, ItemType currentType, Tier tierCurrent, BaseSkill.ETierSkill eTierSkillCurrent, EquipmentTier eTierEquipmentCurrent)
    {
        if (currentType != ItemType.VuKhi && currentType != ItemType.PhongCu && currentType != ItemType.TrangSuc)
            return false;

        CreateCharacter.Equipment equipment = currentType switch
        {
            ItemType.VuKhi => Client.Instance.CharacterData.InventoryManager.VuKhis.FirstOrDefault(x => x.SelfId == item.Id),
            ItemType.PhongCu => Client.Instance.CharacterData.InventoryManager.PhongCus.FirstOrDefault(x => x.SelfId == item.Id),
            ItemType.TrangSuc => Client.Instance.CharacterData.InventoryManager.TrangSucs.FirstOrDefault(x => x.SelfId == item.Id),
            _ => null
        };

        return equipment != null && equipment.Item.Tier == eTierEquipmentCurrent;
    }
}

public class DefaultFilterProvider : IFilterProvider
{
    private static readonly List<Tier> listTier = new List<Tier>
    {
        Tier.NhatPham,
        Tier.NhiPham,
        Tier.TamPham,
        Tier.TuPham,
        Tier.NguPham,
        Tier.LucPham
    };

    public int OptionCount => listTier.Count;

    public string GetOptionText(int index)
    {
        return GameManager.TextWithTier.GetValueOrDefault(listTier[index]);
    }

    public bool Matches(ItemRef item, ItemType currentType, Tier tierCurrent, BaseSkill.ETierSkill eTierSkillCurrent, EquipmentTier eTierEquipmentCurrent)
    {
        if (currentType == ItemType.Material)
        {
            var data = LuyenKhiResources.Instance.FindItem(item);
            return data != null && data.Tier == tierCurrent;
        }

        var dataItem = LuyenDanResources.Instance.FindItem(item);
        return dataItem != null && dataItem.Tier == tierCurrent;
    }
}
