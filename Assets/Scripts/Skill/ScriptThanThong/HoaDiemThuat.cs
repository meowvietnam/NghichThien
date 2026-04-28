using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HoaDiemThuat), menuName = "Skill/Thần Thông/" + nameof(HoaDiemThuat))]

public class HoaDiemThuat : ThanThong
{
    [SerializeField] protected int[] hoaDiems = new int[]
    {
        1,2,3,4,5
    };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoaDiem, hoaDiems[GetIndexLevelThanThong(caster)]);

    }


    public override string GetDesc(int level)
    {
          return $"Thu hoạch được {GetColoredText($"+{hoaDiems[level-1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoaDiem)}";

    }
}
