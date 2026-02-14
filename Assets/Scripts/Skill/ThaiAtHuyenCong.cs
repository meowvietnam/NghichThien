using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThaiAtHuyenCong))]

public class ThaiAtHuyenCong : CongPhap
{
    [SerializeField] private int[] percentDameBuffs;



    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int trienNhieu = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out trienNhieu);
        if (trienNhieu > 0 && CheckIsEqualETypeSkill(caster))
        {
            float percent = (float)percentDameBuffs[CurrentLevelCongPhapInCombat(caster) - 1]/100f;
            int dame = (int)(target.allDamages[0] * percent);
            target.AddDamageData(new System.Collections.Generic.List<int> { dame });
        }



    }
    public override string GetDesc(int level)
    {
        return $"Sử dụng {GetTextTypeSkill()} hệ thần thông gây sát thương nếu đối phương có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)} thì tổn thương {GetColoredText($"+{percentDameBuffs[level - 1]}%", "red")}";

    }



    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ExecuteParams);
    }
}
