using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KinhDaoKiemQuyet), menuName = "Skill/Thần Thông/" + nameof(KinhDaoKiemQuyet))]
public class KinhDaoKiemQuyet : ThanThong, IMemorySerializable<KinhDaoKiemQuyet>
{
    [SerializeField]
    private int[] kinhDaoReset;
    [SerializeField]
    private int[] chiThuyReset;
    [SerializeField]
    private int kiemKhi = 1;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KinhDao, out SecureInt kinhDao);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChiThuy, out SecureInt chiThuy);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, kinhDao * kiemKhi);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KinhDao, kinhDao);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChiThuy, chiThuy);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KinhDao, kinhDaoReset[GetIndexLevelThanThong(caster)]);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChiThuy, chiThuyReset[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)}. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} thu hoạch được {GetColoredText($"+{kiemKhi}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}, Làm mới {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} thành {GetColoredText($"{kinhDaoReset[level - 1]}", "red")} tầng, Làm mới {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)} thành {GetColoredText($"{chiThuyReset[level - 1]}", "red")} tầng";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KinhDaoKiemQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}