using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TiDocChanDiem), menuName = "Skill/Thần Thông/" + nameof(TiDocChanDiem))]
public class TiDocChanDiem : ThanThong, IMemorySerializable<TiDocChanDiem>
{
    [SerializeField]
    private int[] requires = new int[]{1, 2, 3, 5, 7};
    [SerializeField]
    private int[] healValues = new int[]{8, 9, 12, 15, 21};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, requires[GetIndexLevelThanThong(caster)]);
        caster.Healing(healValues[GetIndexLevelThanThong(caster)]);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out SecureInt trungDoc);
        if (trungDoc >= requires[GetIndexLevelThanThong(caster)] && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return $"Bản thân gỡ ra {GetColoredText($"{requires[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}, khôi phục {GetColoredText($"+{healValues[level - 1]}", "red")} điểm HP";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TiDocChanDiem value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}