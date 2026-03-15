using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KimHongQuanNhat), menuName = "Skill/Thần Thông/" + nameof(KimHongQuanNhat))]
public class KimHongQuanNhat : ThanThong
{
    [SerializeField] private int[] dame;
    [SerializeField] private int[] perDameKiemKhi;
    [SerializeField] private int[] perDameSucThe;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KietSuc, 1);


        SecureInt sucThe = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out sucThe);
        SecureInt kiemKhi = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out kiemKhi);

        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.SucThe,sucThe);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi,kiemKhi);

        int dame = this.dame[GetIndexLevelThanThong(caster)] + perDameKiemKhi[GetIndexLevelThanThong(caster)] * kiemKhi + perDameSucThe[GetIndexLevelThanThong(caster)] * sucThe;
        target.CaculateDameByThanThong(caster, new List<int> { dame }, eTypeSkill);

        ShowSkillEffectParticle(target, 1f);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tạo thành {GetColoredText($"{dame[level-1]}", "red")} điểm sát thương, cũng tiêu hao toàn bộ {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} mỗi tầng tăng thêm {GetColoredText($"{perDameKiemKhi[level - 1]}", "red")} điểm thương tổn và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)} mỗi tầng tăng thêm {GetColoredText($"{perDameSucThe[level-1]}", "red")} điểm thương tổn. Bạn bị {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KietSuc)}";
    }

}
