using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CuuHuyenNoiKinh))]
public class CuuHuyenNoiKinh : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
            {
                4, 5 , 6,7, 8
            };
    
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = caster.listAmKinhTT
                .Where(x => x.ThanThong is AmKinhTT)
                .Select(x => x.ThanThong.name)
                .Distinct()
                .Count();
        float multiplier = count * buffValues[CurrentLevelCongPhapInCombat(caster) - 1] / 100f;
        target.AddDamageData(new List<int> { (int)(target.allDamages[0] * multiplier) });
        
    }

    public override string GetDesc(int level)
    {
        return $"Bản thân mỗi có một loại {GetKeyWord(EKeyWord.AmKinh)} khác biệt thì đối phương nhận tổn thương tăng lên {GetColoredText($"+{buffValues[level - 1]}%", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ExecuteParams);

    }
}
