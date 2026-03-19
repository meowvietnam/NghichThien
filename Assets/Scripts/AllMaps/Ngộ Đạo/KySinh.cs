using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KySinh), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(KySinh))]

public class KySinh : NgoDaoCombat
{

    [SerializeField] private int buff = 2;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu,out SecureInt trienNhieu))
        {
            target.AddDamageData(new List<int> {buff});
        }    
    }

    public override string GetDes()
    {
        return $"Nếu đối thủ ở trạng thái {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)} thì tổn thương {BaseSkill.GetColoredText($"+{buff}", "red")}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ObserverEffect);
    }
}
