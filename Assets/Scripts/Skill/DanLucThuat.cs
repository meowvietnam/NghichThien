using UnityEngine;
[CreateAssetMenu(fileName = nameof(DanLucThuat), menuName = "Skill/Thần Thông/" + nameof(DanLucThuat))]
public class DanLucThuat : ThanThong
{
    [SerializeField]
    private int[] buffValues = new int[]
   {
        1,2,4,6,13
   };
    [SerializeField]
    private int[] danLucs = new int[]
   {
        1,1,1,1,1
   };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buffValues[GetIndexLevelThanThong(caster)]);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.DanLuc, danLucs[GetIndexLevelThanThong(caster)]);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DanLuc, danLucs[GetIndexLevelThanThong(caster)]);

    }



    public override string GetDesc(int level)
    {
        return $"Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}. Song phương đều nhận phải {GetColoredText($"+{danLucs[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DanLuc)}";

    }
}
