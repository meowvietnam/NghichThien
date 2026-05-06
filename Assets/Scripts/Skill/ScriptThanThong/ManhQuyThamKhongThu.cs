using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ManhQuyThamKhongThu), menuName = "Skill/Thần Thông/" + nameof(ManhQuyThamKhongThu))]

public class ManhQuyThamKhongThu : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
    {
        1,2,3,6,12
    };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,1,1,1,1
    };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Hon, out SecureInt  hon);
        int dame = dames[GetIndexLevelThanThong(caster)] + hon * buffValues[GetIndexLevelThanThong(caster)];

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)} thì tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";

    }
}
