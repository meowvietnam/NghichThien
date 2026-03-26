using UnityEngine;
[CreateAssetMenu(fileName = nameof(LamThuong), menuName = "Skill/Thần Thông/" + nameof(LamThuong))]

public class LamThuong : ThanThong
{
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int linhKhiHapThu = caster.totalKhi - caster.CurrentLinhKhi;
        linhKhiHapThu = Mathf.Clamp(linhKhiHapThu, 0, int.MaxValue);
        var dic = caster.DictHapThuLinhKhi(linhKhiHapThu);
        foreach ( var key in dic)
        {
            caster.HapThuLinhKhi(key.Key, key.Value);
        }
        ShowSkillEffectWorld(caster, delayTimeEffect);


    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Hấp thụ số linh khí thẳng đến số linh khí tối đa (Nên dùng khi số linh khí còn lại ít hơn số linh khí tối đa)";
    }

}
