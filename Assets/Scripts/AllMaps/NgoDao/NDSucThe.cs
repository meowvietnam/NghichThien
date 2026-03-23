using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NDSucThe), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(NDSucThe))]
public class NDSucThe : NgoDaoCombat
{
    [SerializeField] private int requireActiveSkil = 4;
    [SerializeField] private int buff = 12;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<LinhKhiTieuTan> linhKhiTieuTans = (List<LinhKhiTieuTan>)data[3];
        int kimLinhKhi = 0;
        foreach (var item in linhKhiTieuTans)
        {
            if (item.eLinhKhi == ELinhKhi.Kim)
            {
                kimLinhKhi += item.count;
            }
        }
       if (caster.IsSkillTichLuyCanUse(this,kimLinhKhi, requireActiveSkil))
       {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, buff);
       }
    }

    public override string GetDes()
    {
        return $"Mỗi tích lũy sử dụng {BaseSkill.GetColoredText($"{requireActiveSkil}", "red")} điểm {BaseSkill.GetTextLinhKhi(ELinhKhi.Kim)} hệ linh khí, thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
    }


}
