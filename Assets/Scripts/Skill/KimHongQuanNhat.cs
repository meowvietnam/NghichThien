using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class KimHongQuanNhat : ThanThong
{
    [SerializeField] private int dame = 190;
    [SerializeField] private int perDameKiemKhi = 6;
    [SerializeField] private int perDameSucThe = 1;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongCoolDown(this, 1);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KietSuc, 1);


        int sucThe = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out sucThe);
        int kiemKhi = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out kiemKhi);

        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.SucThe,sucThe);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi,kiemKhi);

        int dame =  this.dame+ perDameKiemKhi * kiemKhi + perDameSucThe * sucThe;
        target.TakeDameCaculate(caster, new List<int> { dame });

        ShowSkillEffectParticle(target, 1f);
    }

    public override string GetDesc()
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tạo thành {GetColoredText($"{dame}", "red")} điểm sát thương, cũng tiêu hao toàn bộ {GetDescBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} mỗi tầng tăng thêm {GetColoredText($"{perDameKiemKhi}", "red")} điểm thương tổn và {GetDescBuffOrDeBuff(EBuffAndDeBuff.SucThe)} mỗi tầng tăng thêm {GetColoredText($"{perDameSucThe}", "red")} điểm thương tổn. Bạn bị {GetDescBuffOrDeBuff(EBuffAndDeBuff.KietSuc)}";
    }

}
