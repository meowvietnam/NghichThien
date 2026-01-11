using DG.Tweening;
using LongDH.Script.ObjectPool;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public abstract class ThanThong : BaseSkill
{
    [System.Serializable]
    public class RequireActive 
    {
        public ELinhKhi linhKhiRequire;
        public int count;
    }
    public List<RequireActive> requireActives;
    public ELinhKhi[] LinhKhiTypes =>
    requireActives
        .Where(r => r.linhKhiRequire != ELinhKhi.None)
        .Select(r => r.linhKhiRequire)
        .ToArray();
    protected override ETypeItem typeItem
    {
        get => ETypeItem.ThanThong;
    }
    [SerializeField] protected GameObject prefabEffectSkill;
    public abstract override void Execute(CharacterCombat caster, CharacterCombat target);


    public abstract override string GetDesc();
    
    public virtual bool IsCanUse(CharacterCombat caster , CharacterCombat target)
    {
        return true;
    }    
    protected virtual void ShowSkillEffect(CharacterCombat target , float delay , Vector3 addPos = default)
    {
        if (prefabEffectSkill == null) return;
        var posCenter = Vector3.up * 500;
        posCenter += addPos;
        var effect = prefabEffectSkill.Spawn(target.transform);
        effect.transform.localPosition = posCenter;
        effect.transform.localScale = prefabEffectSkill.transform.localScale;
        DOVirtual.DelayedCall(delay, () =>
        {
            effect.gameObject.Recycle();
        });

    }
    protected virtual void ShowSkillEffectParticle(CharacterCombat target, float delay, Vector3 addPos = default)
    {
        if (prefabEffectSkill == null) return;
        var posCenter = Vector3.up * 500;
        posCenter += addPos;
        var effect = prefabEffectSkill.Spawn(target.transform);
        effect.transform.localPosition = posCenter;
        DOVirtual.DelayedCall(delay, () =>
        {
            effect.gameObject.Recycle();
        });

    }
}
