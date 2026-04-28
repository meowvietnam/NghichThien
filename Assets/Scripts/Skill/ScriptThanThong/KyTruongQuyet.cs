using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KyTruongQuyet), menuName = "Skill/Thần Thông/" + nameof(KyTruongQuyet))]
public class KyTruongQuyet : ThanThong
{

    [SerializeField] private int hapThuLinhKhi = 3;
    [SerializeField]
    private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        var dicHapThu = caster.DictHapThuLinhKhi(hapThuLinhKhi);
        foreach ( var item in dicHapThu )
        {
            int countHapThu = item.Value;
            if(item.Key == ELinhKhi.Moc)
            {
                countHapThu *= 2;
            }    
            caster.HapThuLinhKhi(item.Key, countHapThu);
        }
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }

 
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ {GetColoredText($"+{hapThuLinhKhi} điểm", "red")} linh khí, nếu là {GetTextLinhKhi(ELinhKhi.Moc)} hệ linh khí thì hấp thụ gấp đôi";
    }
}
