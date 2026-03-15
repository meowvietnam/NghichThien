using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DiTranPhaTran), menuName = "Skill/Thần Thông/" + nameof(DiTranPhaTran))]

public class DiTranPhaTran : ThanThong
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        PhaTran(caster);
        PhaTran(target);
    }

    private void PhaTran(CharacterCombat target)
    {
        var tranPhapSkills = target.specialThanThongImpactAfterPlaySkill.OfType<TranPhapTT>().ToList();
        if (tranPhapSkills.Count <= 0) return;
        var tranPhapRandom = tranPhapSkills[CombatStatic.IndexRandom(0, tranPhapSkills.Count)];

        target.RemoveAllThanThongImpactAfterPlaySkill(tranPhapRandom);
    }    


    public override string GetDesc(int level)
    {
        return $"Ngẫu nhiên phá hủy một cái trận pháp của bản thân và đối thủ";

    }
}
