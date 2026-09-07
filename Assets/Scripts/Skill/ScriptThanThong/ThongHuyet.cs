using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThongHuyet), menuName = "Skill/Thần Thông/" + nameof(ThongHuyet))]
public class ThongHuyet : ThanThong, IMemorySerializable<ThongHuyet>
{
    [SerializeField] protected int[] buff;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.NoiThuongAction();
        ExecuteAction(caster, target);
    }

    protected virtual void ExecuteAction(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, buff[GetIndexLevelThanThong(caster)]);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        SecureInt noiThuong = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NoiThuong, out noiThuong);
        return noiThuong > 0 && base.IsCanUse(caster, target);
    }

    protected virtual string TextThuHoach()
    {
        return GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu);
    }

    public override string GetDesc(int level)
    {
        return
            $"Bản thân phải có {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NoiThuong)} thì phát động hiệu ứng này, thu hoạch được {GetColoredText($"{buff[level - 1]}", "red")} tầng {TextThuHoach()}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThongHuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}