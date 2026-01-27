using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThanhXaKinh), menuName = "Skill/Thần Thông/" + nameof(ThanhXaKinh))]
public class ThanhXaKinh : ThanThong
{
    [SerializeField] private int[] trungDoc;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, trungDoc[GetIndexLevelThanThong(caster)]);

    }



    public override string GetDesc(int level)
    {
        return $"Đối phương nhận phải {GetColoredText($"+{trungDoc[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}";
    }
}
