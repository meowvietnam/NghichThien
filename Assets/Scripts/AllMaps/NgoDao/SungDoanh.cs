using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(SungDoanh), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(SungDoanh))]

public class SungDoanh : NgoDaoCombat
{
    [SerializeField] private int buff = 12;
    [SerializeField] private int requireSave = 3;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.ngoDaoCombatsDynamic.Count(x => x is SungDoanh) < requireSave)
        {
            caster.AddNgoDaoCombatDynamic(this);


        }
      
    }
    public virtual void ObserverEffect2(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        if (caster == null) return;

        if (caster.ngoDaoCombatsDynamic.Any(x => x.Id == this.Id))
        {
            ExecuteNgoDao2(caster, target, data);

        }


    }
    public void ExecuteNgoDao2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.ngoDaoCombatsDynamic.Count(x => x is SungDoanh) >= requireSave)
        {
            int count = caster.thanThongCurrentUse.linhKhiTieuTan.Sum(x => x.count);
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buff * count);
            caster.RemoveNgoDaoCombatDynamic(this);


        }
    }
    public override string GetDes()
    {
        return $"Mỗi tích lũy sử dụng {BaseSkill.GetColoredText($"{requireSave }", "red")} lần thần thông, thì lần tiếp theo sử dụng thần thông tạo thành tổn thương sẽ thu hoạch được {BaseSkill.GetColoredText($"+{buff} * X", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} (X là số linh khí thần thông này tiêu hao)";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect2);

    }
}
