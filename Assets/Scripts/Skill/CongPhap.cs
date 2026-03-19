using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CongPhapMono
{
    public CongPhap congPhap;
    public SecureInt level;
}
public abstract class CongPhap : BaseSkill
{
    [SerializeField] private int tocDoTuLuyenBase;

    public virtual void EquipCongPhap(CharacterStats statsCurrent , int daiCanhGioiCongPhap)
    {

    }
    public virtual void UnEquipCongPhap(CharacterStats statsCurrent, int daiCanhGioiCongPhap)
    {

    }
    public virtual void UpgradeCongPhapSuccess(CharacterStats statsCurrent, int daiCanhGioiCongPhap)
    {
        UnEquipCongPhap(statsCurrent, daiCanhGioiCongPhap);
        EquipCongPhap(statsCurrent, daiCanhGioiCongPhap);

    }
    public virtual void LearnCongPhapSuccess(CharacterStats statsCurrent, int daiCanhGioiCongPhap)
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
       caster.RemoveCPDynamic(this);
       

    }
    public virtual int CurrentLevelCongPhapInCombat(CharacterCombat caster)
    {
        // 1. Tìm trong danh sách trang bị của caster
        var equip = GetCongPhapMono(caster.congPhapEquip);

        // 2. Kiểm tra log từng bước
        if (equip == null)
        {
            Debug.LogError("DEBUG: 'equip' bị NULL - Không tìm thấy Công Pháp tương ứng!");
        }
        else if (equip.level == null)
        {
            Debug.LogError("DEBUG: 'equip.level' bị NULL - Đối tượng level chưa được khởi tạo!");
        }
        else
        {
            Debug.Log($"DEBUG: Tìm thấy equip, Level hiện tại là: {equip.level.Value}");
        }

        // 3. Trả về kết quả an toàn
        if (equip != null && equip.level != null && equip.level.Value != 0)
        {
            return equip.level.Value; // Trả về giá trị cụ thể
        }

        return 1; // Giá trị mặc định nếu có bất kỳ thứ gì null
    }

    public abstract void InitPosCallExecute(CharacterCombat caster);

    public abstract override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data);

    public override abstract string GetDesc(int level);

    public override string GetDesc()
    {
        return "";
    }



}
