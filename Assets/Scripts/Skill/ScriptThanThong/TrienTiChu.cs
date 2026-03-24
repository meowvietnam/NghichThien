using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TrienTiChu), menuName = "Skill/Thần Thông/" + nameof(TrienTiChu))]

public class TrienTiChu : ThanThong
{
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
      
        HauKeVoLuc(caster);
        target.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ObserverEffect);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is TrienTiChu))
        {
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
            // end turn giữ lại 1 nữa được xử lí ở character combat

        }


    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        SecureInt trienNhieu = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out trienNhieu);
        return trienNhieu > 0 && base.IsCanUse(caster, target);
    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Đối thủ kết thúc hiệp, thì giữ lại một nửa số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)} của hắn";

    }
}
