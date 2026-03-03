using UnityEngine;
[CreateAssetMenu(fileName = nameof(LaYenBo), menuName = "Skill/Thần Thông/" + nameof(LaYenBo))]

public class LaYenBo : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
    {
        3,5,8,15,30
    };
    [SerializeField]
    private int require = 5;
    [SerializeField]
    private int dameBuffs = 1;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int X = (int)caster.characterStatsCurrent.tocDoBay / require;
        int dame = dames[GetIndexLevelThanThong(caster)] + X * dameBuffs;

        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame });

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Mỗi có {GetColoredText($"{require}", "red")} điểm độn tốc thì tăng lên {GetColoredText($"+{dameBuffs}", "red")} điểm sát thương";

    }
}
