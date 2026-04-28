using LongDH.LongDHModules;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(BangSuongQuyet))]

public class BangSuongQuyet : CongPhap
{
    [SerializeField]
    private int[] percentActives = new int[]
    {
        40 , 55 , 70 ,85 , 100
    };
    public override void ExecuteParams(params object[] data)
    {
        // target là ng nhận dame 
        // ktra xem ng nhận dame có công pháp này k

        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Tìm trong danh sách trang bị của caster
        if (target!=null&&IsCanUse(target))
        {
            Execute(caster, target, data);

        }
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // khi target nhận sát thương thì caster là ng gây dame
        if (CombatStatic.IsActiveWithPercent(percentActives[CurrentLevelCongPhapInCombat(target)-1]))
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, 1);
        }    
    }

    public override string GetDesc(int level)
    {
        return $"Mỗi lần nhận sát thương từ thần thông, có {GetColoredText($"{percentActives[level - 1]}%", "red")} tỷ lệ làm cho đối phương nhận phải 1 tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
    }

}
