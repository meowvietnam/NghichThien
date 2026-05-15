using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ChichHoa), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ChichHoa))]
public class ChichHoa : NgoDaoCombat, IMemorySerializable<ChichHoa>
{
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        CombatStatic.ThieuDot(caster, target);
    }

    public override string GetDes()
    {
        return $"Kết thúc hiệp, phát động một lần {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} cho đối thủ";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ChichHoa value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}