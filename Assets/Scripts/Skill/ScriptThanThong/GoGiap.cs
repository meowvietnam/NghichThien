using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(GoGiap), menuName = "Skill/Thần Thông/" + nameof(GoGiap))]

public class GoGiap : ThanThong
{
    [SerializeField] protected float delayTimeEffect = 1f;

    [SerializeField]
    private int[] buffValues = new int[]
    {
        2,4,6,8,15
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, buffValues[GetIndexLevelThanThong(caster)]);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out SecureInt hoTrao);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, hoTrao);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoTrao,hoTrao);
        ShowSkillEffectWorld(target, delayTimeEffect);




    }


    public override string GetDesc(int level)
    {
        return $"Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}. Bản thân gỡ ra toàn bộ số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} chuyển sang {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} số tầng tương đương";

    }

   
}
