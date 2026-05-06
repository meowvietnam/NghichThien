using UnityEngine;
[CreateAssetMenu(fileName = nameof(KhuAnhThuat), menuName = "Skill/Thần Thông/" + nameof(KhuAnhThuat))]

public class KhuAnhThuat : ThanThong , BaseSkill.TuongSinh , BaseSkill.TuongKhac
{
    [SerializeField]
    private int buffSinh = 2;
    [SerializeField]
    private int buffKhac = 1;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ShowSkillEffectWorld(caster, delayTimeEffect);




    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.TuongSinh)} Mỗi tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnhDich)} thu hoạch thêm {GetColoredText($"+{buffSinh}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Anh)}. {GetKeyWord(EKeyWord.TuongKhac)} {GetColoredText($"{buffKhac} lần", "red")} phát động {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnhDich)}";

    }

    public void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnhDich, out SecureInt  anhDich);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Anh,anhDich * buffSinh);
    }

    public void ExecuteTuongKhac(CharacterCombat caster, CharacterCombat target)
    {
       for (int i = 0; i < buffKhac; i++)
        {
            CombatStatic.AnhDich(caster, target);
        }
    }
}
