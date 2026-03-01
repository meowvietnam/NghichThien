using UnityEngine;
[CreateAssetMenu(fileName = nameof(AmDuongKinh), menuName = "Skill/Thần Thông/" + nameof(AmDuongKinh))]

public class AmDuongKinh : AmKinhTT , BaseSkill.TuongSinh
{
    [SerializeField]
    private int[] dames = new int[]
   {
        450,630,1000,1800,3600
   };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        20,30,50,100,200
    };
    public override void ExecuteAmKinh(CharacterCombat caster, CharacterCombat target)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame });

    }

    public void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        int hoTrao = buffValues[GetIndexLevelThanThong(caster)];
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTrao);
    }

    public override string GetDescAmKinhTT(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.TuongSinh)} thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    
}
