using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Network;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class EquipmentManager : IMemorySerializable<EquipmentManager>
    {
        [field: ShowInInspector] public HashSet<ThanThongEquip> ThanThongEquips { get; set; } = new();
        [field: ShowInInspector] public HashSet<DanCombatUsed> DanCombatUseds { get; set; } = new();
        [field: SerializeField] public ushort DonThuatUsed { get; set; } = ushort.MaxValue;
        [field: SerializeField] public ushort PhuTu1Used { get; set; } = ushort.MaxValue;
        [field: SerializeField] public ushort PhuTu2Used { get; set; } = ushort.MaxValue;
        [field: SerializeField] public ushort PhuTu3Used { get; set; } = ushort.MaxValue;
        [field: SerializeField] public ushort PhuTu4Used { get; set; } = ushort.MaxValue;
        [field: SerializeField] public ushort ChuTu1Used { get; set; } = ushort.MaxValue;
        [field: SerializeField] public ushort ChuTu2Used { get; set; } = ushort.MaxValue;
        [field: SerializeField] public ushort VuKhi { get; set; } = ushort.MaxValue;
        [field: SerializeField] public ushort PhongCu { get; set; } = ushort.MaxValue;
        [field: SerializeField] public ushort TrangSuc { get; set; } = ushort.MaxValue;

        public ushort GetSlotCP(ETypeSlotCP slot)
        {
            switch (slot)
            {
                case ETypeSlotCP.ChuTu1:
                    return ChuTu1Used;
                case ETypeSlotCP.ChuTu2:
                    return ChuTu2Used;
                case ETypeSlotCP.DonThuat:
                    return DonThuatUsed;
                case ETypeSlotCP.PhuTu1:
                    return PhuTu1Used;
                case ETypeSlotCP.PhuTu2:
                    return PhuTu2Used;
                case ETypeSlotCP.PhuTu3:
                    return PhuTu3Used;
                case ETypeSlotCP.PhuTu4:
                    return PhuTu4Used;
                default:
                    return 0;
            }
        }

        public void SetSlotCP(ETypeSlotCP slot, ushort value)
        {
            switch (slot)
            {
                case ETypeSlotCP.ChuTu1:
                    ChuTu1Used = value;
                    break;
                case ETypeSlotCP.ChuTu2:
                    ChuTu2Used = value;
                    break;
                case ETypeSlotCP.DonThuat:
                    DonThuatUsed = value;
                    break;
                case ETypeSlotCP.PhuTu1:
                    PhuTu1Used = value;
                    break;
                case ETypeSlotCP.PhuTu2:
                    PhuTu2Used = value;
                    break;
                case ETypeSlotCP.PhuTu3:
                    PhuTu3Used = value;
                    break;
                case ETypeSlotCP.PhuTu4:
                    PhuTu4Used = value;
                    break;
                default:
                    break;
            }
        }

        public void Deserialize(IReader reader, ref EquipmentManager value)
        {
            value.ThanThongEquips = reader.ReadArray<ThanThongEquip>().ToHashSet();
            value.DanCombatUseds = reader.ReadArray<DanCombatUsed>().ToHashSet();
            value.DonThuatUsed = reader.Read<ushort>();
            value.PhuTu1Used = reader.Read<ushort>();
            value.PhuTu2Used = reader.Read<ushort>();
            value.PhuTu3Used = reader.Read<ushort>();
            value.PhuTu4Used = reader.Read<ushort>();
            value.ChuTu1Used = reader.Read<ushort>();
            value.ChuTu2Used = reader.Read<ushort>();
            value.VuKhi = reader.Read<ushort>();
            value.PhongCu = reader.Read<ushort>();
            value.TrangSuc = reader.Read<ushort>();
        }

        public int ByteSize()
        {
            return ThanThongEquips.ByteSize() + DanCombatUseds.ByteSize() + DonThuatUsed.ByteSize() +
                   PhuTu1Used.ByteSize()
                   + PhuTu2Used.ByteSize() + PhuTu3Used.ByteSize() + PhuTu4Used.ByteSize()
                   + ChuTu1Used.ByteSize() + ChuTu2Used.ByteSize()
                   + VuKhi.ByteSize() + PhongCu.ByteSize() + TrangSuc.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.WriteArray(ThanThongEquips);
            writer.WriteArray(DanCombatUseds);
            writer.Write(DonThuatUsed);
            writer.Write(PhuTu1Used);
            writer.Write(PhuTu2Used);
            writer.Write(PhuTu3Used);
            writer.Write(PhuTu4Used);
            writer.Write(ChuTu1Used);
            writer.Write(ChuTu2Used);
            writer.Write(VuKhi);
            writer.Write(PhongCu);
            writer.Write(TrangSuc);
        }

        public void UnEquipThanThong(ushort id)
        {
            var old = ThanThongEquips.FirstOrDefault(x => x.Id == id);


            ThanThongEquips.Remove(old);
        }

        public ThanThongEquip EquipThanThong(ThanThongEquip thanThong)
        {
            var old = ThanThongEquips.FirstOrDefault(x => x.SlotIndex == thanThong.SlotIndex);
            if (old != null)
            {
                ThanThongEquips.Remove(old);
            }

            ThanThongEquips.Add(thanThong);
            return old;
        }

        public void EquipEquipment(EquipEquipmentResult result)
        {
            switch (result.TypeEquipment)
            {
                case LuyenKhi.EquipmentType.Weapon:
                    VuKhi = result.Id;
                    break;
                case LuyenKhi.EquipmentType.Armor:
                    PhongCu = result.Id;
                    break;
                case LuyenKhi.EquipmentType.Jewelry:
                    TrangSuc = result.Id;
                    break;
                default:
                    break;
            }
        }

        public void ClearDanCombat()
        {
            DanCombatUseds.Clear();
        }
    }
}