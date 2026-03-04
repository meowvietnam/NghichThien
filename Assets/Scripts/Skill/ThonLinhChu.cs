using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThonLinhChu), menuName = "Skill/Thần Thông/" + nameof(ThonLinhChu))]

public class ThonLinhChu : ThanThong
{
   

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out int hoTrao);
        caster.Healing(hoTrao);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTrao);

    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Gỡ ra toàn bộ số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} cũng hồi một lượng HP tương đương";

    }
}
