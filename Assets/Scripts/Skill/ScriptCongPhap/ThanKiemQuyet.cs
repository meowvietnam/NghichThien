using LongDH.LongDHModules;
using NUnit.Framework;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThanKiemQuyet))]
public class ThanKiemQuyet : CongPhap, IMemorySerializable<ThanKiemQuyet>
{
    [SerializeField]
    private int[] buff;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CheckIsEqualETypeSkill(caster))
        {
            SecureInt dame = 0;
            caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out dame);
            dame *= buff[CurrentLevelCongPhapInCombat(caster) - 1];
            target.AddDamageData(new List<int>{dame}, true);
        }
    }

    public void ExecuteParams2(params object[] data)
    {
        // target là ng nh?n dame 
        // ktra xem ng nh?n dame có công pháp này k
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Tìm trong danh sách trang b? c?a caster
        if (caster != null && IsCanUse(caster))
        {
            Execute2(caster, target, data);
        }
    }

    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ETypeSkill[] eTypeSkills = (ETypeSkill[])data[3];
        if (eTypeSkills != null && CheckIsEqualETypeSkill(eTypeSkills))
        {
            SecureInt dame = 0;
            caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out dame);
            dame *= buff[CurrentLevelCongPhapInCombat(caster) - 1];
            target.AddDamageData(new List<int>{dame}, true);
        }
    }

    public void ExecuteParams3(params object[] data)
    {
        // target là ng nh?n dame 
        // ktra xem ng nh?n dame có công pháp này k
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Tìm trong danh sách trang b? c?a caster
        if (caster != null && IsCanUse(caster))
        {
            Execute3(caster, target, data);
        }
    }

    public void Execute3(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        SecureInt count = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out count);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, count);
    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} bây giờ có thể bị gỡ ra. Khi tạo thành {GetTextTypeSkill()} hệ tổn thương thì đối phương nhận thêm sát thương bằng  {GetColoredText($"{buff[level - 1]} lần", "red")} số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} đang có.";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME_BY_STATIC, ExecuteParams2);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams3);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThanKiemQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}