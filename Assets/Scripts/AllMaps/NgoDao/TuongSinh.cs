using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TuongSinh), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TuongSinh))]
public class TuongSinh : NgoDaoCombat
{
    [SerializeField] private ELinhKhi elinhKhi;
    [SerializeField] private int buff = 6;


    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddNgoDaoCombatDynamic(this);

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
        if(BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire))
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TuKhi, buff);
            caster.RemoveNgoDaoCombatDynamic(this);
        }    

    }

    public override string GetDes()
    {
        return $"Mỗi hiệp lần đầu sử dụng {BaseSkill.GetTextLinhKhi(elinhKhi)} hệ thần thông. Đồng thời phát động ngũ hành {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.LienKich)} thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TuKhi)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
        Observer.AddObserver(EventList.Combat.LIEN_KICH, ObserverEffect2);

    }

}
