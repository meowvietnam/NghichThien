using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KienNghi), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(KienNghi))]
public class KienNghi : NgoDaoCombat
{
    [SerializeField] private int buff = 17;

    public override void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        if (target == null) return;

        if (target.ngoDaoCombats.Any(x => x.GetType() == this.GetType()))
        {
            ExecuteNgoDao(caster, target, data);

        }


    }
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out SecureInt sucThe))
        {
            target.AddDamageData(new List<int> { -(int)(buff * target.allDamages[0] / 100) });
        }
    }

    public override string GetDes()
    {
        return $"Nếu bạn ở trạng thái {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)} thì nhận tổn thương giảm xuống {BaseSkill.GetColoredText($"{buff}%", "red")}";



    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ObserverEffect);
    }
}
