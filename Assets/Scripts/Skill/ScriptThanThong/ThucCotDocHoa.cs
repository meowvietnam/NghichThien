using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThucCotDocHoa), menuName = "Skill/Thần Thông/" + nameof(ThucCotDocHoa))]

public class ThucCotDocHoa : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
      {
        1,2,4,6,12
      };
    [SerializeField]
    private int[] deBuffs = new int[]
      {
        1,1,1,1,1
      };
    [SerializeField] private float delayEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        int dame = dames[GetIndexLevelThanThong(caster)];

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);


        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, deBuffs[GetIndexLevelThanThong(caster)]);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TreKhi, deBuffs[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectParticle(caster,delayEffect,3.5f);


    }

  
    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Đối thủ nhận phải {GetColoredText($"+{deBuffs[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TreKhi)}";

    }
}
