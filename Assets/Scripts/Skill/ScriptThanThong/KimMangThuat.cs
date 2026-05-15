using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KimMangThuat), menuName = "Skill/Thần Thông/" + nameof(KimMangThuat))]
public class KimMangThuat : ThanThong, BaseSkill.LienKich, IMemorySerializable<KimMangThuat>
{
    [SerializeField]
    private int[] dames;
    [SerializeField]
    private int[] dameBuffs;
    [SerializeField]
    private int noiThuong = 1;
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NoiThuong, noiThuong);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        int dame = dameBuffs[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill, true);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương, bản thân nhận phải  {GetColoredText($"+{noiThuong}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NoiThuong)}. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng Thổ hệ thần thông thì tăng lên {GetColoredText($"+{dameBuffs[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KimMangThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}