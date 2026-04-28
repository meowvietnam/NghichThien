using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using static CharacterCombat;
[CreateAssetMenu(fileName = nameof(LanHoa), menuName = "Skill/Thần Thông/" + nameof(LanHoa))]

public class LanHoa : ThanThong
{
    [SerializeField]
    private int[] targetThieuDot;
    
    [SerializeField]
    private int[] myThieuDot;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, targetThieuDot[GetIndexLevelThanThong(caster)]);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, myThieuDot[GetIndexLevelThanThong(caster)]);




    }



    public override string GetDesc(int level)
    {
        return $"Đối thủ nhận phải {GetColoredText($"+{targetThieuDot[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}. Bản thân nhận phải {GetColoredText($"+{myThieuDot[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";

    }


}
