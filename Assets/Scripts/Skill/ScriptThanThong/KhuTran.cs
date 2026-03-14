using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KhuTran), menuName = "Skill/Thần Thông/" + nameof(KhuTran))]

public class KhuTran : ThanThong
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        var tranPhapSkills = caster.specialThanThongImpactAfterPlaySkill.OfType<TranPhapTT>().ToList();

        // Bây giờ bạn có thể thao tác với tranPhapSkills
        tranPhapSkills.ForEach(tp =>
        {
            tp.ExecuteTranPhap(caster,target);
        });
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if(caster.specialThanThongImpactAfterPlaySkill.Any(x => x is TranPhapTT) && base.IsCanUse(caster, target))
        {
            return true;
        }    
        return false;
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Phát động 1 lần hiệu quả toàn bộ trận pháp của bản thân";

    }
}
