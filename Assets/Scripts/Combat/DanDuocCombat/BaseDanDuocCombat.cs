using LuyenDan;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

public abstract class BaseDanDuocCombat : BaseData , IStaticCombatEffect
{
    public abstract void InitPosCallExecute(CharacterCombat caster);
    public abstract void ExecuteDanDuoc(CharacterCombat caster, CharacterCombat target, params object[] data);


    public virtual void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        if (caster == null) return;
        if (IsCanUse(caster,target))
        {
            ExecuteDanDuoc(caster, target, data);

        }


    }
    protected CharacterCombat.CountStack<BaseDanDuocCombat> GetDanDuocCombat(CharacterCombat caster)
    {
        return caster.danDuocCombats.FirstOrDefault(x=> IsMatching(x.type));
    }    
    public virtual bool IsCanUse(CharacterCombat caster , CharacterCombat target)
    {
        return caster.danDuocCombats.Any(x => IsMatching(x.type));
      
    }
    // Hàm kiểm tra mặc định (có thể override ở lớp con)
    protected virtual bool IsMatching(BaseDanDuocCombat other)
    {
        if (other == null) return false;
        return other.Id == this.Id;
    }
    [Button]  
    private void SetSprite()
    {
        var LinhDan = LuyenDanResources.Instance.FindLinhDan(Id);
        spriteItem = LinhDan.Icon;
        nameItem = LinhDan.Name;
    }
    [Button]
    public void SetIDLinhDan()
    {
        var LinhDan = LuyenDanResources.Instance.FindLinhDanWithName(nameItem);
        Id = LinhDan.Id;
    }
}
