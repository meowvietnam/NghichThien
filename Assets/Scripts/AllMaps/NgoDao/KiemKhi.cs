using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KiemKhi), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(KiemKhi))]

public class KiemKhi : NgoDaoCombat
{
    [SerializeField] private int buff = 1;
    [SerializeField] private int percent = 50;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(caster.thanThongCurrentUse.thanThong == caster.thanThongOldUse.thanThong && BaseSkill.CheckIsEqualETypeSkill(caster,eTypeRequire))
        {
            if(CombatStatic.IsActiveWithPercent(percent))
            {
                caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, buff);
            }    
        }
    }

    public override string GetDes()
    {
        return $"Liên tục dùng cùng một loại Kiếm hệ thần thông có {BaseSkill.GetColoredText($"{percent}%", "red")} thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
    }

}
