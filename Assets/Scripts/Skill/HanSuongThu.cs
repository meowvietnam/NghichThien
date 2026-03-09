using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = nameof(HanSuongThu), menuName = "Skill/Thần Thông/" + nameof(HanSuongThu))]

public class HanSuongThu : ThanThong
{
    [SerializeField] private int[] dames;
    [SerializeField] private int suongGia = 1;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster, new List<int> { dame }, eTypeSkill);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, suongGia);


    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. Đồng thời, đối thủ nhận phải {GetColoredText($"+{suongGia}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)}";
    }

}
