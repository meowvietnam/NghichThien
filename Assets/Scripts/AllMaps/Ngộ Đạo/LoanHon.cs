using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(LoanHon), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(LoanHon))]
public class LoanHon : NgoDaoCombat
{

    [SerializeField] private int buff = 50;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(target.GetThanThuc() <= 0)
        {
            target.AddDamageData(new List<int> { target.allDamages[0] * buff / 100 });

        }

    }

    public override string GetDes()
    {
        return $"Đối thủ thần thức xuống 0 điểm nhận thần thông tổn thương tăng lên {BaseSkill.GetColoredText($"{buff}%", "red")} điểm sát thương";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
    }

}
