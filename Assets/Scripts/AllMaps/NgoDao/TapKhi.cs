using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TapKhi), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TapKhi))]
public class TapKhi : NgoDaoCombat, IMemorySerializable<TapKhi>
{
    [SerializeField] private int buff = 2;
    [SerializeField] private int requireActive = 3;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.thanThongUseThisTurn.Count < requireActive)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TuKhi, buff);
        }
    }

    public override string GetDes()
    {
        return
            $"Kết thúc hiệp, nếu như trong hiệp sử dụng thần thông nhỏ hơn {BaseSkill.GetColoredText($"{requireActive}", "red")} lần thì thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TuKhi)}";
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

    public void Deserialize(IReader reader, ref TapKhi value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}