using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(fileName = nameof(LeHoa), menuName = "Skill/Thần Thông/" + nameof(LeHoa))]

public class LeHoa : ThanThong
{

    [SerializeField] protected int[] dames = new int[]
    {
        6,10,15,22,44
    };
    [SerializeField] protected int[] dameBuffs = new int[]
    {
        2,3,4,6,10
    };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        int count = caster.thanThongUseThisTurn.Count(x => x.thanThong is LeHoa) - 1;
        int dameBuff = dameBuffs[GetIndexLevelThanThong(caster)] * count;
        target.TakeDameByThanThong(caster, new List<int> { dame + dameBuff }, false);


    }


    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. Trong hiệp này, mỗi sử dụng một lần thần thông này thì tăng lên {GetColoredText($"+{dameBuffs[level - 1]}", "red")} điểm sát thương";

    }
}
