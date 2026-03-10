using UnityEngine;
[CreateAssetMenu(fileName = nameof(DietLinhChuong), menuName = "Skill/Thần Thông/" + nameof(DietLinhChuong))]
public class DietLinhChuong : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
    {
        32,41,56,70,100
    };
  
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.TieuTanAllLinhKhi();

        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Tiêu tán toàn bộ linh khí của đối thủ";

    }
}
