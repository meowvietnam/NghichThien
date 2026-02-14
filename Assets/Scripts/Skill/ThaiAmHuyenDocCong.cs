using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThaiAmHuyenDocCong))]

public class ThaiAmHuyenDocCong : CongPhap
{
    [SerializeField] private int[] dameBuffs;



    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int trungDoc = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out trungDoc);
        if (trungDoc > 0 && CheckIsEqualETypeSkill(caster))
        {
            int dame = dameBuffs[CurrentLevelCongPhapInCombat(caster) - 1];
            target.AddDamageData(new System.Collections.Generic.List<int> { dame });
        }



    }
    public override string GetDesc(int level)
    {
        return $"Sử dụng {GetTextTypeSkill()} hệ thần thông gây sát thương nếu đối phương có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)} thì tổn thương {GetColoredText($"+{dameBuffs[level - 1]}", "red")}";

    }



    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ExecuteParams);
    }
}
