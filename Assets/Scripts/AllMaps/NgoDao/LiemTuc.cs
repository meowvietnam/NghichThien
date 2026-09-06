using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(LiemTuc), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(LiemTuc))]
public class LiemTuc : NgoDaoCombat, IMemorySerializable<LiemTuc>
{
    [SerializeField]
    private int buff = 50;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        EBuffAndDeBuff eBuffParam = (EBuffAndDeBuff)data[1];
        int newValue = (int)data[2];
        int oldValue = (int)data[3];
        if (eBuffParam == EBuffAndDeBuff.AnNac)
        {
            if (newValue == 0 && oldValue > 0) // thoát trạng thái
            {
                caster.AddNgoDaoCombatDynamic(this);
            }
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
        caster.RemoveNgoDaoCombatDynamic(this);
        target.AddDamageData(new List<int>{target.GetDameBuffForTarget() * buff / 100});
    }

    public override string GetDes()
    {
        return $"Thoát khỏi trạng thái {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnNac)} thì một lần sử dụng thần thông tổn thương tăng lên {BaseSkill.GetColoredText($"{buff}%", "red")} điểm sát thương";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.REMOVE_BUFF, ObserverEffect);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect2);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LiemTuc value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}