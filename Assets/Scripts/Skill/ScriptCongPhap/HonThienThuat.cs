using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HonThienThuat))]
public class HonThienThuat : ThoNapCongPhap
{
    
    [SerializeField]
    private int[] dames = new int[]
    {
       1, 2 , 3,4, 5
    };
  
    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (target != null && IsCanUse(target))
        {
            Execute(caster, target, data); // target là ng nhận dame nên check xem target có skill k để active
        }
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddDamageData(new List<int> { (int)(dames[CurrentLevelCongPhapInCombat(target) - 1]) });

       
    }

    public override string GetDesc(int level)
    {
        return $"{base.GetDesc(level)} nhưng nhận tất cả tổn thương tăng {GetColoredText($"+{dames[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_BEFORE_TAKE_DAME, ExecuteParams);

    }
}
