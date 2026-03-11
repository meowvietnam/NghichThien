using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(PhanDuongQuyet))]
public class PhanDuongQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
     {
        2, 3 , 4,6, 9
     };
    public override void ExecuteParams(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1]; // target trong observer lần này là ng nhận dame và check hắn ta
        // Tìm trong danh sách trang bị của caster
        if (IsCanUse(target))
        {
            Execute(caster, target, data);

        }
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);


    }

    public override string GetDesc(int level)
    {
        return $"Mỗi lần nhận tổn thương bởi thần thông, đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ExecuteParams);

    }
}
