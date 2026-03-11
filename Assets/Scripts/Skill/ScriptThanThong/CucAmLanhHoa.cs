using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(CucAmLanhHoa), menuName = "Skill/Thần Thông/" + nameof(CucAmLanhHoa))]

public class CucAmLanhHoa : ThanThong
{

    [SerializeField]
    private int[] debuffs = new int[]
    {
        1,2,3,4,5
    };
    [SerializeField]
    private int[] dameBuffs = new int[]
    {
        30,40,56,80,117
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dameBuffs[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        int debuff = debuffs[GetIndexLevelThanThong(caster)];
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, debuff);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, debuff);

    }

   



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dameBuffs[level - 1]}", "red")} điểm sát thương. Đối thủ nhận phải {GetColoredText($"+{debuffs[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)}";

    }
}
