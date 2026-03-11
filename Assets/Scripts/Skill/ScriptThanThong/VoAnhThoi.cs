using UnityEngine;
[CreateAssetMenu(fileName = nameof(VoAnhThoi), menuName = "Skill/Thần Thông/" + nameof(VoAnhThoi))]
public class VoAnhThoi : ThanThong
{

    [SerializeField]
    private int[] dames = new int[]
    {
        3,5,8,15,30
    };
   
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        
        int dame = dames[GetIndexLevelThanThong(caster)] + caster.characterStatsCurrent.tocDoBay;
        if(caster.characterStatsCurrent.tocDoBay > target.characterStatsCurrent.tocDoBay)
        {
            dame *= 2;
        }    
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành tổn thương tương đương với chỉ số độn tốc {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Nếu độn tốc cao hơn đối thủ thì tổn thương gấp đôi";

    }
}
