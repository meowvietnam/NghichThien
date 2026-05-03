using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(LinhSaThuat), menuName = "Skill/Thần Thông/" + nameof(LinhSaThuat))]

public class LinhSaThuat : ThanThong
{


    [SerializeField] protected float delayTimeEffect = 1f;

    [SerializeField]
    private int require = 1;
    [SerializeField]
    private int hapThu = 1;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Tran, require);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
        ShowSkillEffectWorld(caster, delayTimeEffect);


    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        ThanThong thanThong = (ThanThong)data[2];

        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is LinhSaThuat) )
        {
            if(thanThong is HoanTran)
            {
                int count = currentTurn.specialThanThongImpactAfterPlaySkill.Count(x => x is LinhSaThuat);
                for (int i = 0; i < hapThu * count; i++)
                {
                    var linhkhi = targetTurn.GetRandomLinhKhiConLai();
                    targetTurn.TieuTanLinhKhi(linhkhi, 1);
                    currentTurn.HapThuLinhKhi(linhkhi, 1);
                }
                currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);

            }


        }


    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out SecureInt  tran);
        if (tran >= require && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }
    public override string GetDesc(int level)
    {
        return $"Tiêu hao {GetColoredText($"{require}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}, sử dụng thần thông lần sau nếu là {GetColoredText($"[Hoàn Trần]", "red")} thì ngẫu nhiên hấp thụ {GetColoredText($"{hapThu}", "red")} điểm linh khí của đối thủ (Có thể cộng dồn)";

    }
}
