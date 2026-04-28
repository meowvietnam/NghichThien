using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class EquipmentManager : IMemorySerializable<EquipmentManager>
    {
        [field: ShowInInspector]
        public HashSet<ThanThongEquip> ThanThongEquips { get; set; } = new();
        [field: ShowInInspector]
        public HashSet<DanCombatUsed> DanCombatUseds { get; set; } = new();
        [field: SerializeField ]
        public ushort DonThuatUsed { get; set; }
        [field: SerializeField]
        public ushort PhuTu1Used { get; set; }
        [field: SerializeField]
        public ushort PhuTu2Used { get; set; }
        [field: SerializeField]
        public ushort PhuTu3Used { get; set; }
        [field: SerializeField]
        public ushort PhuTu4Used { get; set; }
        [field: SerializeField]
        public ushort ChuTu1Used { get; set; }
        [field: SerializeField]
        public ushort ChuTu2Used { get; set; }

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
        }

        public int ByteSize()
        {
            return ThanThongEquips.ByteSize() + DanCombatUseds.ByteSize() + DonThuatUsed.ByteSize() + PhuTu1Used.ByteSize() 
                + PhuTu2Used.ByteSize() + PhuTu3Used.ByteSize() + PhuTu4Used.ByteSize()
                + ChuTu1Used.ByteSize() + ChuTu2Used.ByteSize();
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
        }
    }
}