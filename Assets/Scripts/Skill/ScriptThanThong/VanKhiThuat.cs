using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(VanKhiThuat), menuName = "Skill/Thần Thông/" + nameof(VanKhiThuat))]
public class VanKhiThuat : ThanThong, IMemorySerializable<VanKhiThuat>
{
    [SerializeField]
    private int hapThuSO = 3;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int hapThu = this.hapThuSO;
        var equippedItem = caster?.congPhapEquip?.FirstOrDefault(x => x.congPhap is TuDuongHonNguyenKinh);
        if (equippedItem != null)
        {
            TuDuongHonNguyenKinh tuDuongHonNguyenKinh = equippedItem.congPhap as TuDuongHonNguyenKinh;
            if (CombatStatic.IsActiveWithPercent(tuDuongHonNguyenKinh.GetPercent(caster)))
            {
                hapThu *= 2;
            }
        }

        HauKeVoLuc(caster);
        caster.HapThuLinhKhi(hapThu);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ {GetColoredText($"{hapThuSO}", "red")} điểm linh khí";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref VanKhiThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}