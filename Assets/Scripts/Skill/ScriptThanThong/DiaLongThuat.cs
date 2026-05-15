using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(DiaLongThuat), menuName = "Skill/Thần Thông/" + nameof(DiaLongThuat))]
public class DiaLongThuat : ThanThong, BaseSkill.LienKich, IMemorySerializable<DiaLongThuat>
{
    [SerializeField]
    private int[] dameValues = new int[]{7, 8, 12, 24, 50};
    [SerializeField]
    private int[] buffValues = new int[]{3, 6, 9, 15, 30};
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dameValues[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        int hoTrao = buffValues[GetIndexLevelThanThong(caster)];
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTrao);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương.{GetKeyWord(EKeyWord.LienKich)}, nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Hoa)} hệ thần thông thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref DiaLongThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}