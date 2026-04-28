using UnityEngine;
[CreateAssetMenu(fileName = nameof(TuTranThuat), menuName = "Skill/Thần Thông/" + nameof(TuTranThuat))]

public class TuTranThuat : ThanThong, BaseSkill.LienKich
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        4,6,9,16,32
    };
    [SerializeField]
    private int[] buff2Values = new int[]
   {
        2,3,4,6,9
   };
    [SerializeField] protected float delayTimeEffect = 0.5f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int tran = buffValues[GetIndexLevelThanThong(caster)];
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, tran);
        ShowSkillEffectWorld(caster, delayTimeEffect);


    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        int tran = buff2Values[GetIndexLevelThanThong(caster)];
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, tran);
    }

    public override string GetDesc(int level)
    {
        return $"Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}, {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Hoa)} hệ thần thông thì thu hoạch được {GetColoredText($"+{buff2Values[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}";

    }
}
