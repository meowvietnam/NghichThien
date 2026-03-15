using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HoaVuQuyet))]

public class HoaVuQuyet : CongPhap
{
    [SerializeField]
    private int[] healValues;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.Healing(healValues[CurrentLevelCongPhapInCombat(caster) - 1]);

    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, bản thân khôi phục {GetColoredText($"+{healValues}", "red")} điểm Hp";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ExecuteParams);
    }

}
