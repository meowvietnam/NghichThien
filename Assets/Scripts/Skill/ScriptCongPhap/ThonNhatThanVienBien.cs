using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThonNhatThanVienBien))]
public class ThonNhatThanVienBien : CongPhap, IMemorySerializable<ThonNhatThanVienBien>
{
    [SerializeField] private int[] buffValues = new int[] { 22, 26, 30, 34, 38 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // có trong dynamic 
        Debug.Log(
            $"[ThonNhatThanVienBien] Execute: caster={caster.name}, target={target.name}, buffValue={buffValues[CurrentLevelCongPhapInCombat(caster) - 1]}");
        target.AddDamageData(
            new List<long>
                { target.GetDameBuffForTarget() * buffValues[CurrentLevelCongPhapInCombat(caster) - 1] / 100 },
            true);
        RemoveCongPhap(caster);
    }

    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddCPDynamic(GetCongPhapMono(caster.congPhapEquip));
    }

    public virtual void ExecuteParams2(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (caster != null && GetCongPhapMono(caster.congPhapEquip) != null)
        {
            Execute2(caster, target, data);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Mỗi hiệp, lần đầu sử dụng thần thông tổn thương thì sát thương tăng lên {GetColoredText($"{buffValues[level - 1]}%", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams2);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThonNhatThanVienBien value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}