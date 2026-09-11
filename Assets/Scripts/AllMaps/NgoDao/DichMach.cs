using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(DichMach), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(DichMach))]
public class DichMach : NgoDaoCombat, IMemorySerializable<DichMach>
{
    [SerializeField] private int buff = 1;

    public override void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        if (caster == null)
            return;
        if (caster.ngoDaoCombatsDynamic.Any(x => x.Id == this.Id))
        {
            ExecuteNgoDao(caster, target, data);
        }
    }

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.RemoveNgoDaoCombatDynamic(this);
        caster.totalKhiRandom -= buff;
    }

    public override string GetDes()
    {
        return $"Bắt đầu chiến đấu, hấp thụ {BaseSkill.GetColoredText($"+{buff}", "red")} điểm linh khi";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
        caster.totalKhiRandom += buff;
        caster.AddNgoDaoCombatDynamic(this);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref DichMach value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}