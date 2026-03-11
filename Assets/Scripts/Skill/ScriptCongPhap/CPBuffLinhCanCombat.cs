using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CPBuffLinhCanCombat))]

public class CPBuffLinhCanCombat : CongPhap
{
    [SerializeField] private int[] buffs;
    [SerializeField] private ELinhKhi eLinhKhi;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        // CH? REMOVE KHI ?Ã TH?C THI BUFF THÀNH CÔNG

        

    }
    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, {GetTextLinhKhi(eLinhKhi)} linh căn thuộc tính {GetColoredText($"+{buffs[level - 1]}", "red")}";

    }


  

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        RemoveCongPhap(caster);
        int buff = buffs[CurrentLevelCongPhapInCombat(caster) - 1];
        switch (eLinhKhi)
        {
            case ELinhKhi.None:
                break;
            case ELinhKhi.Kim:
                caster.characterStatsCurrent.percentKim += buff;
                break;
            case ELinhKhi.Moc:
                caster.characterStatsCurrent.percentMoc += buff;

                break;
            case ELinhKhi.Thuy:
                caster.characterStatsCurrent.percentThuy += buff;

                break;
            case ELinhKhi.Hoa:
                caster.characterStatsCurrent.percentHoa += buff;

                break;
            case ELinhKhi.Tho:
                caster.characterStatsCurrent.percentTho += buff;

                break;
            case ELinhKhi.Ma:
                caster.characterStatsCurrent.percentMa += buff;

                break;
            default:
                break;
        }
    }
}
