using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(VanTranQuyLinhQuyet))]

public class VanTranQuyLinhQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1, 2 , 3,4, 5
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThong = (ThanThong)data[2];
        if (thanThong is HoanTran)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TuKhi, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }

    }

    public override string GetDesc(int level)
    {
        return $"Khi sử dụng thần thông {GetColoredText($"[Hoàn Trần]", "red")} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TuKhi)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.PLAY_SKILL, ExecuteParams);

    }


}
