using System.Linq;
using UnityEngine;
using LongDH.LongDHModules;
[CreateAssetMenu(fileName = nameof(CapHuyetDang), menuName = "Skill/Thần Thông/" + nameof(CapHuyetDang))]

public class CapHuyetDang : ThanThong , BaseSkill.TuongDong
{
    [SerializeField] private int[] dames;
    [SerializeField] private int[] dameBuffs;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        caster.Healing(dame);


    }
    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. Đồng thời, hồi lại lượng Hp bằng sát thương gây ra. Nếu là {GetKeyWord(EKeyWord.TuongDong)} tăng lên {GetColoredText($"{dameBuffs[level - 1]}", "red")} điểm sát thương";

    }

    public void ExecuteTuongDong(CharacterCombat caster, CharacterCombat target)
    {
        int dame = dameBuffs[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill, true);
    }
}
