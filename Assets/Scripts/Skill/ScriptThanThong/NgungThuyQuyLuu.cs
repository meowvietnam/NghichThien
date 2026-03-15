using UnityEngine;
[CreateAssetMenu(fileName = nameof(NgungThuyQuyLuu), menuName = "Skill/Thần Thông/" + nameof(NgungThuyQuyLuu))]

public class NgungThuyQuyLuu : ThanThong
{

    [SerializeField] private int[] healValues;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int heal = healValues[GetIndexLevelThanThong(caster)];
       
        caster.Healing(heal * caster.CurrentLinhKhi);

    }
    public override string GetDesc(int level)
    {
        return $"Mỗi dư thừa một điểm linh khí thì khôi phục {GetColoredText($"+{healValues[level - 1]}", "red")} điểm Hp";
    }
}
