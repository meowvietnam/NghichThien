using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(NguHanh), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(NguHanh))]
public class NguHanh : NgoDaoCombat, IMemorySerializable<NguHanh>
{
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThong = (ThanThong)data[2];
        if (thanThong is BaseSkill.LienKich lienKich)
        {
            lienKich.ExecuteLienKich(caster, target);
        }
    }

    public override string GetDes()
    {
        return
            $"Khi phát động ngũ hành {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.LienKich)} thì tự động phát động thêm một lần nữa";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.LIEN_KICH, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NguHanh value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}