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
    protected override ETypeItem tierItem
    {
        get => ETypeItem.ThanThong;
    }
    public abstract override void Execute(CharacterCombat caster, CharacterCombat target);


    public abstract override string GetDesc();
    
    public virtual bool IsCanUse(CharacterCombat caster , CharacterCombat target)
    {
        return true;
    }    
   
}
