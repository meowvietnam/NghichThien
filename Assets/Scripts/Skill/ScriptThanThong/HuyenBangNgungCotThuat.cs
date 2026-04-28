 using System.Linq;
 using Skill;
 using UnityEngine;
[CreateAssetMenu(fileName = nameof(HuyenBangNgungCotThuat), menuName = "Skill/Thần Thông/" + nameof(HuyenBangNgungCotThuat))]

public class HuyenBangNgungCotThuat : ThanThong
{
    [SerializeField] private int requireSuongGia = 10;
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.RemoveBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, requireSuongGia);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DongKet, 1);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Gỡ ra {GetColoredText($"{requireSuongGia}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)} của đối thủ. Đồng thời, đối phương nhận phải {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DongKet)} {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";

    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SuongGia, out SecureInt  suongGia);
        if (suongGia >= requireSuongGia && !caster.thanThongUseThisComBat.Any(x => x.Any(y=>y.thanThong is HuyenBangNgungCotThuat)) && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }
}
