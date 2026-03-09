using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NguTran), menuName = "Skill/Thần Thông/" + nameof(NguTran))]

public class NguTran : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
     {
        1,2,3,5,10
     };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        4,6,9,17,33
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = caster.specialThanThongImpactAfterPlaySkill.Count(x=>x is TranPhapTT);
        int dame = dames[GetIndexLevelThanThong(caster)] + count * buffValues[GetIndexLevelThanThong(caster)];

        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Bản thân mỗi có một trận pháp thì tổn thương {GetColoredText($"+{buffValues[level - 1]}", "red")}";

    }
}
