using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThienTinhNhuanThuyQuyet))]

public class ThienTinhNhuanThuyQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        2 , 3 , 5 ,9 , 16
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.Healing(buffValues[CurrentLevelCongPhapInCombat(caster) - 1] *  caster.CurrentLinhKhi);
    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, khôi phục {GetColoredText($"{buffValues[level - 1]}*X", "red")} điểm Hp (X là tổng số linh khí)";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);
    }
}
