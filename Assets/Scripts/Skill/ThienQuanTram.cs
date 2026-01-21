using System.Collections.Generic;
using UnityEngine;
using static CharacterCombat;
[CreateAssetMenu]
public class ThienQuanTram : ThanThong
{
    [SerializeField] private int[] dames;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int hoTrao = 0;
        int dame = dames[GetIndexLevelThanThong(caster)-1];
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out hoTrao);

        if (hoTrao > 0)
        {
            target.TakeDameByThanThong(caster, new List<int> { dame *2 });
        }
        else
        {
            target.TakeDameByThanThong(caster, new List<int> { dame });
        }
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dames[level-1]}", "red")} điểm sát thương, nếu đối phương có {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} thì nhận gấp đôi sát thương";

    }
}
