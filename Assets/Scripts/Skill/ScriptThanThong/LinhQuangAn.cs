using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(LinhQuangAn), menuName = "Skill/Thần Thông/" + nameof(LinhQuangAn))]
public class LinhQuangAn : ThanThong, IMemorySerializable<LinhQuangAn>
{
    [SerializeField]
    private int[] countHapThu;
    [SerializeField]
    protected float delayTimeEffect = 0.85f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect2);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    private void ObserverEffect(params object[] data)
    {
        Debug.Log("có gọi vào LinhQuangANRemoveCheck1");
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn == null)
            return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is LinhQuangAn))
        {
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
        }
    }

    private void ObserverEffect2(params object[] data)
    {
        Debug.Log("có gọi vào LinhQuangANRemoveCheck2");
        CharacterCombat charTakeDame = (CharacterCombat)data[1];
        if (charTakeDame == null)
            return;
        if (charTakeDame.specialThanThongImpactAfterPlaySkill.Any(x => x is LinhQuangAn))
        {
            charTakeDame.HapThuLinhKhi(ELinhKhi.Kim, countHapThu[GetIndexLevelThanThong(charTakeDame)]);
            charTakeDame.RemoveAllThanThongImpactAfterPlaySkill(this);
        }
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Ở hiệp tiếp theo của đối thủ bản thân bị tổn thương thì thu hoạch được {GetColoredText($"+{countHapThu[level - 1]} điểm", "red")} {GetTextLinhKhi(ELinhKhi.Kim)} linh khí";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LinhQuangAn value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}