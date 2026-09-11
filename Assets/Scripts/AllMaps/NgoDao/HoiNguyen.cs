using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HoiNguyen), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(HoiNguyen))]
public class HoiNguyen : NgoDaoCombat, IMemorySerializable<HoiNguyen>
{
    [SerializeField] private int buff = 5;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.Healing(buff);
    }

    public override string GetDes()
    {
        return
            $"Mỗi sử dụng thần thông gây tổn thương, khôi phục {BaseSkill.GetColoredText($"+{buff}", "red")} điểm HP";
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

    public void Deserialize(IReader reader, ref HoiNguyen value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}