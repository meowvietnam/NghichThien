using LongDH.Script.ObjectPool;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(BasicTTBuffPercentLinhKhi), menuName = "Skill/Thần Thông/" + nameof(BasicTTBuffPercentLinhKhi))]
public class BasicTTBuffPercentLinhKhi : ThanThong
{
    [SerializeField] private float[] buffPercent;
    [SerializeField] private ELinhKhi elinhKhi;
    [SerializeField]
    private float delayTimeEffect = 1f;
    protected override ETypeItem typeItem => throw new System.NotImplementedException();

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        switch (elinhKhi)
        {
            case ELinhKhi.None:
                break;
            case ELinhKhi.Kim:
                caster.characterStats.percentKim += buffPercent[GetIndexLevelThanThong(caster)];
                caster.characterStatsCurrent.percentKim += buffPercent[GetIndexLevelThanThong(caster)];

                break;
            case ELinhKhi.Moc:
                caster.characterStats.percentMoc += buffPercent[GetIndexLevelThanThong(caster)];
                caster.characterStatsCurrent.percentMoc += buffPercent[GetIndexLevelThanThong(caster)];
                    
                break;
            case ELinhKhi.Thuy:
                caster.characterStats.percentThuy += buffPercent[GetIndexLevelThanThong(caster)];
                caster.characterStatsCurrent.percentThuy += buffPercent[GetIndexLevelThanThong(caster)];

                break;
            case ELinhKhi.Hoa:
                caster.characterStats.percentHoa += buffPercent[GetIndexLevelThanThong(caster)];
                caster.characterStatsCurrent.percentHoa += buffPercent[GetIndexLevelThanThong(caster)];

                break;
            case ELinhKhi.Tho:
                caster.characterStats.percentTho += buffPercent[GetIndexLevelThanThong(caster)];
                caster.characterStatsCurrent.percentTho += buffPercent[GetIndexLevelThanThong(caster)];

                break;
            case ELinhKhi.Ma:
                caster.characterStats.percentMa += buffPercent[GetIndexLevelThanThong(caster)];
                caster.characterStatsCurrent.percentMa += buffPercent[GetIndexLevelThanThong(caster)];

                break;
            default:
                break;
        }
        ShowSkillEffectBuff(caster, delayTimeEffect);
        
    }

    public override string GetDesc(int level)
    {
        return $"Cơ sở {GetTextLinhKhi(elinhKhi)} hệ thần thông, tăng {GetColoredText($"{buffPercent[level-1]}", "red")} điểm {GetTextLinhKhi(elinhKhi)} hệ linh khí";
    }
}
