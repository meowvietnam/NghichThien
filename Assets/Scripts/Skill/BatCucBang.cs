using UnityEngine;
[CreateAssetMenu(fileName = nameof(BatCucBang), menuName = "Skill/Thần Thông/" + nameof(BatCucBang))]

public class BatCucBang : ThanThong , BaseSkill.TuongKhac
{
    [SerializeField]
    private int[] dames = new int[]
   {
        6,9,14,26,52
   };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        3,4,6,11,23
    };
    [SerializeField]
    private int treKhi = 2;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ActionBCB(caster, target, dames);


    }
    private void ActionBCB(CharacterCombat caster, CharacterCombat target , int[] dames , bool isCongDon = false)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TreKhi, treKhi);
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame },eTypeSkill , isCongDon);
    }    
    public void ExecuteTuongKhac(CharacterCombat caster, CharacterCombat target)
    {
        ActionBCB(caster, target, buffValues , true);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương, thu hoạch được {GetColoredText($"+{treKhi}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TreKhi)}. {GetKeyWord(EKeyWord.TuongKhac)} cũng tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương, thu hoạch được {GetColoredText($"+{treKhi}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TreKhi)}";

    }
}
