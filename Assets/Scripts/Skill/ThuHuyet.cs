using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThuHuyet), menuName = "Skill/Thần Thông/" + nameof(ThuHuyet))]
public class ThuHuyet : ThanThong
{
    [SerializeField] private int[] hapThu;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KietSuc,1);
        foreach (var item in caster.DictHapThuLinhKhi(hapThu[GetIndexLevelThanThong(caster)]))
        {
            caster.HapThuLinhKhi(item.Key, item.Value);
        }
        ShowSkillEffectUI(caster, 1.5f);

    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ {GetColoredText($"ngẫu nhiên {hapThu[level-1]} điểm", "red")} linh khí. Bạn bị {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KietSuc)}";
    }
}
