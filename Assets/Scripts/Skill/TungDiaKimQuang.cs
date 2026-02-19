using UnityEngine;
[CreateAssetMenu(fileName = nameof(TungDiaKimQuang), menuName = "Skill/Thần Thông/" + nameof(TungDiaKimQuang))]
public class TungDiaKimQuang : ThanThong
{
    [SerializeField]private int[] buff;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        var linhKhiHapThu = caster.DictHapThuLinhKhi(buff[GetIndexLevelThanThong(caster)]);
        caster.HapThuLinhKhi(ELinhKhi.Kim,linhKhiHapThu[ELinhKhi.Kim]);
        ShowSkillEffect(caster, 1f);

    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ {GetColoredText($"ngẫu nhiên {buff[level-1]} điểm", "red")} linh khí. Chỉ giữ lại Kim hệ linh khí";
    }


}
