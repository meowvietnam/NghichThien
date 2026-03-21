using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TauHuyet), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TauHuyet))]
public class TauHuyet : NgoDaoCombat
{
    [SerializeField] private int buff = 1;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
       
    }

    public override string GetDes()
    {
        return $"Linh khí tối đa {BaseSkill.GetColoredText($"+{buff}", "red")}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.totalKhi += buff;
    }
}
