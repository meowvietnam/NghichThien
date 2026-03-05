using UnityEngine;
[CreateAssetMenu(fileName = nameof(AnhSat), menuName = "Skill/Thần Thông/" + nameof(AnhSat))]

public class AnhSat : ThanThong
{
    [SerializeField]
    private int[] tieuHaos = new int[]
   {
        9,14,18,25,29
   };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        2,3,4,5,6
    };
    [SerializeField]
    private int[] dames = new int[]
    {
        1,2,3,4,5
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int require = tieuHaos[GetIndexLevelThanThong(caster)];
        int dame = dames[GetIndexLevelThanThong(caster)];
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Anh, require);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, buffValues[GetIndexLevelThanThong(caster)]);
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame });



    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Anh, out int anh);
        int require = tieuHaos[GetIndexLevelThanThong(caster)];
        if (anh >= require)
        {
            return true;
        }
        return false;
    }



    public override string GetDesc(int level)
    {
        return $"Tiêu hao {GetColoredText($"{tieuHaos[level - 1]}", "red")} {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Anh)}. Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}";

    }
}
