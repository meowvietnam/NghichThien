using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NguKimMocThuyHoaTho), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(NguKimMocThuyHoaTho))]
public class NguKimMocThuyHoaTho : NgoDaoCombat
{
    [SerializeField] private ELinhKhi elinhKhi;
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddDamageData(new System.Collections.Generic.List<int> {-target.GetLinhKhi(elinhKhi)});
    }
    public override void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        if (target == null) return;
        // kiểm tra thần thông vừa truyền vào của caster có phải mộc k  
        if (target.ngoDaoCombats.Any(x => x.GetType() == this.GetType()) && BaseSkill.CheckIsEqualETypeSkill(caster, new BaseSkill.ETypeSkill[] { GetETypeSkillKhacWithLinhKhi(elinhKhi) }))
        {
            ExecuteNgoDao(caster, target);

        }


    }
    public void ObserverEffect2(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        BaseSkill.ETypeSkill[] eTypeSkills = (BaseSkill.ETypeSkill[])data[3];

        if (target == null) return;
        // kiểm tra dame được truyền vào có phải là mộc linh khí không
        if (target.ngoDaoCombats.Any(x => x.GetType() == this.GetType()) && eTypeSkills != null && BaseSkill.CheckIsEqualETypeSkill(eTypeSkills, new BaseSkill.ETypeSkill[] { GetETypeSkillKhacWithLinhKhi(elinhKhi) }))
        {
            ExecuteNgoDao(caster, target);

        }


    }
    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME_BY_STATIC, ObserverEffect2);

    }
    public override global::System.String GetDes()
    {
        return $"Mỗi một điểm {BaseSkill.GetTextLinhKhi(elinhKhi)} hệ linh khí, nhận {BaseSkill.GetTextTypeSkill(GetETypeSkillKhacWithLinhKhi(elinhKhi))} hệ tổn thương -1";
    }
    private BaseSkill.ETypeSkill GetETypeSkillKhacWithLinhKhi(ELinhKhi elinhKhi)
    {
        switch (elinhKhi)
        {
            case ELinhKhi.None:
                break;
            case ELinhKhi.Kim:
                return BaseSkill.ETypeSkill.Moc;
            case ELinhKhi.Moc:
                return BaseSkill.ETypeSkill.Tho;
            case ELinhKhi.Thuy:
                return BaseSkill.ETypeSkill.Hoa;
            case ELinhKhi.Hoa:
                return BaseSkill.ETypeSkill.Kim;
            case ELinhKhi.Tho:
                return BaseSkill.ETypeSkill.Thuy;
            case ELinhKhi.Ma:
                break;
            default:
                break;
        }
        return BaseSkill.ETypeSkill.None;
    }

}
