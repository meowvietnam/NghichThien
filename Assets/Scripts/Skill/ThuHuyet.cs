using UnityEngine;
[CreateAssetMenu]
public class ThuHuyet : ThanThong
{
    [SerializeField] private int hapThu = 6;
    public override void Execute(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddThanThongCoolDown(this,1);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KietSuc,1);
        foreach (var item in caster.HapThuLinhKhi(hapThu))
        {
            caster.HapThuLinhKhi(item.Key, item.Value);
        }
        ShowSkillEffect(caster, 1.5f);

    }

    public override string GetDesc()
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ {GetColoredText($"ngẫu nhiên {hapThu} điểm", "red")} linh khí. Bạn bị {GetDescBuffOrDeBuff(EBuffAndDeBuff.KietSuc)}";
    }
}
