using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CuuUHuyetMaCong))]
public class CuuUHuyetMaCong : CongPhap, IMemorySerializable<CuuUHuyetMaCong>
{
    [SerializeField]
    private int[] buffValues = new int[] { 10, 20, 30, 40, 50 };
    [SerializeField]
    private int[] tieuHao = new int[] { 10, 20, 30, 40, 50 };
    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (caster != null && target != null && IsCanUse(target))
        {
            Execute(caster, target, data);
        }
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // target là ng nhận dame mục đích giảm dame cp cho target
        int indexCP = CurrentLevelCongPhapInCombat(target) - 1;
        float multiplier =   buffValues[indexCP] / 100f;
        target.AddDamageData(new List<int> {-(int)(target.GetDameBuffForTarget() * multiplier) });
        target.characterStatsBase.thoNguyen -= tieuHao[indexCP];

    }
    public override string GetDesc(int level)
    {
        return $"Tiêu hao {GetColoredText($"-{tieuHao[level - 1]}", "red")} thọ nguyên giảm {GetColoredText($"-{buffValues[level - 1]}%", "red")} tổn thương phải gánh chịu";
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

    public void Deserialize(IReader reader, ref CuuUHuyetMaCong value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}
