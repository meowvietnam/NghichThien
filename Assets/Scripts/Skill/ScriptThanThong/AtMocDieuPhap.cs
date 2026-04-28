using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(AtMocDieuPhap), menuName = "Skill/Thần Thông/" + nameof(AtMocDieuPhap))]

public class AtMocDieuPhap : ThanThong
{
    [SerializeField] private int[] dames;
    [SerializeField] private int hapThu = 1;
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        var dicHapThuLinhKhi = caster.DictHapThuLinhKhi(hapThu);

        bool isHaveMoc = dicHapThuLinhKhi.Any(x => x.Key == ELinhKhi.Moc);
        if(isHaveMoc)
        {
            caster.Healing(dame);
        }
        ShowSkillEffectWorld(target, delayTimeEffect);

    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. Đồng thời hấp thụ {GetColoredText($"{hapThu}", "red")} điểm linh khí. Nếu là {GetTextLinhKhi(ELinhKhi.Moc)} hệ linh khí thì hồi một lượng máu bằng sát thương gây ra";

    }
}
