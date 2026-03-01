using UnityEngine;
[CreateAssetMenu(fileName = nameof(KhuVatThuat), menuName = "Skill/Thần Thông/" + nameof(KhuVatThuat))]

public class KhuVatThuat : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
      {
        2,3,5,9,18
      };
    
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame });

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương";

    }
}
