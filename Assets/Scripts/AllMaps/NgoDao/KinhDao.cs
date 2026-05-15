using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KinhDao), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(KinhDao))]
public class KinhDao : NgoDaoCombat, IMemorySerializable<KinhDao>
{
    [SerializeField]
    private int buff = 30;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        EBuffAndDeBuff type = (EBuffAndDeBuff)data[1];
        int newValue = (int)data[2];
        int currentValue = (int)data[3];
        //Mathf.Abs(newValue - currentValue)
        if (type == EBuffAndDeBuff.KinhDao && newValue < currentValue) // remove buff
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanThuy, buff);
        }
    }

    public override string GetDes()
    {
        return $"Mỗi lần tiêu hao {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanThuy)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.ADD_BUFF, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KinhDao value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}