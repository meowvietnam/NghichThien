using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(LuyenAnhLuc), menuName = "Skill/Thần Thông/" + nameof(LuyenAnhLuc))]

public class LuyenAnhLuc : ThanThong , BaseSkill.TuongSinh , BaseSkill.TuongKhac
{
    [SerializeField]
    private int[] tieuHaos = new int[]
  {
        1,2,3,4,5
  };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,2,3,4,5
    };
    [SerializeField]
    private int[] dames = new int[]
    {
        9,18,27,36,45
    };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int require = tieuHaos[GetIndexLevelThanThong(caster)];
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.AnhDich, require);
        ShowSkillEffectWorld(target, delayTimeEffect);

    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnhDich, out SecureInt  anhDich);
        int require = tieuHaos[GetIndexLevelThanThong(caster)];
        if (anhDich >= require && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }



    public override string GetDesc(int level)
    {
        return $"Tiêu hao {GetColoredText($"{tieuHaos[level - 1]}", "red")} {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnhDich)}.{GetKeyWord(EKeyWord.TuongSinh)} Đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChayMau)}. {GetKeyWord(EKeyWord.TuongKhac)} Hấp thụ {GetColoredText($"+{dames[level - 1]}", "red")} điểm HP của đối thủ";

    }

    public void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ChayMau, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public void ExecuteTuongKhac(CharacterCombat caster, CharacterCombat target)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

        caster.Healing(dame);

    }
}
