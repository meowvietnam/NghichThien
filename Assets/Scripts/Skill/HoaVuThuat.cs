using System.Linq;
using UnityEngine;
using LongDH.LongDHModules;
[CreateAssetMenu(fileName = nameof(HoaVuThuat), menuName = "Skill/Thần Thông/" + nameof(HoaVuThuat))]
public class HoaVuThuat : ThanThong , BaseSkill.LienKich
{
    [SerializeField] private int[] dames;
    [SerializeField] private int debuffCount=2;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);


    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        for (int i = 0; i < debuffCount; i++)
        {
            target.AddBuffAndDebuffCount(BaseSkill.eDebuffs[Random.Range(0,BaseSkill.eDebuffs.Count)], 1);

        }

    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)}. Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Thuy)} hệ thần thông, đối phương nhận {GetColoredText($"{debuffCount}", "red")} trạng thái tiêu cực ngẫu nhiên";

    }
}
