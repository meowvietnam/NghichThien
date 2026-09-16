using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(PhuTranLuyenTamQuyet))]
public class PhuTranLuyenTamQuyet : BangTamQuyet, IMemorySerializable<PhuTranLuyenTamQuyet>
{
    [SerializeField] private int[] buffValues2 = new int[] { 9, 13, 17, 21, 25 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NhieuHon, out SecureInt nhieuHon) &&
            CheckIsEqualETypeSkill(caster))
        {
            target.AddDamageData(new List<long>
                { target.GetDameBuffForTarget() * buffValues2[CurrentLevelCongPhapInCombat(caster) - 1] / 100 });
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"{base.GetDesc(level)}. Nếu đối thủ ở trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NhieuHon)} thì sử dụng {GetTextTypeSkill(ETypeSkill.Than)} hệ thần thông tổn thương tăng lên {GetColoredText($"{buffValues2[level - 1]}%", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + buffValues2.ByteSize();
    }

    public void Deserialize(IReader reader, ref PhuTranLuyenTamQuyet value)
    {
        BangTamQuyet baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
        value.buffValues2 = reader.ReadArray<Int32>();
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.WriteArray(buffValues2);
    }
}