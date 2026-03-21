using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NhapVi), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(NhapVi))]

public class NhapVi : NgoDaoCombat
{
    [SerializeField] private int buff = 3;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        
        if (BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire))
        {
            List<LinhKhiTieuTan> linhKhiTieuTans = (List<LinhKhiTieuTan>)data[3];

            int count = linhKhiTieuTans.Sum(x => x.count);
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKiem, buff * count);
        }
    }

    public override string GetDes()
    {
        return $"Sử dụng kiếm hệ thần thông thu hoạch được {BaseSkill.GetColoredText($"+{buff} * X", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanKiem)} (X là số linh khí thần thông này tiêu hao)";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
    }
}
