using UnityEngine;
[CreateAssetMenu(fileName = nameof(AnHuPha), menuName = "Skill/Thần Thông/" + nameof(AnHuPha))]

public class AnHuPha : ThanThong
{

    [SerializeField]
    private int[] dames = new int[]
    {
        3,5,8,15,30
    };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,3,7,20,40
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)] + caster.CurrentLinhKhi * buffValues[GetIndexLevelThanThong(caster)];
        caster.TieuTanAllLinhKhi();

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame } , eTypeSkill);

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Tiêu tán tất cả linh khí, mỗi một điểm linh khí tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";

    }
}
