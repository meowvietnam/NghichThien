using System.Linq;
using Skill;
using UnityEngine;
using static BaseSkill;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(NhanDiaThienKiem), menuName = "Skill/Thần Thông/" + nameof(NhanDiaThienKiem))]
public class NhanDiaThienKiem : ThanThong, BaseSkill.TuongKhac, IMemorySerializable<NhanDiaThienKiem>
{
    [SerializeField] private int[] dames;
    [SerializeField] private int[] damesCaster;
    [SerializeField] private int[] buffValues;
    [SerializeField] private int[] buffValueKiemKhis;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        int dame = dames[GetIndexLevelThanThong(caster)];
        caster.AddBuffAndDebuffCount(GetTypeKiem(), buffValues[GetIndexLevelThanThong(caster)]);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public void ExecuteTuongKhac(CharacterCombat caster, CharacterCombat target)
    {
        int dame = damesCaster[GetIndexLevelThanThong(caster)];
        caster.TakeDameByStatic(null, new System.Collections.Generic.List<int> { dame }, false, null, false);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, buffValueKiemKhis[GetIndexLevelThanThong(caster)]);
    }

    public EBuffAndDeBuff GetTypeKiem()
    {
        var listKeyBuff = GetEKeywordBuffSkillHave(1);
        if (listKeyBuff.Contains(EBuffAndDeBuff.NhanKiem))
        {
            return EBuffAndDeBuff.NhanKiem;
        }

        if (listKeyBuff.Contains(EBuffAndDeBuff.DiaKiem))
        {
            return EBuffAndDeBuff.DiaKiem;
        }

        if (listKeyBuff.Contains(EBuffAndDeBuff.ThienKiem))
        {
            return EBuffAndDeBuff.ThienKiem;
        }

        return EBuffAndDeBuff.None;
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.KiemY)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(GetTypeKiem())}. {GetKeyWord(EKeyWord.TuongKhac)} Nhận {GetColoredText($"+{damesCaster[level - 1]}", "red")} điểm sát thương, thu hoạch được {GetColoredText($"+{buffValueKiemKhis[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NhanDiaThienKiem value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}