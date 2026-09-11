using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(DoatLinhThuat), menuName = "Skill/Thần Thông/" + nameof(DoatLinhThuat))]
public class DoatLinhThuat : ThanThong, IMemorySerializable<DoatLinhThuat>
{
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.HapThuLinhKhi(ELinhKhi.Kim, target.khiKim);
        caster.HapThuLinhKhi(ELinhKhi.Moc, target.khiMoc);
        caster.HapThuLinhKhi(ELinhKhi.Thuy, target.khiThuy);
        caster.HapThuLinhKhi(ELinhKhi.Hoa, target.khiHoa);
        caster.HapThuLinhKhi(ELinhKhi.Tho, target.khiTho);
        caster.HapThuLinhKhi(ELinhKhi.Ma, target.khiMa);
        target.TieuTanAllLinhKhi();
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Đánh cắp toàn bộ linh khí của đối thủ";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref DoatLinhThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}