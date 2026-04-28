using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TuNguyenChu), menuName = "Skill/Thần Thông/" + nameof(TuNguyenChu))]

public class TuNguyenChu : ThanThong
{
    [SerializeField]
    private int[] buffValues = new int[]
  {
        3,5,6,8,16
  };
    [SerializeField]
    private int require = 3;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int count = (int)caster.thanThongUseThisTurn.Count / require;
        int hoTrao = count * buffValues[GetIndexLevelThanThong(caster)];
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao,hoTrao);

    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Trong hiệp này, mỗi sử dụng {GetColoredText($"{require} lần", "red")} thần thông thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";

    }
}
