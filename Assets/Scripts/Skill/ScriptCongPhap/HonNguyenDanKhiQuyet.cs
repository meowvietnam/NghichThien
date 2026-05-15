using LongDH.LongDHModules;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HonNguyenDanKhiQuyet))]
public class HonNguyenDanKhiQuyet : CongPhap, IMemorySerializable<HonNguyenDanKhiQuyet>
{
    [SerializeField]
    private int[] buffValues = new int[]{6, 12, 24, 48, 96};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DanLuc, out SecureInt danluc);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out SecureInt tran);
        if (danluc > 0 && tran > 0)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }
    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, nếu bản thân có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DanLuc)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
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

    public void Deserialize(IReader reader, ref HonNguyenDanKhiQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}