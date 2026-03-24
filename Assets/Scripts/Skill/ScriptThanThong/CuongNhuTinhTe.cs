using UnityEngine;
[CreateAssetMenu(fileName = nameof(CuongNhuTinhTe), menuName = "Skill/Thần Thông/" + nameof(CuongNhuTinhTe))]

public class CuongNhuTinhTe : ThanThong
{
    [SerializeField] private int thuyLinhKhi = 4;
    [SerializeField] private int[] kiemKhiPerKinhDaoValues;
    [SerializeField] private int[] triLieuPerChiThuyValues;

    [SerializeField] private float delayTimeEffect = 1f;


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.HapThuLinhKhi(ELinhKhi.Thuy, thuyLinhKhi);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KinhDao, out SecureInt kinhDao);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChiThuy, out SecureInt chiThuy);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, triLieuPerChiThuyValues[GetIndexLevelThanThong(caster)] * chiThuy);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, kiemKhiPerKinhDaoValues[GetIndexLevelThanThong(caster)] * kinhDao);

        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KinhDao, kinhDao);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChiThuy, chiThuy);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Thu hoạch được {GetColoredText($"+{thuyLinhKhi}", "red")} điểm {GetTextLinhKhi(ELinhKhi.Thuy)} hệ linh khí. Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)}. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} thu hoạch được {GetColoredText($"+{kiemKhiPerKinhDaoValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)} thu hoạch được {GetColoredText($"+{triLieuPerChiThuyValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)}";
    }
}
