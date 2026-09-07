using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(AmDuongDong), menuName = "Skill/Thần Thông/" + nameof(AmDuongDong))]
public class AmDuongDong : ThanThong, IMemorySerializable<AmDuongDong>
{
    [SerializeField] private float delayTimeEffect = 2f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddSkillVoHieuHoa(this);
        caster.AddThanThongImpactAfterPlaySkill(this);
        PlayVisualSkill(caster, target);
    }

    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectNoTarget(delayTimeEffect);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong is AmDuongDong)) &&
            base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return
            $"Làm cho đối thủ lần sử dụng thần thông tiếp theo mất đi hiệu lực {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref AmDuongDong value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}