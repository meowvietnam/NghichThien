using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DanKhi), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(DanKhi))]

public class DanKhi : NgoDaoCombat
{

    [SerializeField] private int buff = 1;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        CombatStatic.AmKinhTTNewTurn(target, caster);
        int count = target.listAmKinhTT
                          .Select(x => x.ThanThong)
                          .Distinct()
                          .Count();
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.NoiThuong, count);
    }

    public override string GetDes()
    {
        return $"Kết thúc hiệp, Tất cả {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.AmKinh)} của đối thủ số hiệp {BaseSkill.GetColoredText($"-{buff}", "red")}. Mỗi một loại {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.AmKinh)} khác biệt thì đối thủ nhận phải {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NoiThuong)}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
    }

}
