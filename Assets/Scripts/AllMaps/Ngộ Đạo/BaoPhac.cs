using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(BaoPhac), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(BaoPhac))]

public class BaoPhac : NgoDaoCombat
{
    [SerializeField] private int buff = 4;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        
    }

    public override string GetDes()
    {
        return $"Mỗi hiệp, số lượng linh khí hấp thụ {BaseSkill.GetColoredText($"+{buff}", "red")}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.totalKhiRandom += buff;
    }
}
