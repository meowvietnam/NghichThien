using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(LietKhiTram), menuName = "Skill/Thần Thông/" + nameof(LietKhiTram))]
public class LietKhiTram : ThanThong, IMemorySerializable<LietKhiTram>
{
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out SecureInt hoTrao);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { hoTrao }, eTypeSkill);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return
            $"Tạo thành lượng sát thương tương đương với số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} bạn đang có";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LietKhiTram value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}