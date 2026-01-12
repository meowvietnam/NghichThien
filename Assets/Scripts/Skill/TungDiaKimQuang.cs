using UnityEngine;
[CreateAssetMenu]
public class TungDiaKimQuang : ThanThong
{
    [SerializeField]private int buff = 8;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongCoolDown(this, 1);
        var linhKhiHapThu = caster.HapThuLinhKhi(8);
        caster.HapThuLinhKhi(ELinhKhi.Kim,linhKhiHapThu[ELinhKhi.Kim]);
        ShowSkillEffect(caster, 1f);

    }

    public override string GetDesc()
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ {GetColoredText($"ngẫu nhiên {buff} điểm", "red")} linh khí. Chỉ giữ lại Kim hệ linh khí";
    }


}
