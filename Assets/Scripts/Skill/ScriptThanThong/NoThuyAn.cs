using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(NoThuyAn), menuName = "Skill/Thần Thông/" + nameof(NoThuyAn))]
public class NoThuyAn : ThanThong, BaseSkill.LienKich, IMemorySerializable<NoThuyAn>
{
    [SerializeField]
    private int[] triLieus;
    [SerializeField]
    private float delayTimeEffect = 0.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.LIEN_KICH, ObserverEffect);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        if (currentTurn == null)
            return;
        int count = currentTurn.specialThanThongImpactAfterPlaySkill.Count(x => x is NoThuyAn);
        currentTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, count * triLieus[GetIndexLevelThanThong(currentTurn)]);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Trong chiến đấu, mỗi lần phát động ngũ hành {GetKeyWord(EKeyWord.LienKich)} thì thu hoạch được {GetColoredText($"+{triLieus[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)} (Thần thông này có thể cộng dồn)";
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

    public void Deserialize(IReader reader, ref NoThuyAn value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}