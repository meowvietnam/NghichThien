using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThongMinhKiem), menuName = "Skill/Thần Thông/" + nameof(ThongMinhKiem))]
public class ThongMinhKiem : ThanThong, IMemorySerializable<ThongMinhKiem>
{
    [SerializeField]
    private int[] dames = new int[]{6, 12, 18, 30, 48};
    [SerializeField]
    private int[] buffValues = new int[]{3, 4, 5, 7, 12};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        caster.TakeDameByStatic(null, new System.Collections.Generic.List<int>{dame}, false, null, false);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemTam, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return $"Bản thân nhận {GetColoredText($"+{dames[level - 1]}", "red")} điểm {GetKeyWord(EKeyWord.ChanThucTonThuong)}. Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemTam)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThongMinhKiem value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}