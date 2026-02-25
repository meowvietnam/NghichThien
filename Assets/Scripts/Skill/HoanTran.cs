using UnityEngine;
[CreateAssetMenu(fileName = nameof(HoanTran), menuName = "Skill/Thần Thông/" + nameof(HoanTran))]

public class HoanTran : ThanThong
{
 

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Cat, out int cat);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, cat);
        target.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Cat, cat);

    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {   target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Cat, out int cat);
        if(cat>0)
        {
            return true;
        }    
        return false;
    }

    public override string GetDesc(int level)
    {
        return $"Đối thủ có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cat)} mới có thể sử dụng. {GetKeyWord(EKeyWord.HauKeVoLuc)}. Gỡ ra toàn bộ {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cat)} của đối thủ bản thân thu hoạch được {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} số tầng tương đương";

    }
}
