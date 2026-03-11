using UnityEngine;
using static BaseSkill;
[CreateAssetMenu(fileName = nameof(ThienKiemQuyet), menuName = "Skill/Thần Thông/" + nameof(ThienKiemQuyet))]

public class ThienKiemQuyet : ThanThong
{
    [SerializeField]
    protected int[] dames = new int[]
      {
        13,20,30,54,108
      };

    [SerializeField] private int require = 10;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi);

        int dame = dames[GetIndexLevelThanThong(caster)];
        if (kiemKhi > require)
        {
            dame *= 2;
        }
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Nếu số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} lớn hơn {GetColoredText($"{require}", "red")} thì tổn thương gấp đôi";

    }
}
