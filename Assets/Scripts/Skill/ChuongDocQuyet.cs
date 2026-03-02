using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ChuongDocQuyet), menuName = "Skill/Thần Thông/" + nameof(ChuongDocQuyet))]

public class ChuongDocQuyet : ThanThong , BaseSkill.TuongDong
{
    [SerializeField] private int[] dames;
    [SerializeField] private int[] dameBuffs;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        int trungDoc = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out trungDoc);
        if(trungDoc > 0)
        {
            dame *=2;
        }    
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame });
        

    }

    public void ExecuteTuongDong(CharacterCombat caster, CharacterCombat target)
    {
        int dame = dameBuffs[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame });

    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. Nếu đối phương có {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)} thì nhận tổn thương gấp đôi. Nếu là {GetKeyWord(EKeyWord.TuongDong)} tăng lên {GetColoredText($"{dameBuffs[level - 1]}", "red")} điểm sát thương";

    }
}
