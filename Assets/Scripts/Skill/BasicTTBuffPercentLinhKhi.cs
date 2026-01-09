using LongDH.Script.ObjectPool;
using UnityEngine;
[CreateAssetMenu]
public class BasicTTBuffPercentLinhKhi : ThanThong
{
    [SerializeField] private float buffPercent = 2.5f;
    [SerializeField] private ELinhKhi elinhKhi;
    [SerializeField]
    private float delayTimeEffect = 1f;
    protected override ETypeItem tierItem => throw new System.NotImplementedException();

    public override void Execute(CharacterCombat caster, CharacterCombat target)
    {
        switch (elinhKhi)
        {
            case ELinhKhi.None:
                break;
            case ELinhKhi.Kim:
                caster.characterStats.percentKim += buffPercent;
                caster.characterStatsCurrent.percentKim += buffPercent;

                break;
            case ELinhKhi.Moc:
                caster.characterStats.percentMoc += buffPercent;
                caster.characterStatsCurrent.percentMoc += buffPercent;

                break;
            case ELinhKhi.Thuy:
                caster.characterStats.percentThuy += buffPercent;
                caster.characterStatsCurrent.percentThuy += buffPercent;

                break;
            case ELinhKhi.Hoa:
                caster.characterStats.percentHoa += buffPercent;
                caster.characterStatsCurrent.percentHoa += buffPercent;

                break;
            case ELinhKhi.Tho:
                caster.characterStats.percentTho += buffPercent;
                caster.characterStatsCurrent.percentTho += buffPercent;

                break;
            case ELinhKhi.Ma:
                caster.characterStats.percentMa += buffPercent;
                caster.characterStatsCurrent.percentMa += buffPercent;

                break;
            default:
                break;
        }
        ShowSkillEffect(caster, delayTimeEffect);
        
    }

    public override string GetDesc()
    {
        return $"Cơ sở {GetTextLinhKhi(elinhKhi)} hệ thần thông, tăng {GetColoredText($"{buffPercent}", "red")} điểm {GetTextLinhKhi(elinhKhi)} hệ linh khí";
    }
}
