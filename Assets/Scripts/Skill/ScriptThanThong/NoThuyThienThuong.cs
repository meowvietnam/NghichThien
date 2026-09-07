using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(NoThuyThienThuong), menuName = "Skill/Thần Thông/" + nameof(NoThuyThienThuong))]
public class NoThuyThienThuong : ThanThong, IMemorySerializable<NoThuyThienThuong>
{
    [SerializeField]
    private int[] chiThuyReset;
    [SerializeField]
    private int chanKiem = 1;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KinhDao, out SecureInt kinhDao);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChiThuy, out SecureInt chiThuy);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKiem, kinhDao * chanKiem);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KinhDao, kinhDao);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChiThuy, chiThuy);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChiThuy, chiThuyReset[GetIndexLevelThanThong(caster)]);
        PlayVisualSkill(caster, target);
    }
    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)}. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} thu hoạch được {GetColoredText($"+{chanKiem}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanKiem)}, Làm mới {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)} thành {GetColoredText($"{chiThuyReset[level - 1]}", "red")} tầng";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NoThuyThienThuong value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}