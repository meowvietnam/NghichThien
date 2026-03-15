using UnityEngine;
[CreateAssetMenu(fileName = nameof(BangNhamThuat), menuName = "Skill/Thần Thông/" + nameof(BangNhamThuat))]

public class BangNhamThuat : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
    {
        6,10,18,35,70
    };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        6,7,9,20,40
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        var dic = caster.DictHapThuLinhKhi(1);
        foreach ( var kvp in dic)
        {
            caster.HapThuLinhKhi(kvp.Key, kvp.Value);
            if(kvp.Key == ELinhKhi.Tho)
            {
                int hoTrao = buffValues[GetIndexLevelThanThong(caster)];

                caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTrao);
            }    
        } 
            
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Hấp thụ một điểm linh khí, nếu là {GetTextLinhKhi(ELinhKhi.Tho)} hệ linh khí thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";

    }
}
