using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CuuHuyenNoiKinh))]
public class CuuHuyenNoiKinh : CongPhap, IMemorySerializable<CuuHuyenNoiKinh>
{
    [SerializeField]
    private int[] buffValues = new int[]{4, 5, 6, 7, 8};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = target.listAmKinhTT.Where(x => x.ThanThong is AmKinhTT).Select(x => x.ThanThong.name).Distinct().Count();
        float multiplier = count * buffValues[CurrentLevelCongPhapInCombat(caster) - 1] / 100f;
        target.AddDamageData(new List<int>{(int)(target.allDamages[0] * multiplier)});
    }

    public override string GetDesc(int level)
    {
        return $"Đối thủ mỗi có một loại {GetKeyWord(EKeyWord.AmKinh)} khác biệt thì nhận tổn thương tăng lên {GetColoredText($"+{buffValues[level - 1]}%", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref CuuHuyenNoiKinh value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}