using LongDH.LongDHModules;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThaiAmKyMonTran), menuName = "Skill/Thần Thông/" + nameof(ThaiAmKyMonTran))]
public class ThaiAmKyMonTran : TranPhapTT, IMemorySerializable<ThaiAmKyMonTran>
{
    [SerializeField]
    private int[] buffValues = new int[]{2, 3, 4, 5, 6};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
    }

    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Bắt đầu hiệp, đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThaiAmKyMonTran value)
    {
        TranPhapTT baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}