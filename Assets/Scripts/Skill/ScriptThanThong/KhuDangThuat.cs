using UnityEngine;
using LongDH.LongDHModules;
using System.Linq;
using Skill;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KhuDangThuat), menuName = "Skill/Thần Thông/" + nameof(KhuDangThuat))]
public class KhuDangThuat : ThanThong, IMemorySerializable<KhuDangThuat>
{
    [SerializeField] private int[] trienNhieu;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect2);
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
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is KhuDangThuat))
        {
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
        }
    }

    private void ObserverEffect2(params object[] data)
    {
        CharacterCombat charSendDame = (CharacterCombat)data[0];
        CharacterCombat charTakeDame = (CharacterCombat)data[1];
        if (charTakeDame.specialThanThongImpactAfterPlaySkill.Any(x => x is KhuDangThuat))
        {
            charSendDame.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu,
                trienNhieu[GetIndexLevelThanThong(charTakeDame)]);
            charTakeDame.RemoveAllThanThongImpactAfterPlaySkill(this);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Ở hiệp tiếp theo của đối thủ bản thân bị tổn thương thì đối thủ nhận phải {GetColoredText($"+{trienNhieu[level - 1]} điểm", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KhuDangThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}