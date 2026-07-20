using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(CuuUThu), menuName = "Skill/Thần Thông/" + nameof(CuuUThu))]
public class CuuUThu : ThanThong, IMemorySerializable<CuuUThu>
{
    [SerializeField]
    private int[] dames = new int[]{20, 28, 45, 80, 150};
    [SerializeField]
    private int[] dames2 = new int[]{1, 1, 1, 1, 1};
    [SerializeField]
    private int tieuHao = 30;
    [SerializeField]
    private int percent = 90;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.characterStatsBase.thoNguyen -= tieuHao;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Anh, out SecureInt anh);
        int dameHP = (int)(percent * caster.characterStatsTemp.hp / 100f);
        int dame = dames[GetIndexLevelThanThong(caster)] + anh * dames2[GetIndexLevelThanThong(caster)] + dameHP;
        caster.TakeDameByStatic(null, new System.Collections.Generic.List<int>{dameHP}, false, null, false);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Anh, anh);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (caster.characterStatsBase.thoNguyen >= tieuHao && !caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong is CuuUThu)) && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return $"Tiêu hao {GetColoredText($"-{tieuHao} năm thọ nguyên", "red")}, tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Chuyển hóa {GetColoredText($"{percent}%", "red")} HP hiện tại thành điểm sát thương. Đồng thời, Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Anh)} mỗi một tầng tăng lên {GetColoredText($"+{dames2[level - 1]}", "red")} điểm sát thương {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref CuuUThu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}