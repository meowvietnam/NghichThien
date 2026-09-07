using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(SongKich), menuName = "Skill/Thần Thông/" + nameof(SongKich))]
public class SongKich : ThanThong, BaseSkill.LienKich, IMemorySerializable<SongKich>
{
    [SerializeField] protected int[] damePerCountAttack;
    [SerializeField] protected int[] countAttack;
    [SerializeField] protected int[] sucLucs;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<int> dames = new List<int>();
        for (int i = 0; i < countAttack[GetIndexLevelThanThong(caster)]; i++)
        {
            dames.Add(damePerCountAttack[GetIndexLevelThanThong(caster)]);
        }

        target.CaculateDameByThanThong(caster, dames, eTypeSkill, false);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        if (currentTurn == null)
            return;
        int count = currentTurn.specialThanThongImpactAfterPlaySkill.Count(x => x is SongKich);
        currentTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.SucLuc, count * sucLucs[GetIndexLevelThanThong(currentTurn)]);
        currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
    }

    public override string GetDesc(int level)
    {
        return
            $"Tạo thành {GetColoredText($"{countAttack[level - 1]} lần {damePerCountAttack[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng Kim hệ thần thông, thì hiệp tiếp theo thu hoạch được {GetColoredText($"+{sucLucs[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucLuc)} (Có thể cộng dồn)";
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref SongKich value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}