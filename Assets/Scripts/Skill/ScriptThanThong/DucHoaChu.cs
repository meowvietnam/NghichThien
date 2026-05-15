using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(DucHoaChu), menuName = "Skill/Thần Thông/" + nameof(DucHoaChu))]
public class DucHoaChu : ThanThong, IMemorySerializable<DucHoaChu>
{
    [SerializeField]
    private int[] hoaDiemBuffs = new int[]{8, 10, 14, 22, 38};
    [SerializeField]
    private int[] thieuDotBuffs = new int[]{1, 2, 4, 5, 7};
    [SerializeField]
    private float delayEffect = 0.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int hoaDiem = hoaDiemBuffs[GetIndexLevelThanThong(caster)];
        int thieuDot = thieuDotBuffs[GetIndexLevelThanThong(caster)];
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoaDiem, hoaDiem);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, thieuDot);
        ShowSkillEffectWorld(caster, delayEffect);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoaDiem, out SecureInt hoaDiem);
        if (hoaDiem > 0 && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return $"Bản thân đang trong trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoaDiem)} thì thu hoạch được {GetColoredText($"+{hoaDiemBuffs[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoaDiem)} và {GetColoredText($"+{thieuDotBuffs[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref DucHoaChu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}