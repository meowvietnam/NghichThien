using LongDH.LongDHModules;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CuongLanQuyet))]

public class CuongLanQuyet : CongPhap
{
    [SerializeField]
    private int[] percentActives = new int[]
    {
        8 , 11 , 14 ,17 , 20
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // khi target nhận sát thương thì caster là ng gây dame
        if (CheckIsEqualETypeSkill(caster)&&CombatStatic.IsActiveWithPercent(percentActives[CurrentLevelCongPhapInCombat(caster) - 1]))
        {
            caster.HapThuLinhKhi(ELinhKhi.Thuy, 1);
        }
    }

    public override string GetDesc(int level)
    {
        return $"Mỗi lần sử dụng {GetTextTypeSkill()} hệ thần thông gây tổn thương, có {GetColoredText($"{percentActives[level - 1]}%", "red")} tỷ lệ thu hoạch được 1 điểm {GetTextTypeSkill()} hệ linh khí";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
    }

}
