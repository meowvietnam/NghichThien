using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(DuThanNguThuyQuyet))]

public class DuThanNguThuyQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
     {
        2 , 3 , 4 ,5 , 8
     };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThong = (ThanThong)data[2];
        if(thanThong is NguThuyQuyet)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucLuc, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }
    }

    public override string GetDesc(int level)
    {
        return $"Sử dụng thần thông [Ngự Thủy Quyết] thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucLuc)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);
    }

}
