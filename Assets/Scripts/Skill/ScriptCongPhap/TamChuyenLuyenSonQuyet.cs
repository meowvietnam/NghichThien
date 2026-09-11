using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TamChuyenLuyenSonQuyet))]
public class TamChuyenLuyenSonQuyet : CongPhap, IMemorySerializable<TamChuyenLuyenSonQuyet>
{
    [SerializeField] private int[] buffValues = new int[] { 3, 4, 5, 6, 8 };
    [SerializeField] private int require = 6;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int sum = caster.thanThongCurrentUse.linhKhiTieuTan.Sum(x => x.count);
        if (CheckIsEqualETypeSkill(caster) && sum >= require)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran,
                sum * buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Sử dụng {GetTextLinhKhi(ELinhKhi.Tho)} hệ thần thông nếu tiêu tán tổng ít nhất {GetColoredText($"{require}", "red")} điểm linh khí thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}*X", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} (X là tổng số linh khí tiêu tán)";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TamChuyenLuyenSonQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}