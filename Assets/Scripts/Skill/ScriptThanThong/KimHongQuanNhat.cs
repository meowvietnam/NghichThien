using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KimHongQuanNhat), menuName = "Skill/Thần Thông/" + nameof(KimHongQuanNhat))]
public class KimHongQuanNhat : ThanThong, IMemorySerializable<KimHongQuanNhat>
{
    [SerializeField] private int[] dame;
    [SerializeField] private int[] perDameKiemKhi;
    [SerializeField] private int[] perDameSucThe;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KietSuc, 1);
        SecureInt sucThe = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out sucThe);
        SecureInt kiemKhi = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out kiemKhi);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.SucThe, sucThe);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, kiemKhi);
        int dame = this.dame[GetIndexLevelThanThong(caster)] +
                   perDameKiemKhi[GetIndexLevelThanThong(caster)] * kiemKhi +
                   perDameSucThe[GetIndexLevelThanThong(caster)] * sucThe;
        target.CaculateDameByThanThong(caster, new List<int> { dame }, eTypeSkill);

        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectParticleBase(target, 1f);
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tạo thành {GetColoredText($"{dame[level - 1]}", "red")} điểm sát thương, cũng tiêu hao toàn bộ {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} mỗi tầng tăng thêm {GetColoredText($"{perDameKiemKhi[level - 1]}", "red")} điểm thương tổn và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)} mỗi tầng tăng thêm {GetColoredText($"{perDameSucThe[level - 1]}", "red")} điểm thương tổn. Bạn bị {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KietSuc)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KimHongQuanNhat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}