using UnityEngine;
using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HacHoiTuong), menuName = "Skill/Thần Thông/" + nameof(HacHoiTuong))]
public class HacHoiTuong : ThanThong
{
    [SerializeField] private ThanThong thanThongActive;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        SecureInt cham = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Cham, out cham);
        for (int i = 0; i < cham; i++)
        {
            Debug.Log("HacHoiTuong");
            Observer.Notify(EventList.Combat.PLAY_SKILL, caster, target, thanThongActive, new List<LinhKhiTieuTan>());

        }




    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        SecureInt cham = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Cham, out cham);

        return cham > 0 && base.IsCanUse(caster, target);
    }
    public override string GetDesc(int level)
    {
        return $"Liên tục phóng thích X lần thần thông [Phi Châm Thuật] (X là số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cham)} bạn đang có)";

    }
}
