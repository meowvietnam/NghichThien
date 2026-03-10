using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(LacThachTran), menuName = "Skill/Thần Thông/" + nameof(LacThachTran))]

public class LacThachTran : TranPhapTT
{
    [SerializeField]
    private int require = 5;
    [SerializeField] private ThanThong thanThongActive;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ObserverEffect);


    }
    public override void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (targetTurn == null) return;

        if (targetTurn.specialThanThongImpactAfterPlaySkill.Any(x => x.GetType() == this.GetType()))
        {
            ExecuteTranPhap(currentTurn, targetTurn);

        }
    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        int count = target.specialThanThongImpactAfterPlaySkill.Count(x=> x is LacThachTran);
        if (count >= require)
        {
            Observer.Notify(EventList.GamePlay.PLAY_SKILL, target, caster, thanThongActive, new List<LinhKhiTieuTan>());
            target.RemoveAllThanThongImpactAfterPlaySkill(this);
            target.AddThanThongImpactAfterPlaySkill(this);

        }
        else
        {
            target.AddThanThongImpactAfterPlaySkill(this);

        }


    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Mỗi nhận {GetColoredText($"{require}", "red")} lần công kích thì phóng thích một lần [Lạc Thạch] phản kích đối thủ";
    }
}
