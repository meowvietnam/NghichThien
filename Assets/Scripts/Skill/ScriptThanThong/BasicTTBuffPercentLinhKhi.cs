using LongDH.Script.ObjectPool;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(BasicTTBuffPercentLinhKhi),
    menuName = "Skill/Thần Thông/" + nameof(BasicTTBuffPercentLinhKhi))]
public class BasicTTBuffPercentLinhKhi : ThanThong, IMemorySerializable<BasicTTBuffPercentLinhKhi>
{
    [SerializeField] private float[] buffPercent;
    [SerializeField] private ELinhKhi elinhKhi;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        switch (elinhKhi)
        {
            case ELinhKhi.None:
                break;
            case ELinhKhi.Kim:
                caster.characterStatsTemp.percentKim += buffPercent[GetIndexLevelThanThong(caster)];
                break;
            case ELinhKhi.Moc:
                caster.characterStatsTemp.percentMoc += buffPercent[GetIndexLevelThanThong(caster)];
                break;
            case ELinhKhi.Thuy:
                caster.characterStatsTemp.percentThuy += buffPercent[GetIndexLevelThanThong(caster)];
                break;
            case ELinhKhi.Hoa:
                caster.characterStatsTemp.percentHoa += buffPercent[GetIndexLevelThanThong(caster)];
                break;
            case ELinhKhi.Tho:
                caster.characterStatsTemp.percentTho += buffPercent[GetIndexLevelThanThong(caster)];
                break;
            case ELinhKhi.Ma:
                caster.characterStatsTemp.percentMa += buffPercent[GetIndexLevelThanThong(caster)];
                break;
            default:
                break;
        }

        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return
            $"Cơ sở {GetTextLinhKhi(elinhKhi)} hệ thần thông, tăng {GetColoredText($"{buffPercent[level - 1]}", "red")}% khả năng hấp thụ {GetTextLinhKhi(elinhKhi)} hệ linh khí trong trận chiến";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref BasicTTBuffPercentLinhKhi value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}