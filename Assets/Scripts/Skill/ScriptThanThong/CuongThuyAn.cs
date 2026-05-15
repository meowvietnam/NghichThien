using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(CuongThuyAn), menuName = "Skill/Thần Thông/" + nameof(CuongThuyAn))]
public class CuongThuyAn : ThanThong, IMemorySerializable<CuongThuyAn>
{
    [SerializeField]
    private int[] dames;
    [SerializeField]
    private float delayTimeEffect = 0.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect2);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        if (currentTurn == null)
            return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is CuongThuyAn) && CheckIsEqualETypeSkill(currentTurn))
        {
            int dame = dames[GetIndexLevelThanThong(currentTurn)];
            targetTurn.AddDamageData(new List<int>{dame});
        }
    }

    private void ObserverEffect2(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        if (currentTurn == null)
            return;
        if (currentTurn.thanThongUseLastTurn == null)
            return;
        if (currentTurn.thanThongUseLastTurn.Any(x => x.thanThong is CuongThuyAn))
        {
            if (!currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is CuongThuyAn))
            {
                currentTurn.AddThanThongImpactAfterPlaySkill(this);
            }
        }
        else
        {
            if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is CuongThuyAn))
            {
                currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
            }
        }
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Sử dụng {GetTextTypeSkill()} hệ thần thông thì tổn thương {GetColoredText($"+{dames[level - 1]}", "red")} (tồn tại ở hiệp sau)";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref CuongThuyAn value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}