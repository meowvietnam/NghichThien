using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NhieuHonThuat), menuName = "Skill/Thần Thông/" + nameof(NhieuHonThuat))]
public class NhieuHonThuat : ThanThong, BaseSkill.ThanThuc
{
    [SerializeField]
    private int[] dames = new int[]
   {
        3,4,5,8,16
   };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        ShowSkillEffectWorld(target, delayTimeEffect);

    }

    public void ExecuteThanThuc(CharacterCombat caster, CharacterCombat target)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.TakeDameByStatic(caster, new System.Collections.Generic.List<int> { dame },true, eTypeSkill);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.NhieuHon, 1);

    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.ThanThuc)} thì tổn thương gấp đôi. Đồng thời, đối thủ nhận phải một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NhieuHon)}";

    }
}
