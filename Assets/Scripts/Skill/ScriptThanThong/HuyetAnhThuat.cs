using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HuyetAnhThuat), menuName = "Skill/Thần Thông/" + nameof(HuyetAnhThuat))]
public class HuyetAnhThuat : ThanThong, IMemorySerializable<HuyetAnhThuat>
{
    [SerializeField]
    private int[] tieuHaos = new int[]{5, 6, 9, 13, 18};
    [SerializeField]
    private int[] buffValues = new int[]{10, 12, 18, 26, 36};
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.TakeDameByStatic(null, new System.Collections.Generic.List<int>{tieuHaos[GetIndexLevelThanThong(caster)]}, false, null, false);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Anh, buffValues[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Tiêu hao {GetColoredText($"{tieuHaos[level - 1]}", "red")} điểm HP. Bản thân thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Anh)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HuyetAnhThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}