using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ManKiem), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ManKiem))]
public class ManKiem : NgoDaoCombat, IMemorySerializable<ManKiem>
{
    [SerializeField] private int buff = 20;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire))
        {
            Action(caster, target);
        }
    }

    public virtual void Action(CharacterCombat caster, CharacterCombat target)
    {
        target.AddDamageData(new List<int> { target.GetDameBuffForTarget() * buff / 100 });
    }

    public void ObserverEffect3(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        BaseSkill.ETypeSkill[] eTypeSkills = (BaseSkill.ETypeSkill[])data[3];
        if (caster == null)
            return;
        // kiểm tra dame được truyền vào có phải là mộc linh khí không
        if (caster.ngoDaoCombats.Any(x => x.Id == this.Id) && eTypeSkills != null &&
            BaseSkill.CheckIsEqualETypeSkill(eTypeSkills, eTypeRequire))
        {
            Action(caster, target);
        }
    }

    public override string GetDes()
    {
        return $"Tạo thành kiếm hệ tổn thương {BaseSkill.GetColoredText($"+{buff}%", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME_BY_STATIC, ObserverEffect3);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ManKiem value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}