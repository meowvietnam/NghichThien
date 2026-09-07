using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(DanDocChu), menuName = "Skill/Thần Thông/" + nameof(DanDocChu))]
public class DanDocChu : ThanThong, IMemorySerializable<DanDocChu>
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        CombatStatic.TrungDoc(caster, target);
    }

    public override List<RequireActive> GetRequireActive(CharacterCombat caster)
    {
        List<RequireActive> listRqA = new List<RequireActive>();
        foreach (var requireActive in requireActives)
        {
            // clone object
            RequireActive clone = new RequireActive()
                { linhKhiRequire = requireActive.linhKhiRequire, count = requireActive.count };
            if (clone.linhKhiRequire == ELinhKhi.Moc)
            {
                int extra = caster.thanThongUseThisTurn.Count(x => x.thanThong is DanDocChu);
                clone.count += extra;
            }

            listRqA.Add(clone);
        }

        return listRqA;
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        SecureInt countTrungDoc = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrungDoc, out countTrungDoc);
        return countTrungDoc > 0 && base.IsCanUse(caster, target);
    }

    public override string GetDesc(int level)
    {
        return
            $"Đối phương bị kích phát hiệu quả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}. Trong hiệp này, mỗi một lần sử dụng thần thông này thì tiêu hao thêm một điểm {GetTextLinhKhi(ELinhKhi.Moc)} linh khí";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref DanDocChu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}