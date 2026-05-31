using LongDH.LongDHModules;
using UnityEngine;
using System;
using LongDHModules;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(DaiNguHanhTran), menuName = "Skill/Thần Thông/" + nameof(DaiNguHanhTran))]
public class DaiNguHanhTran : TranPhapTT, IMemorySerializable<DaiNguHanhTran>
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
    }

    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        Pool.Get<CombatManager>().transform.GetComponentInChildren<PlayerUICombat>().linhKhiSelectUI.SetActive(true);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Bắt đầu hiệp, Lựa chọn một điểm linh khí bất kì để hấp thụ";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref DaiNguHanhTran value)
    {
        TranPhapTT baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}