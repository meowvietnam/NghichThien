using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThongThuy), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ThongThuy))]

public class ThongThuy : NgoDaoCombat
{
    [SerializeField] private int buff = 30;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(target == null)
        {
            CombatManager cbManager = Pool.Get<CombatManager>();
            target = caster == cbManager.char1 ? cbManager.char2 : cbManager.char1; 
        }    
        int finalHpGain = (int)data[2];
        int currentValue = (int)data[1];
        int remain = currentValue - (int)caster.characterStatsTemp.hpMax;
        caster.AddDataHeal(finalHpGain * buff /100);
        if (remain > 0)
        {
            target.TakeDameByStatic(caster,new List<int> { remain} , false , null);
        }

    }

    public override string GetDes()
    {
        return $"Hiệu quả trị liệu tăng lên {BaseSkill.GetColoredText($"{buff}%", "red")}. Khi hồi phục nếu tràn ra sẽ chuyển hóa tạo thành tổn thương cho đối thủ";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_HEALING, ObserverEffect);
    }
}
