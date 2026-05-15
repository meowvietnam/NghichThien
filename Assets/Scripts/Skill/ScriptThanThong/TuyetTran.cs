using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TuyetTran), menuName = "Skill/Thần Thông/" + nameof(TuyetTran))]
public class TuyetTran : ThanThong, BaseSkill.LienKich, IMemorySerializable<TuyetTran>
{
    [SerializeField]
    protected float delayTimeEffect = 1f;
    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        int dame = damebuffs[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill, true);
    }

    [SerializeField]
    private int[] dames = new int[]{7, 11, 20, 40, 80};
    [SerializeField]
    private int[] damebuffs = new int[]{2, 4, 6, 10, 20};
    [SerializeField]
    private int[] requires = new int[]{3, 6, 12, 24, 48};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int require = requires[GetIndexLevelThanThong(caster)];
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Tran, require);
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out SecureInt tran);
        if (tran >= requires[GetIndexLevelThanThong(caster)] && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return $"Tiêu hao {GetColoredText($"{requires[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}. Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Hoa)} hệ thần thông thì tăng lên {GetColoredText($"+{damebuffs[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TuyetTran value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}