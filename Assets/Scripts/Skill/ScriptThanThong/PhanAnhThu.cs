using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(PhanAnhThu), menuName = "Skill/Thần Thông/" + nameof(PhanAnhThu))]
public class PhanAnhThu : ThanThong, IMemorySerializable<PhanAnhThu>
{
    [SerializeField] private int[] buffValues = new int[] { 3, 4, 5, 7, 10 };
    [SerializeField] private int require = 2;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int count = (int)caster.thanThongUseThisTurn.Count / require;
        int dame = count * buffValues[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Trong hiệp này, mỗi sử dụng {GetColoredText($"{require} lần", "red")} thần thông thì tạo thành {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref PhanAnhThu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}