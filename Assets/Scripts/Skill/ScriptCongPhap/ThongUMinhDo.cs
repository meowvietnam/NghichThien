using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThongUMinhDo))]
public class ThongUMinhDo : HoanAnhDien, IMemorySerializable<ThongUMinhDo>
{
    [SerializeField]
    private int[] buffValues2 = new int[]{1, 2, 3, 4, 5};
    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (target != null && IsCanUse(target))
        {
            Execute(caster, target, data);
        }
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // target là ng né thành công
        // caster là ng gây dame
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.AnhDich, buffValues2[CurrentLevelCongPhapInCombat(target) - 1]);
    }

    public override string GetDesc(int level)
    {
        return $"Mỗi lần né tránh thành công thu hoạch được {GetColoredText($"+{buffValues2[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnhDich)}. {base.GetDesc(level)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        base.InitPosCallExecute(caster);
        Observer.AddObserver(EventList.Combat.NE_TRANH_SUCCESS, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThongUMinhDo value)
    {
        HoanAnhDien baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}