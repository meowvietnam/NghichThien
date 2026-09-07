using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(NghiepHoaAn), menuName = "Skill/Thần Thông/" + nameof(NghiepHoaAn))]
public class NghiepHoaAn : ThanThong, BaseSkill.LienKich, IMemorySerializable<NghiepHoaAn>
{
    [SerializeField]
    private int[] valueDeBuffs = new int[]{1, 2, 3, 4, 5};
    [SerializeField]
    private float delayTimeEffect = 1f;
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
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        if (currentTurn == null)
            return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is NghiepHoaAn))
        {
            int count = currentTurn.specialThanThongImpactAfterPlaySkill.Count(x => x is NghiepHoaAn);
            targetTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, valueDeBuffs[GetIndexLevelThanThong(currentTurn)] * count);
        }
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        return;
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Trong cuộc chiến, mỗi khi phát động ngũ hành {GetKeyWord(EKeyWord.LienKich)} thì đối thủ nhận phải {GetColoredText($"+{valueDeBuffs[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} (Có thể cộng dồn)";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NghiepHoaAn value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}