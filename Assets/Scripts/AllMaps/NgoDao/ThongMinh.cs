using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThongMinh), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ThongMinh))]
public class ThongMinh : NgoDaoCombat, IMemorySerializable<ThongMinh>
{
    [SerializeField] private int buff = 1;

    public override void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        if (target == null)
            return;
        if (target.ngoDaoCombats.Any(x => x.Id == this.Id))
        {
            if (BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire))
            {
                ExecuteNgoDao(caster, target, data);
            }
        }
    }

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddDamageData(new List<int> { -buff });
    }

    public void ObserverEffect2(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        BaseSkill.ETypeSkill[] eTypeSkills = (BaseSkill.ETypeSkill[])data[3];
        if (target == null)
            return;
        // kiểm tra dame được truyền vào có phải là mộc linh khí không
        if (target.ngoDaoCombats.Any(x => x.Id == this.Id) && eTypeSkills != null &&
            BaseSkill.CheckIsEqualETypeSkill(eTypeSkills, eTypeRequire))
        {
            ExecuteNgoDao(caster, target);
        }
    }

    public override string GetDes()
    {
        return $"Nhận tất cả tổn thương {BaseSkill.GetColoredText($"-{buff}%", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME_BY_STATIC, ObserverEffect2);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThongMinh value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}