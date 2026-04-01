using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(PhanHoaDiCanhThuat), menuName = "Skill/Thần Thông/" + nameof(PhanHoaDiCanhThuat))]

public class PhanHoaDiCanhThuat : ThanThong
{
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt  myThieuDot);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, myThieuDot);
        ShowSkillEffectWorld(caster, delayTimeEffect);


    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt  myThieuDot);
        if(myThieuDot > 0 && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }


    public override string GetDesc(int level)
    {
        return $"Đối thủ nhận phải số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} bằng với số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} của bản thân";

    }
}
