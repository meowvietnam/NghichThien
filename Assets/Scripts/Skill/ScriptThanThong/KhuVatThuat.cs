using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KhuVatThuat), menuName = "Skill/Thần Thông/" + nameof(KhuVatThuat))]

public class KhuVatThuat : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
      {
        2,3,5,9,18
      };
    [SerializeField]
    private int countAttack = 1;
    
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<int> dameList = new List<int>();
        for (int i = 0; i < countAttack; i++)
        {
            dameList.Add(dames[GetIndexLevelThanThong(caster)]);
        }
        target.CaculateDameByThanThong(caster, dameList, eTypeSkill);

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetDescCountAttack()} {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương";

    }
    private string GetDescCountAttack()
    {
        return countAttack == 1 ? " " : $"{GetColoredText($"{countAttack} lần", "red")}";
    }    

}
