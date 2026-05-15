using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TamTaiQuyNguyenKiem), menuName = "Skill/Thần Thông/" + nameof(TamTaiQuyNguyenKiem))]
public class TamTaiQuyNguyenKiem : AmDuongLuongNghiKiem, IMemorySerializable<TamTaiQuyNguyenKiem>
{
    public override void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{kiemKhi * buffValues[GetIndexLevelThanThong(caster)]}, eTypeSkill, true);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThienKiem, out SecureInt thienKiem);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DiaKiem, out SecureInt diaKiem);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NhanKiem, out SecureInt nhanKiem);
        if (thienKiem > 0 && diaKiem > 0 && nhanKiem > 0 && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    protected override string GetDescTuongSinh(int level)
    {
        return $"Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} thì tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";
    }

    protected override string GetDescRequire()
    {
        return $"Sở hữu {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThienKiem)}, {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DiaKiem)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NhanKiem)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TamTaiQuyNguyenKiem value)
    {
        AmDuongLuongNghiKiem baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}