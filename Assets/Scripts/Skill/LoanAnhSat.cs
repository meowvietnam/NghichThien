using UnityEngine;
[CreateAssetMenu(fileName = nameof(LoanAnhSat), menuName = "Skill/Thần Thông/" + nameof(LoanAnhSat))]

public class LoanAnhSat : ThanThong
{
    
    [SerializeField]
    private int[] tieuHaos = new int[]
   {
        5,10,15,20,25
   };
    [SerializeField]
    private int[] tieuHaos2 = new int[]
   {
        8,16,24,36,40
   };
    [SerializeField]
    private int[] dames1 = new int[]
    {
        7,10,15,20,28
    };
    [SerializeField]
    private int countDame1 = 2;
    [SerializeField]
    private int[] countDame2 = new int[]
    {
        1,2,3,4,5
    };
    [SerializeField]
    private int dame2 = 6;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddDamageData(new System.Collections.Generic.List<int> { tieuHaos[GetIndexLevelThanThong(caster)] });
        caster.TakeDame(caster);
        for (int i = 0; i < countDame1; i++)
        {

            int dame = dames1[GetIndexLevelThanThong(caster)];
            target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame });
        }
        int tieuHao2 = tieuHaos2[GetIndexLevelThanThong(caster)];
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Anh, out SecureInt  anh);
        if(anh >= tieuHao2)
        {
            caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Anh, tieuHao2);
            for (int i = 0; i < countDame2[GetIndexLevelThanThong(caster)]; i++)
            {

                target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame2 });
            }
        }    


    }
   



    public override string GetDesc(int level)
    {
        return $"Tiêu hao {GetColoredText($"{tieuHaos[level - 1]}", "red")} điểm HP thì tạo thành {GetColoredText($"{countDame1} lần", "red")} {GetColoredText($"+{dames1[level - 1]}", "red")} điểm sát thương. Nếu đủ thì tiêu hao {GetColoredText($"{tieuHaos2[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Anh)} thì tạo thành {GetColoredText($"{countDame2[level-1]} lần", "red")} {GetColoredText($"+{dame2}", "red")} điểm sát thương";

    }
}
