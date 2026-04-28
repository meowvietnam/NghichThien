using LongDH.LongDHModules;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(LinhThuyQuyet))]

public class LinhThuyQuyet : CongPhap
{
    [SerializeField]
    private int[] healValues;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(CheckIsEqualETypeSkill(caster))
        {
            caster.Healing(healValues[CurrentLevelCongPhapInCombat(caster)-1]);
        }    
    }

    public override string GetDesc(int level)
    {
        return $"Mỗi sử dụng {GetTextTypeSkill()} hệ thần thông, bản thân khôi phục {GetColoredText($"+{healValues}", "red")} điểm Hp";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);
    }


}
