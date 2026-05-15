using LongDH.LongDHModules;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ChanNguyenHuyenDien))]
public class ChanNguyenHuyenDien : CongPhap, IMemorySerializable<ChanNguyenHuyenDien>
{
    [SerializeField]
    private int[] chanMocs;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int countDebuff = 0;
        foreach (var item in target.buffAndDebuffCount)
        {
            if (eDebuffs.Contains(item.Key))
            {
                countDebuff++;
            }
        }

        for (int i = 0; i < countDebuff; i++)
        {
            var buff = eBuffs[CombatStatic.IndexRandom(0, eDebuffs.Count)];
            caster.AddBuffAndDebuffCount(buff, 1);
        }

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanMoc, countDebuff * chanMocs[CurrentLevelCongPhapInCombat(caster) - 1]);
    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, đối thủ mỗi có 1 loại trạng thái tiêu cực thì bản thân thu hoạch được ngẫu nhiên 1 loại trang thái tích cực. Đồng thời, cũng thu hoạch được {GetColoredText($"+{chanMocs[level - 1]}", "red")} {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanMoc)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ChanNguyenHuyenDien value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}