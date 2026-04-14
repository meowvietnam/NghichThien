using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NhienHoaThuat), menuName = "Skill/Thần Thông/" + nameof(NhienHoaThuat))]

public class NhienHoaThuat : ThanThong, BaseSkill.LienKich
{
    [SerializeField]
    private int[] valueBuffs = new int[]
   {
        1,2,3,4,5
   };
    [SerializeField]
    private int[] dameValues = new int[]
     {
        3,3,3,3,3
     };
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {


        int dame = dameValues[GetIndexLevelThanThong(caster)];

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame  }, eTypeSkill);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }
   
    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Diem, valueBuffs[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)}, nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Moc)} hệ thần thông thì thu hoạch được {GetColoredText($"+{valueBuffs[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)}";

    }
}
