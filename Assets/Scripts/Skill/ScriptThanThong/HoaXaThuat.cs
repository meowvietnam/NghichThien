using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HoaXaThuat), menuName = "Skill/Thần Thông/" + nameof(HoaXaThuat))]

public class HoaXaThuat : ThanThong , BaseSkill.TuongDong
{
    [SerializeField]
    private int[] dameValues = new int[]
        {
        9,11,15,22,46
        };
    [SerializeField]
    private int[] dameValueBuffs = new int[]
    {
        3,4,5,6,10
    };
    [SerializeField]
    private int[] debuffs = new int[]
   {
        1,2,3,5,8
   };
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dameValues[GetIndexLevelThanThong(caster)];

        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt  thieuDot);
        if(thieuDot>0)
        {
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, debuffs[GetIndexLevelThanThong(caster)]);
        }    
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

        ShowSkillEffectWorld(caster, delayTimeEffect);

    }

    public void ExecuteTuongDong(CharacterCombat caster, CharacterCombat target)
    {
        int dameBuff = dameValueBuffs[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> {  dameBuff }, eTypeSkill, true);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương. Nếu đối thủ có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} thì nhận phải {GetColoredText($"+{debuffs[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}. {GetKeyWord(EKeyWord.TuongDong)} thì tăng lên {GetColoredText($"+{dameValueBuffs[level - 1]}", "red")} điểm sát thương";

    }
}
