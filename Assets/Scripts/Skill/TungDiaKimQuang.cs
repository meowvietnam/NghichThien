using UnityEngine;
[CreateAssetMenu]
public class TungDiaKimQuang : ThanThong
{
    [SerializeField]private int[] buff;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongCoolDown(this, 1);
        var linhKhiHapThu = caster.DictHapThuLinhKhi(buff[GetIndexLevelThanThong(caster)-1]);
        caster.HapThuLinhKhi(ELinhKhi.Kim,linhKhiHapThu[ELinhKhi.Kim]);
        ShowSkillEffect(caster, 1f);

    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ {GetColoredText($"ngẫu nhiên {buff[level-1]} điểm", "red")} linh khí. Chỉ giữ lại Kim hệ linh khí";
    }


}
