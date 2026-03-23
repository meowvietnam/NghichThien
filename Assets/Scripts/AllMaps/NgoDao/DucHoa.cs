using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DucHoa), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(DucHoa))]

public class DucHoa : NgoDaoCombat
{
    [SerializeField] private int percent = 20;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
       
    }
    public bool IsCanUse(CharacterCombat charTakeDame)
    {
        if(charTakeDame.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot,out SecureInt thieuDot))
        {
            int hpPer = percent * (int)charTakeDame.characterStatsBase.hpMax / 100;
            if(thieuDot < hpPer)
            {
                return true;
            }
        }
        return false;

    }
    public override string GetDes()
    {
        return $"Nếu bản thân số tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} thấp hơn {BaseSkill.GetColoredText($"{percent}%", "red")} HP tối đa thì lúc này {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} không còn gây tổn thương mà là khôi phục HP";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
    }

}
