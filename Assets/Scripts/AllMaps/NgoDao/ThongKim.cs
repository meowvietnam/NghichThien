using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThongKim), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ThongKim))]
public class ThongKim : NgoDaoCombat, IMemorySerializable<ThongKim>
{
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (target.characterStatsTemp.hp < target.characterStatsTemp.hpMax * 50 / 100)
        {
            target.AddDamageData(new List<int>{target.GetDameBuffForTarget()});
        }
        else
        {
            float percentHpLost = (target.characterStatsTemp.hpMax - target.characterStatsTemp.hp) / target.characterStatsTemp.hpMax;
            target.AddDamageData(new List<int>{(int)(target.GetDameBuffForTarget() * percentHpLost)});
        }
    }

    public override string GetDes()
    {
        return $"Đối thủ mỗi tổn thất 1% lượng máu, nhận tổn thương cũng tăng lên 1%. Nếu đối thủ HP thấp hơn 50% thì nhận tổn thương gấp đôi";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThongKim value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}