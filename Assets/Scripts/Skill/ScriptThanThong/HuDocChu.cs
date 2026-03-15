using UnityEngine;
[CreateAssetMenu(fileName = nameof(HuDocChu), menuName = "Skill/Thần Thông/" + nameof(HuDocChu))]

public class HuDocChu : ThanThong
{
    [SerializeField] private int[] trungDoc;
    [SerializeField] private int[] dame;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame[GetIndexLevelThanThong(caster)] }, eTypeSkill);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, trungDoc[GetIndexLevelThanThong(caster)]);

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dame[level - 1]}", "red")}. Đối phương nhận phải {GetColoredText($"+{trungDoc[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}";
    }
}
