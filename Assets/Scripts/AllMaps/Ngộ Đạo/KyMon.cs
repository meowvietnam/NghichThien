using UnityEngine;
[CreateAssetMenu(fileName = nameof(KyMon), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(KyMon))]

public class KyMon : NgoDaoCombat
{
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDes()
    {
        return $"Sau khi bày {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.TranPhap)}, có thể sử dụng lại thần thông này khu động {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.TranPhap)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        
    }
}
