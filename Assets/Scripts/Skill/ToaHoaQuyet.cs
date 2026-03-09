using UnityEngine;
[CreateAssetMenu(fileName = nameof(ToaHoaQuyet), menuName = "Skill/Thần Thông/" + nameof(ToaHoaQuyet))]

public class ToaHoaQuyet : ThanThong , BaseSkill.TuongDong
{
    [SerializeField]
    private int[] dameValues = new int[]
       {
        11,15,22,40,80
       };
    [SerializeField]
    private int[] dameValueBuffs = new int[]
    {
        2,3,6,10,20
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        int dame = dameValues[GetIndexLevelThanThong(caster)];
        if (target.CurrentLinhKhi <= 0)
        {
            dame *= 2;
        }

        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame  }, eTypeSkill);
       

    }

    public void ExecuteTuongDong(CharacterCombat caster, CharacterCombat target)
    {
        int dameBuff = dameValueBuffs[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> {  dameBuff }, eTypeSkill, true);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương, nếu như đối thủ không có linh khí thì tổn thương gấp đôi. {GetKeyWord(EKeyWord.TuongDong)} Tăng lên {GetColoredText($"+{dameValueBuffs[level - 1]}", "red")} điểm sát thương ";
    }
}
