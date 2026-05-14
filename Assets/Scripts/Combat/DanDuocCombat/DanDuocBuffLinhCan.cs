using UnityEngine;
[CreateAssetMenu(fileName = nameof(DanDuocBuffLinhCan), menuName = "Đan dược/Đan dược Combat/" + nameof(DanDuocBuffLinhCan))]

public class DanDuocBuffLinhCan : DanDuocBuffStartGame
{
    [SerializeField] private int buffLinhCan;
    [SerializeField] private ELinhKhi eLinhKhi;

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        base.InitPosCallExecute(caster);
        var danDuocCombat = GetDanDuocCombat(caster);
        switch (eLinhKhi)
        {
            case ELinhKhi.None:
                break;
            case ELinhKhi.Kim:
                caster.characterStatsTemp.percentKim += buffLinhCan * danDuocCombat.count;
                break;
            case ELinhKhi.Moc:
                caster.characterStatsTemp.percentMoc += buffLinhCan * danDuocCombat.count;

                break;
            case ELinhKhi.Thuy:
                caster.characterStatsTemp.percentThuy += buffLinhCan * danDuocCombat.count;

                break;
            case ELinhKhi.Hoa:
                caster.characterStatsTemp.percentHoa += buffLinhCan * danDuocCombat.count;

                break;
            case ELinhKhi.Tho:
                caster.characterStatsTemp.percentTho += buffLinhCan * danDuocCombat.count;

                break;
            case ELinhKhi.Ma:
                caster.characterStatsTemp.percentMa += buffLinhCan * danDuocCombat.count;

                break;
            default:
                break;
        }
    }
}
