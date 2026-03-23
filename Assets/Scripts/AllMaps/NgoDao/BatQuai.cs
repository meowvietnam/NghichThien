using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(BatQuai), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(BatQuai))]

public class BatQuai : NgoDaoCombat
{
    [SerializeField] private int buff = 10;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<LinhKhiTieuTan> linhKhiTieuTans = (List<LinhKhiTieuTan>)data[3];
        ThanThong thanThong = (ThanThong)data[2];
        if ((thanThong is TranPhapTT || thanThong.GetEKeywordSkillHave().Contains(BaseSkill.EKeyWord.TranPhap))&& CombatManager.GetETuongQuan(linhKhiTieuTans) == ETuongQuan.TuongSinh)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buff);
        }
       
    }

    public override string GetDes()
    {
        return $"Sử dụng {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.TuongSinh)} linh khí phóng thích {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.TranPhap)} thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
    }

}
