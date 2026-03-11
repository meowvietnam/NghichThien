using UnityEngine;
[CreateAssetMenu(fileName = nameof(NoThuyThienThuong), menuName = "Skill/Thần Thông/" + nameof(NoThuyThienThuong))]

public class NoThuyThienThuong : ThanThong
{
    [SerializeField] private int[] chiThuyReset;
    [SerializeField] private int chanKiem = 1;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KinhDao, out SecureInt  kinhDao);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChiThuy, out SecureInt  chiThuy);

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKiem, kinhDao * chanKiem);

        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KinhDao, kinhDao);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChiThuy, chiThuy);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChiThuy, chiThuyReset[GetIndexLevelThanThong(caster)]);

    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)}. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} thu hoạch được {GetColoredText($"+{chanKiem}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanKiem)}, Làm mới {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)} thành {GetColoredText($"{chiThuyReset[level - 1]}", "red")} tầng";
    }
}
