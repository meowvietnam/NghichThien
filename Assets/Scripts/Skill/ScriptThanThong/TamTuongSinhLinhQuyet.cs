using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TamTuongSinhLinhQuyet), menuName = "Skill/Thần Thông/" + nameof(TamTuongSinhLinhQuyet))]
public class TamTuongSinhLinhQuyet : ThanThong, IMemorySerializable<TamTuongSinhLinhQuyet>
{
    [SerializeField]
    private int[] buffs;
    [SerializeField]
    private int tuKhi = 2;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TuKhi, tuKhi);
        float percent = (float)buffs[GetIndexLevelThanThong(caster)] / 100f;
        caster.Healing((int)(percent * (caster.characterStatsTemp.hpMax - caster.characterStatsTemp.hp)));
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Hồi phục {GetColoredText($"{buffs[level - 1]}%", "red")} lượng Hp đã mất. Đồng thời thu hoạch được {GetColoredText($"{tuKhi}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TuKhi)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TamTuongSinhLinhQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}