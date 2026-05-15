using AllMaps.NgoDao;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(NgoDaoBuffStats),
    menuName = "Ngộ Đạo/Ngộ Đạo Buff Stats/" + nameof(NgoDaoBuffStats))]
public class NgoDaoBuffStats : NgoDao
{
    [SerializeField] private EBuffStats currentEBuffStat;
    [SerializeField] private int buff;
    private enum EBuffStats
    {
        None,
        ThanThuc,
        HP,
        DonToc,
    }
    public override string GetDes()
    {
        return $"{GetTextBuff(currentEBuffStat)} {BaseSkill.GetColoredText($"+{buff}", "red")}";
    }

    private string GetTextBuff(EBuffStats ebuffStats)
    {
        switch (ebuffStats)
        {
            case EBuffStats.None:
                break;
            case EBuffStats.ThanThuc:
                return "Thần thức";
            case EBuffStats.HP:
                return "HP tối đa";
            case EBuffStats.DonToc:
                return "Độn tốc";
            default:
                break;
        }
        return "";
    }
}