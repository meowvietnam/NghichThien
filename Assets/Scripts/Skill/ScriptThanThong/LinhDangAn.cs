using System.Linq;
using UnityEngine;
using LongDH.LongDHModules;
using Skill;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(LinhDangAn), menuName = "Skill/Thần Thông/" + nameof(LinhDangAn))]
public class LinhDangAn : ThanThong, BaseSkill.LienKich, IMemorySerializable<LinhDangAn>
{
    [SerializeField] private int[] trienNhieu;
    [SerializeField] private float delayTimeEffect = 1f;

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
        int count = currentTurn.specialThanThongImpactAfterPlaySkill.Count(x => x is LinhDangAn);
        targetTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu,
            count * trienNhieu[GetIndexLevelThanThong(currentTurn)]);
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Trong chiến đấu, mỗi lần phát động ngũ hành {GetKeyWord(EKeyWord.LienKich)} thì đối phương nhận phải {GetColoredText($"+{trienNhieu[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)} (Thần thông này có thể cộng dồn)";
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        return;
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LinhDangAn value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}