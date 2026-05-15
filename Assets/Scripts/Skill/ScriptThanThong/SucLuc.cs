using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(SucLuc), menuName = "Skill/Thần Thông/" + nameof(SucLuc))]
public class SucLuc : ThanThong, IMemorySerializable<SucLuc>
{
    [SerializeField]
    private int[] buffValues = new int[]{2, 2, 2, 2, 2};
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn == null)
            return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is SucLuc))
        {
            currentTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.SucLuc, buffValues[GetIndexLevelThanThong(currentTurn)]);
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
        }
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Hiệp sau, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucLuc)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref SucLuc value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}