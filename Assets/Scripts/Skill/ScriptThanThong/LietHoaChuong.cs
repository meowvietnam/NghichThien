using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(LietHoaChuong), menuName = "Skill/Thần Thông/" + nameof(LietHoaChuong))]
public class LietHoaChuong : ThanThong, IMemorySerializable<LietHoaChuong>
{
    [SerializeField]
    private int[] dameValues = new int[]{12, 16, 24, 45, 90};
    [SerializeField]
    private int[] dameValueBuffs = new int[]{1, 2, 3, 4, 5};
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dameValues[GetIndexLevelThanThong(caster)];
        int dameBuff = dameValueBuffs[GetIndexLevelThanThong(caster)] * caster.khiHoa;
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame + dameBuff}, eTypeSkill);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương. Mỗi còn lại một điểm {GetTextLinhKhi(ELinhKhi.Hoa)} hệ linh khí thì tăng lên {GetColoredText($"+{dameValueBuffs[level - 1]}", "red")} điểm sát thương ";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LietHoaChuong value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}