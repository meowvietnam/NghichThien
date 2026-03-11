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
        for (int i = 0; i < target.allDamages.Count; i++)
        {
            int dame = target.allDamages[i];
            int count = caster.listAmKinhTT
                .Where(x => x.ThanThong is AmKinhTT)
                .Select(x => x.ThanThong.name)
                .Distinct()
                .Count();
            float multiplier = count * buffValues[CurrentLevelCongPhapInCombat(caster) - 1] / 100f;
            target.allDamages[i] += (int)(dame * multiplier);
        }
    }

    public override string GetDesc(int level)
    {
        return $"Bản thân mỗi có một loại {GetKeyWord(EKeyWord.AmKinh)} khác biệt thì đối phương nhận tổn thương tăng lên {GetColoredText($"+{buffValues[level - 1]}%", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_BEFORE_TAKE_DAME, ExecuteParams);

    }
}
