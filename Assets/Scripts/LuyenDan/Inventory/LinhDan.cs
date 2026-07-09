using System;
using System.IO;
using Base;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenDan.Inventory
{
    [Serializable]
    public class LinhDan : IItemData, IMemorySerializable<LinhDan>
    {
        [field: SerializeField] public int Id { get; private set; }

        public Sprite Icon
        {
            get
            {
                if (icon == null)
                {
                    icon = Resources.Load<Sprite>(IconPath);
                }
                return icon;
            }
        }

        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Tier Tier { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public int DanDoc { get; private set; }
        [field: SerializeField] public int NhinThuoc { get; private set; }
        [field: SerializeField] public int PriceV104 { get; private set; }
        [field: SerializeField] public int PriceV130 { get; private set; }
        [field: SerializeField] public int Price { get; private set; }
        [field: SerializeField] public int Profit { get; private set; }
        [field: SerializeField] public float Income { get; private set; }
        [field: SerializeField] public int ProfitPerDay { get; private set; }
        [field: SerializeField] public int ProfitDanThanh { get; private set; }
        [field: SerializeField] public int ProfitDanThanhPerDay { get; private set; }
        [field: SerializeField] public float IncomeDanThanh { get; private set; }
        [field: SerializeField] public ETypeDan ETypeDan { get; private set; }
        [field: SerializeField] public int BuffValue { get; private set; }
        [field: SerializeField] public EDanBuffType EBuffType{ get; private set; }
        private Sprite icon;
        [field: SerializeField] public string IconPath { get; private set; }
        public int ByteSize()
        {
            return Id.ByteSize() + Name.ByteSize() + Tier.ByteSize() + Description.ByteSize()
                   + DanDoc.ByteSize() + NhinThuoc.ByteSize() + PriceV104.ByteSize() + PriceV130.ByteSize()
                   + Price.ByteSize() + Profit.ByteSize() + Income.ByteSize()
                   + ProfitPerDay.ByteSize() + ProfitDanThanh.ByteSize() + ProfitDanThanhPerDay.ByteSize()
                   + IncomeDanThanh.ByteSize() + ETypeDan.ByteSize()
                   + BuffValue.ByteSize() + EBuffType.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Name);
            writer.Write(Tier);
            writer.Write(Description);
            writer.Write(DanDoc);
            writer.Write(NhinThuoc);
            writer.Write(PriceV104);
            writer.Write(PriceV130);
            writer.Write(Price);
            writer.Write(Profit);
            writer.Write(Income);
            writer.Write(ProfitPerDay);
            writer.Write(ProfitDanThanh);
            writer.Write(ProfitDanThanhPerDay);
            writer.Write(IncomeDanThanh);
            writer.Write(ETypeDan);
            writer.Write(BuffValue);
            writer.Write(EBuffType);
        }

        public void Deserialize(IReader reader, ref LinhDan value)
        {
            value.Id = reader.Read<int>();
            value.Name = reader.Read();
            value.Tier = reader.Read<Tier>();
            value.Description = reader.Read();
            value.DanDoc = reader.Read<int>();
            value.NhinThuoc = reader.Read<int>();
            value.PriceV104 = reader.Read<int>();
            value.PriceV130 = reader.Read<int>();
            value.Price = reader.Read<int>();
            value.Profit = reader.Read<int>();
            value.Income = reader.Read<float>();
            value.ProfitPerDay = reader.Read<int>();
            value.ProfitDanThanh = reader.Read<int>();
            value.ProfitDanThanhPerDay = reader.Read<int>();
            value.IncomeDanThanh = reader.Read<float>();
            value.ETypeDan = reader.Read<ETypeDan>();
            value.BuffValue = reader.Read<int>();
            value.EBuffType = reader.Read<EDanBuffType>();
        }
#if UNITY_EDITOR
        
        public void Move()
        {
            var icon = Icon;
            var root = "Assets/Resources/";
            var path = Path.GetDirectoryName(root + IconPath);
            AssetOptimize.Move(icon.texture, path);
        }
#endif
        
    }
}