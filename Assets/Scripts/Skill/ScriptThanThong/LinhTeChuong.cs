using UnityEngine;
[CreateAssetMenu(fileName = nameof(LinhTeChuong), menuName = "Skill/Thần Thông/" + nameof(LinhTeChuong))]

public class LinhTeChuong : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
      {
        1,2,3,5,10
      };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,1,1,1,1
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, buffValues[GetIndexLevelThanThong(caster)]);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}";

    }
}
