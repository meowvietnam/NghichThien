using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KhoaiKiem), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(KhoaiKiem))]
public class KhoaiKiem : NgoDaoCombat, IMemorySerializable<KhoaiKiem>
{
    [SerializeField]
    private int buff = 1;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire))
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguKiemHiep, buff);
        }
    }

    public override string GetDes()
    {
        return $"Sử dụng kiếm hệ thần thông sát thương, thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NguKiemHiep)}";
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

    public void Deserialize(IReader reader, ref KhoaiKiem value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}