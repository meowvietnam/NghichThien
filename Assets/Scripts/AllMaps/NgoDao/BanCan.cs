using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(BanCan), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(BanCan))]
public class BanCan : NgoDaoCombat, IMemorySerializable<BanCan>
{
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out SecureInt trienNhieu))
        {
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, trienNhieu);
        }
    }

    public override string GetDes()
    {
        return $"Kết thúc hiệp, đối thủ số tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)} gấp đôi";
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

    public void Deserialize(IReader reader, ref BanCan value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}