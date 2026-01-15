using UnityEngine;
[CreateAssetMenu]
public class ThuHuyet : ThanThong
{
    [SerializeField] private int[] hapThu;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongCoolDown(this,1);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KietSuc,1);
        foreach (var item in caster.DictHapThuLinhKhi(hapThu[GetIndexLevelThanThong(caster)-1]))
        {
            caster.HapThuLinhKhi(item.Key, item.Value);
        }
        ShowSkillEffect(caster, 1.5f);

    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ {GetColoredText($"ngẫu nhiên {hapThu[level-1]} điểm", "red")} linh khí. Bạn bị {GetDescBuffOrDeBuff(EBuffAndDeBuff.KietSuc)}";
    }
}
