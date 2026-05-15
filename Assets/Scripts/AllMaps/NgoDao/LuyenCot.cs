using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(LuyenCot), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(LuyenCot))]
public class LuyenCot : NgoDaoCombat, IMemorySerializable<LuyenCot>
{
    [SerializeField]
    private int buff = 1;
    public override void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        if (target == null)
            return;
        if (target.ngoDaoCombats.Any(x => x.Id == this.Id))
        {
            ExecuteNgoDao(caster, target, data);
        }
    }

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.GiamTonThuong, buff);
    }

    public override string GetDes()
    {
        return $"Mỗi lần nhận tổn thương thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.GiamTonThuong)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LuyenCot value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}