using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TramThien), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TramThien))]
public class TramThien : ManKiem, IMemorySerializable<TramThien>
{
    public override void Action(CharacterCombat caster, CharacterCombat target)
    {
        if (target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out SecureInt hoTrao))
        {
            base.Action(caster, target);
        }
    }

    public override string GetDes()
    {
        return $"Nếu đối thủ có {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}, {base.GetDes()}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TramThien value)
    {
        ManKiem baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}