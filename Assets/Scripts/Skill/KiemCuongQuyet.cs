using UnityEngine;
[CreateAssetMenu(fileName = nameof(KiemCuongQuyet), menuName = "Skill/Thần Thông/" + nameof(KiemCuongQuyet))]

public class KiemCuongQuyet : ThanThong
{
    [SerializeField]
    protected int[] dames = new int[]
    {
        1,2,4,7,12
    };
    [SerializeField]
    protected int[] buffValues = new int[]
    {
        1,1,1,1,1
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi);
        int dame = dames[GetIndexLevelThanThong(caster)] + kiemKhi * buffValues[GetIndexLevelThanThong(caster)];

        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

    }

   

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} Tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";

    }
}
