using UnityEngine;
[CreateAssetMenu(fileName = nameof(HoaCat), menuName = "Skill/Thần Thông/" + nameof(HoaCat))]

public class HoaCat : ThanThong
{

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out int tran);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.Cat, tran);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Tran, tran);

    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out int tran);
        if (tran > 0)
        {
            return true;
        }
        return false;
    }

    public override string GetDesc(int level)
    {
        return $"Bản thân có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} mới có thể sử dụng. {GetKeyWord(EKeyWord.HauKeVoLuc)}. Gỡ ra toàn bộ {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} của bản thân đối thủ nhận phải {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cat)} số tầng tương đương";

    }
}
