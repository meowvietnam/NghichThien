using UnityEngine;
[CreateAssetMenu(fileName = nameof(PhanMocQuyet), menuName = "Skill/Thần Thông/" + nameof(PhanMocQuyet))]

public class PhanMocQuyet : ThanThong
{

    [SerializeField]
    private int[] dameValues = new int[]
    {
        32,45,74,132,264
    };
    [SerializeField]
    private int[] dameValueBuffs = new int[]
    {
        2,4,11,22,44
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int mocLinhKhi = caster.khiMoc;

        caster.TieuTanLinhKhi(ELinhKhi.Moc, mocLinhKhi);
        int dame = dameValues[GetIndexLevelThanThong(caster)];
        int dameBuff = dameValueBuffs[GetIndexLevelThanThong(caster)] * mocLinhKhi;
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame + dameBuff }, eTypeSkill);



    }

   


    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương. Tiêu tán tất cả {GetTextLinhKhi(ELinhKhi.Moc)} hệ linh khí, mỗi một điểm tiêu tán tăng lên {GetColoredText($"+{dameValueBuffs[level - 1]}", "red")} điểm sát thương";

    }
}
