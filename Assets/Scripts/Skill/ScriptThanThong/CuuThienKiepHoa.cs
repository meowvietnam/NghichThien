using UnityEngine;
[CreateAssetMenu(fileName = nameof(CuuThienKiepHoa), menuName = "Skill/Thần Thông/" + nameof(CuuThienKiepHoa))]

public class CuuThienKiepHoa : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
       {
        85,116,184,320,640
       };
    [SerializeField]
    private int[] deBuffs = new int[]
      {
        10,12,16,20,40
      };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        int dame = dames[GetIndexLevelThanThong(caster)];

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);


        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, deBuffs[GetIndexLevelThanThong(caster)]);

        ShowSkillEffectWorld(caster, delayTimeEffect);


    }


    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Bản thân nhận phải {GetColoredText($"+{deBuffs[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";

    }
}
