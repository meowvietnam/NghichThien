using CreateCharacter;
using Network;
using Network.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Skill
{
    [Serializable]
    public class CongPhapMono
    {
        public CongPhap congPhap;
        public SecureInt level;
    }

    public class CongPhap : BaseSkill, IStaticCombatEffect, IMemorySerializable<CongPhap>
    {
        [SerializeField] private int tocDoTuLuyenBase;
        [SerializeField] public bool isDonThuat = false;
        protected override string addressEffectString => "PrefabEffectSkillCongPhap";
        public override void SetBaseTimeLearn()
        {
            baseTimeLearn = 1702 * (int)eTierSkill;
        }

        public virtual void EquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
        {
        }

        public virtual void UnEquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
        {
        }

        public virtual void UpgradeCongPhapSuccess(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
        {
            UnEquipCongPhap(statsCurrent, daiCanhGioiCongPhap);
            EquipCongPhap(statsCurrent, daiCanhGioiCongPhap);
        }

        public virtual void LearnCongPhapSuccess(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
        {
        }

        protected void TieuHaoThoNguyen(CharacterData statsCurrent, int level, int[] tieuHao)
        {
            var stats = statsCurrent.Stats;
            var thoNguyen = stats.ThoNguyen;
            var require = tieuHao[level - 1];
            var newThoNguyen = thoNguyen - require;
            stats.SetThoNguyen(newThoNguyen);
        }

        public virtual void EquipCongPhap(CharacterData statsCurrent, int level)
        {
        }

        public virtual void UnEquipCongPhap(CharacterData statsCurrent, int level)
        {
        }

        public virtual void UpgradeCongPhapSuccess(CharacterData statsCurrent, int level)
        {
        }

        public virtual void LearnCongPhapSuccess(CharacterData statsCurrent, int level)
        {
        }

        public virtual int GetTocDoTuLuyen(int levelCP)
        {
            return levelCP * tocDoTuLuyenBase;
        }

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
                    x.congPhap.Id == this.Id && // So sánh kiểu chính xác của class thực tế
                    x.congPhap.eTypeSkill == this.eTypeSkill // Khớp loại kỹ năng
            );

            return equip;
        }

        protected virtual bool IsCanUse(CharacterCombat caster)
        {
            return GetCongPhapMono(caster.congPhapEquip) != null && GetCongPhapMono(caster.congPhapDynamic) != null;
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

        public virtual void InitPosCallExecute(CharacterCombat caster)
        {
        }


        public override string GetDesc()
        {
            return "";
        }


        public override int ByteSize()
        {
            var byteSize = base.ByteSize();
            return byteSize + tocDoTuLuyenBase.ByteSize() + isDonThuat.ByteSize();
        }

        public void Deserialize(IReader reader, ref CongPhap value)
        {
            BaseSkill baseValue = value;
            baseValue.Deserialize(reader, ref baseValue);
            value.tocDoTuLuyenBase = reader.Read<int>();
            value.isDonThuat = reader.Read<bool>();
        }

        public override void Serialize(IWriter writer)
        {
            base.Serialize(writer);
            writer.Write(tocDoTuLuyenBase);
            writer.Write(isDonThuat);
        }

        public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
        {
        }

        public override string GetDesc(int daiCanhGioi)
        {
            return string.Empty;
        }
    }
}