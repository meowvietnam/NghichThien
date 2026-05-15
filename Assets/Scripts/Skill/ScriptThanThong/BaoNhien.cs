using Skill;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(BaoNhien), menuName = "Skill/Thần Thông/" + nameof(BaoNhien))]
public class BaoNhien : ThanThong, BaseSkill.LienKich, IMemorySerializable<BaoNhien>
{
    [SerializeField]
    private int[] dameValues;
    [SerializeField]
    private float delayTimeEffect = 1f;
    private void ExcuteDiem(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Diem, out SecureInt diem);
        target.AddDamageData(new System.Collections.Generic.List<int>{diem});
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dameValues[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
        Debug.Log("BaoNhien before" + target.allDamages[0]);
        ExcuteDiem(caster, target);
        Debug.Log("BaoNhien after" + target.allDamages[0]);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        ExcuteDiem(caster, target);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương. Đồng thời phát động thêm 1 lần {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)}. {GetKeyWord(EKeyWord.LienKich)}, nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Moc)} hệ thần thông thì phát động thêm 1 lần {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref BaoNhien value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}