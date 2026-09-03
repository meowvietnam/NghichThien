using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TinhKhi), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TinhKhi))]
public class TinhKhi : NgoDaoCombat, IMemorySerializable<TinhKhi>
{
    [SerializeField]
    private int buff = 25;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (target.oldTurnDameTaked != null && target.oldTurnDameTaked.Value <= 0)
        {
            target.AddDamageData(new List<int>{target.GetDameBuffForTarget() * buff / 100});
        }
    }

    public override string GetDes()
    {
        return $"Nếu hiệp trước, ngươi chưa sử dụng bất kì thần thông gây tổn thương nào thì hiệp này ngươi tạo thành thần thông tăng lên {BaseSkill.GetColoredText($"{buff}%", "red")} điểm sát thương";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TinhKhi value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}