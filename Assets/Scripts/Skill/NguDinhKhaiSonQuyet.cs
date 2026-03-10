using UnityEngine;
[CreateAssetMenu(fileName = nameof(NguDinhKhaiSonQuyet), menuName = "Skill/Thần Thông/" + nameof(NguDinhKhaiSonQuyet))]
public class NguDinhKhaiSonQuyet : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
    {
        30,55,110,220,440
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        if(caster.CurrentLinhKhi>10)
        {
            dame *= 2;
        }    
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương, nếu số lượng linh khí còn thừa lớn hơn 10 thì tổn thương gấp đôi";

    }
}
