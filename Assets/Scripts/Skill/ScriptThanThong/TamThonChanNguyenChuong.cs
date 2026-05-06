using UnityEngine;
[CreateAssetMenu(fileName = nameof(TamThonChanNguyenChuong), menuName = "Skill/Thần Thông/" + nameof(TamThonChanNguyenChuong))]
public class TamThonChanNguyenChuong : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
    {
        17,24,38,68,135
    };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        5,7,10,25,50
    };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TreKhi, out SecureInt  treKhi);
        int dame = dames[GetIndexLevelThanThong(caster)] + treKhi * buffValues[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        ShowSkillEffectWorld(target, delayTimeEffect);

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Đối thủ mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TreKhi)} thì tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";

    }
}
