using UnityEngine;
[CreateAssetMenu(fileName = nameof(AnhNac), menuName = "Skill/Thần Thông/" + nameof(AnhNac))]

public class AnhNac : ThanThong
{

    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,1,1,1,1
    };
    [SerializeField]
    private int[] buffValues2 = new int[]
   {
        0,1,8,16,27
   };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.AnhNac, buffValues[GetIndexLevelThanThong(caster)]);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Anh, buffValues2[GetIndexLevelThanThong(caster)]);


    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Anh, out int anh);
        if (anh > 0)
        {
            return true;
        }
        return false;
    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Bản thân có {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Anh)} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnhNac)} và thu hoạch được {GetColoredText($"+{buffValues2[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Anh)}";

    }
}
