using System;
using System.Collections.Generic;
using Network;
using Network.Extensions;
using UnityEngine;

namespace  CreateCharacter.CharStats
{
    [Serializable]
    public class CharacterStats : IMemorySerializable<CharacterStats>
    {
        [field: SerializeField] public float Kim { get; private set; }
        [field: SerializeField] public float Moc { get; private set; }
        [field: SerializeField] public float Thuy { get; private set; }
        [field: SerializeField] public float Hoa { get; private set; }
        [field: SerializeField] public float Tho { get; private set; }
        [field: SerializeField] public int KinhMach { get; private set; }
        [field: SerializeField] public int NgoTinh { get; private set; }
        [field: SerializeField] public int ThanThuc { get; private set; }
        [field: SerializeField] public int TocDoBay { get; private set; }
        [field: SerializeField] public int LinhThach { get; private set; }
        [field: SerializeField] public int SinhMenh { get; private set; }
        [field: SerializeField] public int HuyetKhi { get; private set; }
        [field: SerializeField] public int TamCanh { get; private set; }
        [field: SerializeField] public LinhCan LinhCan { get; private set; }
        [field: SerializeField] public Ability Ability { get; private set; }
        private readonly Dictionary<StatType,Action<object>> onStatChanges = new();
        public void CopyFrom(CharacterStats stats)
        {
            Kim = stats.Kim;
            Moc = stats.Moc;
            Thuy = stats.Thuy;
            Hoa = stats.Hoa;
            Tho = stats.Tho;
            KinhMach = stats.KinhMach;
            NgoTinh = stats.NgoTinh;
            ThanThuc = stats.ThanThuc;
            TocDoBay = stats.TocDoBay;
            LinhThach = stats.LinhThach;
            SinhMenh = stats.SinhMenh;
            HuyetKhi = stats.HuyetKhi;
            TamCanh = stats.TamCanh;
            LinhCan = stats.LinhCan;
            Ability = stats.Ability;
            foreach (var onStatChange in onStatChanges)
            {
                var value = GetValue(onStatChange.Key);
                onStatChange.Value(value);
            }
        }
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
                case StatType.Kim:
                    return Kim;
                case StatType.Moc:
                    return Moc;
                case StatType.Thuy:
                    return Thuy;
                case StatType.Hoa:
                    return Hoa;
                case StatType.Tho:
                    return Tho;
                case StatType.KinhMach:
                    return KinhMach;
                case StatType.NgoTinh:
                    return NgoTinh;
                case StatType.ThanThuc:
                    return ThanThuc;
                case StatType.TocDoBay:
                    return TocDoBay;
                case StatType.LinhThach:
                    return LinhThach;
                case StatType.SinhMenh:
                    return SinhMenh;
                case StatType.HuyetKhi:
                    return HuyetKhi;
                case StatType.TamCanh:
                    return TamCanh;
                case StatType.LinhCan:
                    return LinhCan;
                case StatType.Ability:
                    return Ability;
                default:
                    return null;
            }
        }
        public void SetValue(StatType type, float value)
        {
            if (type < StatType.KinhMach)
            {
                switch (type)
                {
                    case StatType.Kim:
                        Kim = value;
                        break;
                    case StatType.Moc:
                        Moc = value;
                        break;
                    case StatType.Thuy:
                        Thuy = value;
                        break;
                    case StatType.Hoa:
                        Hoa = value;
                        break;
                    case StatType.Tho:
                        Tho = value;
                        break;
                }
                Emit(type, value);
            }
            else if (type <= StatType.TamCanh)
            {
                var valueInt = (int)value;
                switch (type)
                {
                    case StatType.KinhMach:
                        KinhMach = valueInt;
                        break;
                    case StatType.NgoTinh:
                        NgoTinh = valueInt;
                        break;
                    case StatType.ThanThuc:
                        ThanThuc = valueInt;
                        break;
                    case StatType.TocDoBay:
                        TocDoBay = valueInt;
                        break;
                    case StatType.LinhThach:
                        LinhThach = valueInt;
                        break;
                    case StatType.SinhMenh:
                        SinhMenh = valueInt;
                        break;
                    case StatType.HuyetKhi:
                        HuyetKhi = valueInt;
                        break;
                    case StatType.TamCanh:
                        TamCanh = valueInt;
                        break;
                }
                Emit(type, valueInt);
            }
            else if (type == StatType.LinhCan)
            {
                var valueInt = (int)value;
                LinhCan = (LinhCan)valueInt;
                Emit(type, LinhCan);
            }
            else if (type == StatType.Ability)
            {
                var valueInt = (int)value;
                Ability = (Ability)valueInt;
                Emit(type, Ability);
            }
        }

        public int ByteSize()
        {
           return Kim.ByteSize() + Moc.ByteSize() + Thuy.ByteSize() + Hoa.ByteSize() + Tho.ByteSize() + KinhMach.ByteSize() + NgoTinh.ByteSize()
               + ThanThuc.ByteSize() + TocDoBay.ByteSize() + LinhThach.ByteSize() + SinhMenh.ByteSize() + HuyetKhi.ByteSize() + TamCanh.ByteSize()
               + LinhCan.ByteSize() + Ability.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(Kim);
            writer.Write(Moc);
            writer.Write(Thuy);
            writer.Write(Hoa);
            writer.Write(Tho);
            writer.Write(KinhMach);
            writer.Write(NgoTinh);
            writer.Write(ThanThuc);
            writer.Write(TocDoBay);
            writer.Write(LinhThach);
            writer.Write(SinhMenh);
            writer.Write(HuyetKhi);
            writer.Write(TamCanh);
            writer.Write(LinhCan);
            writer.Write(Ability);
        }
        public void Deserialize(IReader reader, ref CharacterStats value)
        {
            value.Kim = reader.Read<float>();
            value.Moc = reader.Read<float>();
            value.Thuy = reader.Read<float>();
            value.Hoa = reader.Read<float>();
            value.Tho = reader.Read<float>();
            value.KinhMach = reader.Read<int>();
            value.NgoTinh = reader.Read<int>();
            value.ThanThuc = reader.Read<int>();
            value.TocDoBay = reader.Read<int>();
            value.LinhThach = reader.Read<int>();
            value.SinhMenh = reader.Read<int>();
            value.HuyetKhi = reader.Read<int>();
            value.TamCanh = reader.Read<int>();
            value.LinhCan = reader.Read<LinhCan>();
            value.Ability = reader.Read<Ability>();
        }
    }
}
