using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(LuongNghi), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(LuongNghi))]
public class LuongNghi : NgoDaoCombat, IMemorySerializable<LuongNghi>
{
    [SerializeField]
    private int buff = 3;
    public override void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        if (target == null)
            return;
        if (target.ngoDaoCombats.Any(x => x.Id == this.Id))
        {
            ExecuteNgoDao(caster, target, data);
        }
    }

    // target ng nhận nếu có ngộ đạo này check nếu ng nhận dame có trận pháp số lương ng nhận dame đc giảm dame
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = target.specialThanThongImpactAfterPlaySkill.Count(x => x is TranPhapTT || x.GetEKeywordSkillHave().Contains(BaseSkill.EKeyWord.TranPhap));
        target.AddDamageData(new List<int>{-target.GetDameBuffForTarget() * count * 3 / 100});
    }

    public override string GetDes()
    {
        return $"Mỗi có một cái {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.TranPhap)} nhận tổn thương giảm xuống {BaseSkill.GetColoredText($"{buff}%", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LuongNghi value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}