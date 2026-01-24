using UnityEngine;
[CreateAssetMenu(fileName = nameof(QuyNguyenKiemKhi), menuName = "Skill/Thần Thông/" + nameof(QuyNguyenKiemKhi))]
public class QuyNguyenKiemKhi : TietMach
{
    protected override EBuffAndDeBuff eBuffOrDebuff => EBuffAndDeBuff.ChayMau;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        KiemY(caster);
    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} "+base.GetDesc(level);
    }
}
