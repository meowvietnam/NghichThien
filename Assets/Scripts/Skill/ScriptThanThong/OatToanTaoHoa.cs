using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(OatToanTaoHoa), menuName = "Skill/Thần Thông/" + nameof(OatToanTaoHoa))]
public class OatToanTaoHoa : ThanThong, IMemorySerializable<OatToanTaoHoa>
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KietSuc, 1);
        caster.HapThuLinhKhi(ELinhKhi.Kim, caster.khiKim);
        caster.HapThuLinhKhi(ELinhKhi.Moc, caster.khiMoc);
        caster.HapThuLinhKhi(ELinhKhi.Thuy, caster.khiThuy);
        caster.HapThuLinhKhi(ELinhKhi.Hoa, caster.khiHoa);
        caster.HapThuLinhKhi(ELinhKhi.Tho, caster.khiTho);
        caster.HapThuLinhKhi(ELinhKhi.Ma, caster.khiMa);
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KietSuc)} phục chế tất cả linh khí (Mỗi loại linh khí còn lại số lượng tăng gấp đôi) {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong is OatToanTaoHoa)) &&
            base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref OatToanTaoHoa value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}