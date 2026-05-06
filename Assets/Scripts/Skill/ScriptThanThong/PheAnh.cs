using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(PheAnh), menuName = "Skill/Thần Thông/" + nameof(PheAnh))]

public class PheAnh : ThanThong , BaseSkill.TuongKhac
{
    [SerializeField]
    private int[] tieuHaos = new int[]
    {
        3,5,6,8,9
    };
    [SerializeField]
    private int[] hapThus1 = new int[]
    {
        6,7,8,9,10
    };
    [SerializeField]
    private int buffValues2 = 2;
    [SerializeField]
    private int tieuHao2 = 15;
    [SerializeField]
    private int hapThu2 = 1;
    [SerializeField] private float delayTimeEffect = 1f;


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int require = tieuHaos[GetIndexLevelThanThong(caster)];
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.AnhDich, require);
        caster.HapThuLinhKhi(hapThus1[GetIndexLevelThanThong(caster)]);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TreKhi, buffValues2);
        ShowSkillEffectWorld(caster, delayTimeEffect);




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
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tiêu hao {GetColoredText($"{tieuHaos[level - 1]}", "red")} {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnhDich)} hấp thụ {GetColoredText($"+{hapThus1[level - 1]}", "red")} điểm linh khí và nhận phải {GetColoredText($"+{buffValues2}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TreKhi)}. {GetKeyWord(EKeyWord.TuongKhac)} Tiêu hao {GetColoredText($"{tieuHao2}", "red")} điểm HP hấp thụ {GetColoredText($"+{hapThu2}", "red")} điểm linh khí";

    }

    public void ExecuteTuongKhac(CharacterCombat caster, CharacterCombat target)
    {
        caster.TakeDameByStatic(null, new System.Collections.Generic.List<int> { tieuHao2 }, false, null, false);

        caster.HapThuLinhKhi(hapThu2);
    }
}
