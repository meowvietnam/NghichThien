using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HauThoAn), menuName = "Skill/Thần Thông/" + nameof(HauThoAn))]
public class HauThoAn : ThanThong, BaseSkill.LienKich, IMemorySerializable<HauThoAn>
{
    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        return;
    }

    [SerializeField] private int[] buffValues = new int[] { 4, 6, 8, 12, 22 };
    [SerializeField] private float delayTimeEffect = 0.65f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.LIEN_KICH, ObserverEffect);
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
        if (currentTurn == null)
            return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is HauThoAn))
        {
            int count = currentTurn.specialThanThongImpactAfterPlaySkill.Count(x => x is HauThoAn);
            currentTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao,
                count * buffValues[GetIndexLevelThanThong(currentTurn)]);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Trong cuộc chiến, Mỗi lần phát động ngũ hành {GetKeyWord(EKeyWord.LienKich)} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} (Có thể cộng dồn)";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HauThoAn value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}