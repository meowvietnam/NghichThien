using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThanNiemQuyet), menuName = "Skill/Thần Thông/" + nameof(ThanNiemQuyet))]

public class ThanNiemQuyet : ThanThong, BaseSkill.ThanThuc
{
    [SerializeField]
    protected int[] dames = new int[]
     {
        11,19,22,38,53
     };
    [SerializeField]
    protected int[] buffValues = new int[]
    {
        1,1,2,2,3
    };
    [SerializeField] protected float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        Debug.Log("dame base " + dame);

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        ShowEffect(caster,target);
    }
    protected virtual void ShowEffect(CharacterCombat caster, CharacterCombat target)
    {
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }
    public void ExecuteThanThuc(CharacterCombat caster, CharacterCombat target)
    {

        int remainThanThuc = caster.GetThanThuc() - target.GetThanThuc();

        remainThanThuc = Mathf.Clamp(remainThanThuc, 0, int.MaxValue);
        int dame = remainThanThuc * buffValues[GetIndexLevelThanThong(caster)];

        Debug.Log("dame thần thức "+ dame);
        target.TakeDameByStatic(caster, new System.Collections.Generic.List<int> { dame }, true, eTypeSkill);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.ThanThuc)} Mỗi nhiều hơn đối thủ một điểm {GetKeyWord(EKeyWord.ThanThuc)} tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";

    }
}
