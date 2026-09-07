using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ChiThuy), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ChiThuy))]
public class ChiThuy : NgoDaoCombat, IMemorySerializable<ChiThuy>
{
    [SerializeField] private int buff = 1;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        EBuffAndDeBuff type = (EBuffAndDeBuff)data[1];
        int newValue = (int)data[2];
        int currentValue = (int)data[3];
        //Mathf.Abs(newValue - currentValue)
        if (type == EBuffAndDeBuff.ChiThuy && newValue < currentValue) // remove buff
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, buff);
        }
    }

    public override string GetDes()
    {
        return
            $"Mỗi lần tiêu hao {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)} thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)}";
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

    public void Deserialize(IReader reader, ref ChiThuy value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}