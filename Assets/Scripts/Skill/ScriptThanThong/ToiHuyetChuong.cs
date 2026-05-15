using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ToiHuyetChuong), menuName = "Skill/Thần Thông/" + nameof(ToiHuyetChuong))]
public class ToiHuyetChuong : ThanThong, BaseSkill.TuongSinh, IMemorySerializable<ToiHuyetChuong>
{
    [SerializeField]
    private int[] dames = new int[]{11, 15, 25, 45, 90};
    [SerializeField]
    private int[] buffValues = new int[]{3, 4, 6, 7, 9};
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        Action(caster, target, dames);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    private void Action(CharacterCombat caster, CharacterCombat target, int[] dames)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
        if (caster.listAmKinhTT.Count > 0)
        {
            caster.Healing(dame);
        }
    }

    public void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        Action(caster, target, buffValues);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Nếu bản thân đang có {GetKeyWord(EKeyWord.AmKinh)} thì hồi một lượng HP bằng sát thương gây ra. {GetKeyWord(EKeyWord.TuongSinh)} thì tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ToiHuyetChuong value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}