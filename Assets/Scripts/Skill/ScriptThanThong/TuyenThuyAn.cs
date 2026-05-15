using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TuyenThuyAn), menuName = "Skill/Thần Thông/" + nameof(TuyenThuyAn))]
public class TuyenThuyAn : ThanThong, IMemorySerializable<TuyenThuyAn>
{
    [SerializeField]
    private int[] healValues;
    [SerializeField]
    private float delayTimeEffect = 0.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect2);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat targetTurn = (CharacterCombat)data[1]; // target là ng nhận dame
        if (targetTurn == null)
            return;
        if (targetTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is TuyenThuyAn))
        {
            targetTurn.Healing(healValues[GetIndexLevelThanThong(targetTurn)]);
            targetTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
        }
    }

    private void ObserverEffect2(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0]; // 
        if (currentTurn == null)
            return;
        currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Ở hiệp tiếp theo, nhận sát thương thì bản thân được khôi phục {GetColoredText($"+{healValues[level - 1]}", "red")} Hp";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TuyenThuyAn value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}