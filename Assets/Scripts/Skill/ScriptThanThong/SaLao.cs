using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(SaLao), menuName = "Skill/Thần Thông/" + nameof(SaLao))]

public class SaLao : ThanThong
{
    [SerializeField] private HoaCat hoaCat;
    [SerializeField]
    private int[] trienNhieus = new int[]
    {
        2,3,4,6,10
    };
    [SerializeField] protected float delayTimeEffect = 0.85f;


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongCoolDown(hoaCat, -1);
        Observer.Notify(EventList.Combat.PLAY_SKILL, caster, target, hoaCat, new List<LinhKhiTieuTan>());
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, trienNhieus[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(target, delayTimeEffect);

    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out SecureInt  tran);
        if (tran > 0 && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }


   
    public override string GetDesc(int level)
    {
        return $"Bản thân có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} mới có thể sử dụng. {GetKeyWord(EKeyWord.HauKeVoLuc)} kích hoạt thần thông {GetColoredText($"[Hóa Cát]", "red")}. Đối thủ nhận phải {GetColoredText($"+{trienNhieus[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}";

    }
}
