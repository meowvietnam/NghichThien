using UnityEngine;
[CreateAssetMenu]
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
