using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TinhDieuNgungHonQuyet), menuName = "Skill/Thần Thông/" + nameof(TinhDieuNgungHonQuyet))]
public class TinhDieuNgungHonQuyet : ThanThong, IMemorySerializable<TinhDieuNgungHonQuyet>
{
    [SerializeField]
    private int[] buffValues = new int[]{3, 6, 9, 17, 35};
    [SerializeField]
    private int[] debuffValues = new int[]{1, 2, 3, 4, 5};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TinhHonChiLuc, buffValues[GetIndexLevelThanThong(caster)]);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TreKhi, debuffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TinhHonChiLuc)}. Đồng thời nhận phải {GetColoredText($"+{debuffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TreKhi)}.";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TinhDieuNgungHonQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}