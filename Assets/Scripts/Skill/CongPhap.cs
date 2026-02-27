using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CongPhapMono
{
    public CongPhap congPhap;
    public int level = 1;
}
public abstract class CongPhap : BaseSkill
{
    [SerializeField] private int tocDoTuLuyenBase;

    public virtual void EquipCongPhap(CharacterStats stats, int level)
    {

    }
    public virtual void UnEquipCongPhap(CharacterStats stats, int level)
    {

    }
    public virtual int GetTocDoTuLuyen(CharacterCombat caster)
    {
        return CurrentLevelCongPhapInCombat(caster) * tocDoTuLuyenBase;

    }
    protected override ETypeItem typeItem => ETypeItem.CongPhap;
    public virtual void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (caster != null && IsCanUse(caster))
        {
            Execute(caster, target, data);
        }
    }
    public CongPhapMono GetCongPhapMono(List<CongPhapMono> congPhapMonos)
    {
        var equip = congPhapMonos.FirstOrDefault(x =>
           x.congPhap != null &&
           x.congPhap.GetType() == this.GetType() && // So sánh kiểu chính xác của class thực tế
           x.congPhap.eTypeSkill == this.eTypeSkill  // Khớp loại kỹ năng
       );

        return equip;

    }
    protected bool IsCanUse(CharacterCombat caster)
    {
        return GetCongPhapMono(caster.congPhapEquip) != null &&  GetCongPhapMono(caster.congPhapDynamic) != null;
    }    
    protected virtual void RemoveCongPhap(CharacterCombat caster)
    {
        caster.congPhapDynamic.RemoveAll(x =>
            x.congPhap.GetType() == this.GetType()
        );
    }    
    public virtual int CurrentLevelCongPhapInCombat(CharacterCombat caster)
    {
        // Tìm trong danh sách trang bị của caster
        var equip = GetCongPhapMono(caster.congPhapEquip);

        // Trả về level nếu tìm thấy, ngược lại trả về 0 (hoặc 1 tùy logic của bạn)
        return equip != null ? equip.level : 1;
    }

    public abstract void InitPosCallExecute(CharacterCombat caster);

    public abstract override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data);

    public override abstract string GetDesc(int level);

    public override string GetDesc()
    {
        return "";
    }



}
