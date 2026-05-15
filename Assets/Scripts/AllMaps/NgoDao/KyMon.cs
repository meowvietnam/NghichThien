using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KyMon), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(KyMon))]
public class KyMon : NgoDaoCombat, IMemorySerializable<KyMon>
{
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDes()
    {
        return $"Sau khi bày {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.TranPhap)}, có thể sử dụng lại thần thông này khu động {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.TranPhap)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KyMon value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}