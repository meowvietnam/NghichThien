using System;
using System.Collections.Generic;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using Network;
using Network.Extensions;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter.CharStats
{
    [Serializable]
    public class CharacterStats : IMemorySerializable<CharacterStats>
    {
        [field: SerializeField] public float Kim { get; set; }
        [field: SerializeField] public float Moc { get; set; }
        [field: SerializeField] public float Thuy { get; set; }
        [field: SerializeField] public float Hoa { get; set; }
        [field: SerializeField] public float Tho { get; set; }
        [field: SerializeField] public float Ma { get; set; }
        [field: SerializeField] public int TuChat { get; set; }
        [field: SerializeField] public int NgoTinh { get; set; }
        [field: SerializeField] public int ThanThuc { get; set; }
        [field: SerializeField] public int TocDoBay { get; set; }
        [field: SerializeField] public int LinhThach { get; set; }
        [field: SerializeField] public int Hp { get; set; }
        [field: SerializeField] public int SinhMenh { get; set; }
        [field: SerializeField] public int ThoNguyen { get; private set; }
        [field: SerializeField] public int TamCanh { get; set; }
        [field: SerializeField] public LinhCan LinhCan { get; private set; }
        [field: SerializeField] public Ability Ability { get; private set; }
        [field: SerializeField] public int DanDoc { get; private set; }
        [field: ShowInInspector] public TimeSpan TuoiTac { get; set; }
#if !SERVER
        private readonly Dictionary<StatType, Action<object>> onStatChanges = new();
#endif

        public CharacterStats()
        {
        }

        public CharacterStats(CharacterStats stats)
        {
            CopyFrom(stats);
        }

        public void CopyFrom(CharacterStats stats)
        {
            Kim = stats.Kim;
            Moc = stats.Moc;
            Thuy = stats.Thuy;
            Hoa = stats.Hoa;
            Tho = stats.Tho;
            Ma = stats.Ma;
            TuChat = stats.TuChat;
            NgoTinh = stats.NgoTinh;
            ThanThuc = stats.ThanThuc;
            TocDoBay = stats.TocDoBay;
            LinhThach = stats.LinhThach;
            Hp = stats.Hp;
            SinhMenh = stats.SinhMenh;
            ThoNguyen = stats.ThoNguyen;
            TamCanh = stats.TamCanh;
            LinhCan = stats.LinhCan;
            Ability = stats.Ability;
            DanDoc = stats.DanDoc;
            TuoiTac = stats.TuoiTac;
#if !SERVER
            foreach (var onStatChange in onStatChanges)
            {
                var value = GetValue(onStatChange.Key);
                onStatChange.Value(value);
            }
#endif
        }
#if !SERVER
        public void RegisterEvent(StatType stat, Action<object> action)
        {
            if (!onStatChanges.TryAdd(stat, action))
            {
                onStatChanges[stat] += action;
            }
        }

        private void Emit(StatType stat, object data)
        {
            if (onStatChanges.TryGetValue(stat, out var action))
            {
                action(data);
            }
        }
#endif

        public void Apply(StatModifier modifier)
        {
            Apply(modifier.Type, modifier.Action, modifier.Value);
        }

        public void Apply(StatType type, Action action, float value)
        {
            if (type == StatType.None) return;
            if (action == Action.Set)
            {
                SetValue(type, value);
            }
            else if (action == Action.Add)
            {
                var t = GetValue(type);
                SetValue(type, Convert.ToSingle(t) + value);
            }
            else if (action == Action.Or)
            {
                var t = GetValue(type);
                var i = Convert.ToInt32(t);
                var newValue = i | (int)(value);
                SetValue(type, newValue);
            }
        }

        public object GetValue(StatType type)
        {
            switch (type)
            {
                case StatType.Kim: return Kim;
                case StatType.Moc: return Moc;
                case StatType.Thuy: return Thuy;
                case StatType.Hoa: return Hoa;
                case StatType.Tho: return Tho;
                case StatType.Ma: return Ma;
                case StatType.TuChat: return TuChat;
                case StatType.NgoTinh: return NgoTinh;
                case StatType.ThanThuc: return ThanThuc;
                case StatType.TocDoBay: return TocDoBay;
                case StatType.LinhThach: return LinhThach;
                case StatType.Hp: return Hp;
                case StatType.SinhMenh: return SinhMenh;
                case StatType.ThoNguyen: return ThoNguyen;
                case StatType.TamCanh: return TamCanh;
                case StatType.LinhCan: return LinhCan;
                case StatType.Ability: return Ability;
                case StatType.DanDoc: return DanDoc;
                case StatType.TuoiTac: return TuoiTac;
                default: return null;
            }
        }

        private void SetFloat(StatType type, float value)
        {
            switch (type)
            {
                case StatType.Kim: Kim = value; break;
                case StatType.Moc: Moc = value; break;
                case StatType.Thuy: Thuy = value; break;
                case StatType.Hoa: Hoa = value; break;
                case StatType.Tho: Tho = value; break;
                case StatType.Ma: Ma = value; break;
            }
#if !SERVER
            Emit(type, value);
#endif
        }

        private void SetInt(StatType type, int value)
        {
            switch (type)
            {
                case StatType.TuChat: TuChat = value; break;
                case StatType.NgoTinh: NgoTinh = value; break;
                case StatType.ThanThuc: ThanThuc = value; break;
                case StatType.TocDoBay: TocDoBay = value; break;
                case StatType.LinhThach: LinhThach = value; break;
                case StatType.Hp: Hp = value; break;
                case StatType.SinhMenh: SinhMenh = value; break;
                case StatType.ThoNguyen: ThoNguyen = value; break;
                case StatType.TamCanh: TamCanh = value; break;
                case StatType.DanDoc: DanDoc = value; break;
                case StatType.TuoiTac: TuoiTac = new TimeSpan(value); break;
            }
#if !SERVER
            Emit(type, value);
#endif
        }

        public void SetValue(StatType type, float value)
        {
            switch (type)
            {
                case StatType.Kim:
                case StatType.Moc:
                case StatType.Thuy:
                case StatType.Hoa:
                case StatType.Tho:
                case StatType.Ma:
                    SetFloat(type, value);
                    break;
                case StatType.TuChat:
                case StatType.NgoTinh:
                case StatType.ThanThuc:
                case StatType.TocDoBay:
                case StatType.LinhThach:
                case StatType.Hp:
                case StatType.SinhMenh:
                case StatType.ThoNguyen:
                case StatType.TamCanh:
                case StatType.DanDoc:
                case StatType.TuoiTac:
                    SetInt(type, (int)value);
                    break;
                case StatType.LinhCan:
                    LinhCan = (LinhCan)(int)value;
#if !SERVER
                    Emit(type, LinhCan);
#endif
                    break;
                case StatType.Ability:
                    Ability = (Ability)(int)value;
#if !SERVER
                    Emit(type, Ability);
#endif
                    break;
            }
        }

        public void ApplyFrom(StatsSnapshot snapshot)
        {
            Kim = snapshot.Kim;
            Moc = snapshot.Moc;
            Thuy = snapshot.Thuy;
            Hoa = snapshot.Hoa;
            Tho = snapshot.Tho;
            Ma = snapshot.Ma;
            TuChat = snapshot.TuChat;
            NgoTinh = snapshot.NgoTinh;
            ThanThuc = snapshot.ThanThuc;
            TocDoBay = snapshot.TocDoBay;
            LinhThach = snapshot.LinhThach;
            Hp = snapshot.Hp;
            SinhMenh = snapshot.SinhMenh;
            ThoNguyen = snapshot.ThoNguyen;
            TamCanh = snapshot.TamCanh;
            DanDoc = snapshot.DanDoc;
            Observer.Notify(EventList.GamePlay.UPDATE_PLAYER_STATS);

        }

        public int ByteSize()
        {
            return Kim.ByteSize() + Moc.ByteSize() + Thuy.ByteSize() + Hoa.ByteSize()
                   + Tho.ByteSize() + Ma.ByteSize() + TuChat.ByteSize() + NgoTinh.ByteSize()
                   + ThanThuc.ByteSize() + TocDoBay.ByteSize() + LinhThach.ByteSize()
                   + Hp.ByteSize()
                   + SinhMenh.ByteSize()
                   + ThoNguyen.ByteSize() + TamCanh.ByteSize() + LinhCan.ByteSize() + Ability.ByteSize()
                   + DanDoc.ByteSize() + TuoiTac.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Kim);
            writer.Write(Moc);
            writer.Write(Thuy);
            writer.Write(Hoa);
            writer.Write(Tho);
            writer.Write(Ma);
            writer.Write(TuChat);
            writer.Write(NgoTinh);
            writer.Write(ThanThuc);
            writer.Write(TocDoBay);
            writer.Write(LinhThach);
            writer.Write(Hp);
            writer.Write(SinhMenh);
            writer.Write(ThoNguyen);
            writer.Write(TamCanh);
            writer.Write(LinhCan);
            writer.Write(Ability);
            writer.Write(DanDoc);
            writer.Write(TuoiTac);
        }

        public void Deserialize(IReader reader, ref CharacterStats value)
        {
            value.Kim = reader.Read<float>();
            value.Moc = reader.Read<float>();
            value.Thuy = reader.Read<float>();
            value.Hoa = reader.Read<float>();
            value.Tho = reader.Read<float>();
            value.Ma = reader.Read<float>();
            value.TuChat = reader.Read<int>();
            value.NgoTinh = reader.Read<int>();
            value.ThanThuc = reader.Read<int>();
            value.TocDoBay = reader.Read<int>();
            value.LinhThach = reader.Read<int>();
            value.Hp = reader.Read<int>();
            value.SinhMenh = reader.Read<int>();
            value.ThoNguyen = reader.Read<int>();
            value.TamCanh = reader.Read<int>();
            value.LinhCan = reader.Read<LinhCan>();
            value.Ability = reader.Read<Ability>();
            value.DanDoc = reader.Read<int>();
            value.TuoiTac = reader.Read<TimeSpan>();
        }

        public static int GetMinTamCanh()
        {
            return -100;
        }

        public void SetTamCanh(int tamCanh)
        {
            tamCanh = Math.Max(GetMinTamCanh(), tamCanh);
            TamCanh = tamCanh;
        }

        public int GetMinThoNguyen()
        {
            return 0;
        }

        public void SetThoNguyen(int thoNguyen)
        {
            thoNguyen = Math.Max(GetMinThoNguyen(), thoNguyen);
            ThoNguyen = thoNguyen;
        }
    }
}