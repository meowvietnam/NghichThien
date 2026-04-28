using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(QuyAnhTrongTrong), menuName = "Skill/Thần Thông/" + nameof(QuyAnhTrongTrong))]
public class QuyAnhTrongTrong : ThanThong
{
    [SerializeField]
    private int[] tieuHaos = new int[]
   {
        6,6,6,6,6
   };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        3,3,3,3,3
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int require = tieuHaos[GetIndexLevelThanThong(caster)];
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnhDich, out SecureInt  anhDich);
        int count = anhDich / require;
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.AnhDich, count * buffValues[GetIndexLevelThanThong(caster)]);


    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnhDich, out SecureInt  anhDich);
        int require = tieuHaos[GetIndexLevelThanThong(caster)];
        if (anhDich >= require && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Mỗi có {GetColoredText($"{tieuHaos[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnhDich)}. Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnhDich)}";

    }
}
