using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NgungMach), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(NgungMach))]
public class NgungMach : NgoDaoCombat
{
    [SerializeField] private int buff = 3;
    [SerializeField] private int requirePer = 50;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
       if(target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SuongGia , out SecureInt suongGia))
       {
            int count = suongGia / requirePer;

            if (suongGia > 1)
            {
                count += buff;
            }
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.NoiThuong,  count);

        }
    }

    public override string GetDes()
    {
        return $"Bắt đầu hiệp, nếu đối thủ có số tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)} lớn hơn {BaseSkill.GetColoredText($"{requirePer}", "red")} thì nhận phải {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NoiThuong)}. Đồng thời, mỗi có {BaseSkill.GetColoredText($"{requirePer}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)} thì nhận phải {BaseSkill.GetColoredText($"+{1}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NoiThuong)}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
    }

}
