using System;
using UnityEngine;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TinhViThanDietQuyet), menuName = "Skill/Thần Thông/" + nameof(TinhViThanDietQuyet))]
public class TinhViThanDietQuyet : ThanNiemQuyet, IMemorySerializable<TinhViThanDietQuyet>
{
    [SerializeField]
    private float cameraSize = 5;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThanThuong, out SecureInt thanThuong);
        int damebuff = thanThuong * buffValues[GetIndexLevelThanThong(caster)];
        Debug.Log("ddamebuff " + damebuff);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{damebuff}, eTypeSkill);
        target.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ThanThuong, thanThuong);
    }

    protected override void ShowEffect(CharacterCombat caster, CharacterCombat target)
    {
        PlayVisualSkill(caster, target);
    }
    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectParticleBase(caster, delayTimeEffect, cameraSize);
    }
    protected override (Action<Transform, Transform>, Action<Transform, Transform>) GetActionControlEffectParticleBase()
    {
        // nếu caster ng nhận effec là enemy thì 
        return ((childEffect, childPrefab) =>
        {
            base.GetActionControlEffectParticleBase().Item1?.Invoke(childEffect, childPrefab);
            childEffect.transform.localEulerAngles = new Vector3(childPrefab.transform.localEulerAngles.x, childPrefab.transform.localEulerAngles.y * -1, childEffect.transform.localEulerAngles.y);
        }, (childEffect, childPrefab) =>
        {
            base.GetActionControlEffectParticleBase().Item2?.Invoke(childEffect, childPrefab);
            childEffect.transform.localEulerAngles = new Vector3(childPrefab.transform.localEulerAngles.x, childPrefab.transform.localEulerAngles.y, childEffect.transform.localEulerAngles.y);
        });
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnNac, out SecureInt anNac);
        if (anNac > 0 && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return $"Bản thân có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnNac)} mới có thể sử dụng. Gỡ ra toàn bộ {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThanThuong)} của đối thủ mỗi một tầng thì tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương. " + base.GetDesc(level);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TinhViThanDietQuyet value)
    {
        ThanNiemQuyet baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}