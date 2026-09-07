using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(LacThachTran), menuName = "Skill/Thần Thông/" + nameof(LacThachTran))]
public class LacThachTran : TranPhapTT, IMemorySerializable<LacThachTran>
{
    [SerializeField] private int require = 5;
    [SerializeField] private ThanThong thanThongActive;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
    }

    public override void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        if (targetTurn == null)
            return;
        if (targetTurn.specialThanThongImpactAfterPlaySkill.Any(x => x.Id == this.Id))
        {
            ExecuteTranPhap(currentTurn, targetTurn);
        }
    }

    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        int count = target.specialThanThongImpactAfterPlaySkill.Count(x => x is LacThachTran);
        if (count >= require)
        {
            Observer.Notify(EventList.Combat.PLAY_SKILL, target, caster, thanThongActive, new List<LinhKhiTieuTan>());
            target.RemoveAllThanThongImpactAfterPlaySkill(this);
            target.AddThanThongImpactAfterPlaySkill(this);
        }
        else
        {
            target.AddThanThongImpactAfterPlaySkill(this);
        }
    }

    public override string GetDescTranPhapTT(int level)
    {
        return
            $"Mỗi nhận {GetColoredText($"{require}", "red")} lần công kích thì phóng thích một lần [Lạc Thạch] phản kích đối thủ";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LacThachTran value)
    {
        TranPhapTT baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}