using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KimNguyenKiemMang), menuName = "Skill/Thần Thông/" + nameof(KimNguyenKiemMang))]
public class KimNguyenKiemMang : ThienNhanThuat, BaseSkill.TuongDong, IMemorySerializable<KimNguyenKiemMang>
{
    [SerializeField] private int[] dameBuff;

    public void ExecuteTuongDong(CharacterCombat caster, CharacterCombat target)
    {
        target.CaculateDameByThanThong(caster, new List<int> { dameBuff[GetIndexLevelThanThong(caster)] }, eTypeSkill,
            true);
    }

    public override string GetDesc(int level)
    {
        return
            $"Tiêu hao {requireKiemKhi} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} tạo thành {GetColoredText($"{damePerCountAttack[level - 1]}", "red")} điểm sát thương. Nếu {GetKeyWord(EKeyWord.TuongDong)} đối phương nhận thêm {GetColoredText($"+{dameBuff[level - 1]} điểm", "red")} sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KimNguyenKiemMang value)
    {
        ThienNhanThuat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}