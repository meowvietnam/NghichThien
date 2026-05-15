using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThongMoc), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ThongMoc))]
public class ThongMoc : NgoDaoCombat
{
    [SerializeField] private int buff = 10;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(BaseSkill.CheckIsEqualETypeSkill(caster,eTypeRequire))
        {
            Action(caster, target);
        }    

    }
    public void ObserverEffect2(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        if (caster == null) return;

        if (caster.ngoDaoCombats.Any(x => x.Id == this.Id))
        {
            ExecuteNgoDao2(caster, target, data);

        }


    }
    public void ExecuteNgoDao2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        BaseSkill.ETypeSkill[] eTypeSkills = (BaseSkill.ETypeSkill[])data[3];

        if (BaseSkill.CheckIsEqualETypeSkill(eTypeSkills, eTypeRequire))
        {
            Action(caster, target);
        }

    }
    private void Action(CharacterCombat caster, CharacterCombat target)
    {
        var debuffKeys = target.buffAndDebuffCount.Keys
               .Where(key => BaseSkill.eDebuffs.Contains(key))
               .ToList();
        // 2. Kiểm tra xem có debuff nào để xóa không
        if (debuffKeys.Count > 0)
        {
            target.AddDamageData(new List<int> { (int)(target.allDamages[0] * buff * debuffKeys.Count / 100) });

        }
    }    

    public override string GetDes()
    {
        return $"Đối thủ mỗi có một loại trạng thái tiêu cực khác biệt thì nhận {BaseSkill.GetTextLinhKhi(ELinhKhi.Moc)} hệ tổn thương tăng lên {BaseSkill.GetColoredText($"{buff}%", "red")}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME_BY_STATIC, ObserverEffect2);

    }
}
