using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(NgungSa), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(NgungSa))]
public class NgungSa : NgoDaoCombat, IMemorySerializable<NgungSa>
{
    [SerializeField]
    private int buff = 30;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThong = (ThanThong)data[2];
        if (thanThong is HoaCat)
        {
            caster.AddNgoDaoCombatDynamic(this);
        }
    }

    public virtual void ObserverEffect2(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        if (caster == null)
            return;
        if (caster.ngoDaoCombatsDynamic.Any(x => x.Id == this.Id))
        {
            ExecuteNgoDao2(caster, target, data);
        }
    }

    public void ExecuteNgoDao2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddDamageData(new List<int>{(target.GetDameBuffForTarget() * buff / 100)});
        caster.RemoveNgoDaoCombatDynamic(this);
    }

    public override string GetDes()
    {
        return $"Mỗi lần phát động [Hóa Cát] thì lần tiếp theo sử dụng thần thông gây tổn thương tăng lên {BaseSkill.GetColoredText($"{buff}%", "red")} điểm sát thương";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect2);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NgungSa value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}