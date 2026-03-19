using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(PheTam), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(PheTam))]
public class PheTam : NgoDaoCombat
{
    [SerializeField] private int requireActiveSkil = 50;
    [SerializeField] private int buff = 6;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
       if(target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc,out SecureInt TrungDoc))
       {
            if(TrungDoc > requireActiveSkil)
            {
                target.AddBuffAndDebuffCount(EBuffAndDeBuff.TreKhi, buff);
            }
       }    
    }

    public override string GetDes()
    {
        return $"Bắt đầu hiệp, nếu đối thủ số tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)} lớn hơn {BaseSkill.GetColoredText($"{requireActiveSkil}", "red")} thì nhận phải {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TreKhi)}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
    }

}
