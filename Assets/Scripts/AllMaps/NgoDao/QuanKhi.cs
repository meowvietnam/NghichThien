using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(QuanKhi), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(QuanKhi))]
public class QuanKhi : NgoDaoCombat
{
    [SerializeField] private int buff = 8;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<LinhKhiTieuTan> linhKhiTieuTans = (List<LinhKhiTieuTan>)data[3];
        int count = 0;
        foreach (var item in linhKhiTieuTans)
        {
             count += item.count;

        }
        if (BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire) && CombatManager.GetETuongQuan(linhKhiTieuTans) == ETuongQuan.TuongSinh)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buff * count);
        }
    }

    public override string GetDes()
    {
        return $"Sử dụng thần thức hệ thần thông tiêu hao {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.TuongSinh)} linh khí thì thu hoạch được {BaseSkill.GetColoredText($"+{buff} * X", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} (X là số linh khí thần thông này tiêu hao)";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
    }

}
