using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KimNguyenKiemMang), menuName = "Skill/Thần Thông/" + nameof(KimNguyenKiemMang))]
public class KimNguyenKiemMang : ThienNhanThuat , BaseSkill.TuongDong
{
    [SerializeField] private int[] dameBuff;

    public void ExecuteTuongDong(CharacterCombat caster, CharacterCombat target)
    {
        target.TakeDameByThanThong(caster, new List<int> { dameBuff[GetIndexLevelThanThong(caster)] }, eTypeSkill, true);
    }

    public override string GetDesc(int level)
    {
        return $"Tiêu hao {requireKiemKhi} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} tạo thành {GetColoredText($"{damePerCountAttack[level - 1]}", "red")} điểm sát thương. Nếu {GetKeyWord(EKeyWord.TuongDong)} đối phương nhận thêm {GetColoredText($"+{dameBuff[level - 1]} điểm", "red")} sát thương";

    }
}
