using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(DaiThonDiemThuat), menuName = "Skill/Thần Thông/" + nameof(DaiThonDiemThuat))]
public class DaiThonDiemThuat : ThanThong, IMemorySerializable<DaiThonDiemThuat>
{
    [SerializeField] private int[] healValues = new int[] { 2, 3, 4, 5, 6 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Diem, out SecureInt diem);
        int heal = diem * healValues[GetIndexLevelThanThong(caster)];
        caster.Healing(heal);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Diem, diem);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Diem, out SecureInt diem);
        if (diem > 0 && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return
            $"Bản thân gỡ ra trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)} mỗi tầng tiêu hao thì khôi phục {GetColoredText($"+{healValues[level - 1]}", "red")} điểm Hp";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref DaiThonDiemThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}