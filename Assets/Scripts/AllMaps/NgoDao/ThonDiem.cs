using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThonDiem), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ThonDiem))]
public class ThonDiem : NgoDaoCombat, IMemorySerializable<ThonDiem>
{
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        EBuffAndDeBuff eType = (EBuffAndDeBuff)data[1];
        int newValue = (int)data[2];
        int oldValue = (int)data[3];
        if (eType == EBuffAndDeBuff.HoaDiem && newValue <= 0) // thoát khỏi Hỏa Diễm
        {
            caster.Healing(oldValue);
        }
    }

    public override string GetDes()
    {
        return
            $"Thoát khỏi trạng thái {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoaDiem)} thì khôi phục HP một lượng tương đương số tầng";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.REMOVE_BUFF, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThonDiem value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}