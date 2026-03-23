using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HoaDoc), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(HoaDoc))]
public class HoaDoc : NgoDaoCombat
{
    [SerializeField] private int buff = 1;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc,out SecureInt trungDoc) && BaseSkill.CheckIsEqualETypeSkill(caster,new BaseSkill.ETypeSkill[] { BaseSkill.ETypeSkill.Hoa}))
        {

            target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, buff);
        }    
        else if (target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt thieuDot) && BaseSkill.CheckIsEqualETypeSkill(caster, new BaseSkill.ETypeSkill[] { BaseSkill.ETypeSkill.Moc }))
        {
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, buff);

        }
    }

    public override string GetDes()
    {
        return $"Đối thủ ở trạng thái {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)} khi sử dụng {BaseSkill.GetTextLinhKhi(ELinhKhi.Hoa)} hệ thần thông làm tổn thương đối thủ nhận phải {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}. Đối thủ ở trạng thái {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} khi sử dụng {BaseSkill.GetTextLinhKhi(ELinhKhi.Moc)} hệ thần thông làm tổn thương đối thủ nhận phải {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
    }
}
