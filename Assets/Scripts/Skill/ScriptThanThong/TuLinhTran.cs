using LongDH.LongDHModules;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TuLinhTran), menuName = "Skill/Thần Thông/" + nameof(TuLinhTran))]
public class TuLinhTran : TranPhapTT, IMemorySerializable<TuLinhTran>
{
    [SerializeField]
    private int[] hapThus;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        caster.totalKhiRandom += hapThus[GetIndexLevelThanThong(caster)];
    }

    public override void ObserverEffect(params object[] data)
    {
        return;
    }

    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Giai đoạn hấp thụ linh khí {GetColoredText($"{hapThus[level - 1]}", "red")}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TuLinhTran value)
    {
        TranPhapTT baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}