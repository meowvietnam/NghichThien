using UnityEngine;
[CreateAssetMenu(fileName = nameof(LietKhiTram), menuName = "Skill/Thần Thông/" + nameof(LietKhiTram))]

public class LietKhiTram : ThanThong
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out SecureInt  hoTrao);

        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { hoTrao }, eTypeSkill);

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành lượng sát thương tương đương với số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} bạn đang có";

    }
}
