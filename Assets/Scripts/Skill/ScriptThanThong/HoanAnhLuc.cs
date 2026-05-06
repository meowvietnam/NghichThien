using UnityEngine;
[CreateAssetMenu(fileName = nameof(HoanAnhLuc), menuName = "Skill/Thần Thông/" + nameof(HoanAnhLuc))]

public class HoanAnhLuc : ThanThong
{
    [SerializeField]
    private int[] tieuHaos = new int[]
   {
        5,15,30,40,50
   };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        2,3,5,7,9
    };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int require = tieuHaos[GetIndexLevelThanThong(caster)];
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Anh, require);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.AnhDich, buffValues[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(caster, delayTimeEffect);


    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Anh, out SecureInt  anh);
        int require = tieuHaos[GetIndexLevelThanThong(caster)];
        if(anh >= require && base.IsCanUse(caster, target))
        {
            return true;
        }    
        return false;
    }



    public override string GetDesc(int level)
    {
        return $"Tiêu hao {GetColoredText($"{tieuHaos[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Anh)}. Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnhDich)}";

    }
}
