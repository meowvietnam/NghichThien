using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HoaKinh), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(HoaKinh))]

public class HoaKinh : NgoDaoCombat
{
    [SerializeField] private int requireActiveSkil = 50;
    [SerializeField] private int buff = 50;
    public override void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        if (target == null) return;

        if (target.ngoDaoCombats.Any(x => x.Id == this.Id))
        {
            ExecuteNgoDao(caster, target, data);

        }


    }
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        for (int i = 0; i < target.allDamages.Count; i++)
        {
            if (target.allDamages[i] >= requireActiveSkil && target.CurrentLinhKhi>0)
            {
                target.allDamages[i] -= buff; 
                target.TieuTanLinhKhi(target.GetRandomLinhKhiConLai(), 1);

            }    
        }
    }

    public override string GetDes()
    {
        return $"Mỗi lần nhận tổn thương lớn hơn {BaseSkill.GetColoredText($"{requireActiveSkil}", "red")} điểm sát thương. Nếu bản thân còn linh khí thì tiêu tán một điểm linh khí làm tổn thương này giảm xuống {BaseSkill.GetColoredText($"{buff}", "red")} điểm sát thương";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ObserverEffect);
    }

}
