using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using LuyenDan.Inventory;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    public class RecipeData : IItemData, IMemorySerializable<RecipeData>
    {
        [Serializable]
        public class Herbs : IMemorySerializable<Herbs>
        {
            [field: SerializeField] public int Id { get; private set; }
            [field: SerializeField] public int Count { get; private set; }
            [field: SerializeField] public int BaDuoc { get; private set; }

            public Herbs(int id, int count, int baDuoc)
            {
                Id = id;
                Count = count;
                BaDuoc = baDuoc;
            }

            public Herbs()
            {
            }

            public bool IsDefault()
            {
                return Id == 0;
            }

            public void Deserialize(IReader reader, ref Herbs value)
            {
                value.Id = reader.Read<int>();
                value.Count = reader.Read<int>();
                value.BaDuoc = reader.Read<int>();
            }

            public int ByteSize()
            {
                return Id.ByteSize() + Count.ByteSize() + BaDuoc.ByteSize();
            }

            public void Serialize(IWriter writer)
            {
                writer.Write(Id);
                writer.Write(Count);
                writer.Write(BaDuoc);
            }
        }

        [Serializable]
        public class Recipe : IMemorySerializable<Recipe>
        {
            [field: SerializeField] public int Id { get; private set; }
            [field: SerializeField] public Herbs ChuDuoc { get; private set; }
            [field: SerializeField] public Herbs PhuDuoc1 { get; private set; }
            [field: SerializeField] public Herbs PhuDuoc2 { get; private set; }
            [field: SerializeField] public Herbs DuocDan { get; private set; }

            public void Create(int id, int index, string[,] matrix)
            {
                Id = id;
                var colIndex = 2 + index * 6;
                ChuDuoc = GetHerbs(0, colIndex, matrix);
                PhuDuoc1 = GetHerbs(2, colIndex, matrix);
                PhuDuoc2 = GetHerbs(3, colIndex, matrix);
                DuocDan = GetHerbs(4, colIndex, matrix);
            }

            private Herbs GetHerbs(int rowIndex, int colIndex, string[,] matrix)
            {
                var countText = matrix[rowIndex, colIndex];
                int.TryParse(countText, out var count);
                if (count == 0)
                {
                    return null;
                }

                var name = matrix[rowIndex, colIndex + 1];
                var h = LuyenDanResources.Instance.HerbDatas.FirstOrDefault(x =>
                    x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (h == null)
                {
                    Debug.LogError($"There is no herbs with name '{name}'");
                }

                var id = h.Id;
                var baduoc = int.Parse(matrix[rowIndex, colIndex + 4]);
                return new Herbs(id, count, baduoc);
            }

            public void Deserialize(IReader reader, ref Recipe value)
            {
                value.Id = reader.Read<int>();
                value.ChuDuoc = reader.Read<Herbs>();
                value.PhuDuoc1 = reader.Read<Herbs>();
                value.PhuDuoc2 = reader.Read<Herbs>();
                value.DuocDan = reader.Read<Herbs>();
            }

            public int ByteSize()
            {
                return Id.ByteSize() + ChuDuoc.ByteSize() 
                                     + PhuDuoc1.ByteSize() + PhuDuoc2.ByteSize()
                                     + DuocDan.ByteSize();
            }

            public void Serialize(IWriter writer)
            {
                writer.Write(Id);
                writer.Write(ChuDuoc);
                writer.Write(PhuDuoc1);
                writer.Write(PhuDuoc2);
                writer.Write(DuocDan);
            }
        }

        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public int LinhDanId { get; set; }

        [ShowInInspector, ReadOnly, PreviewField(75)]
        public Sprite Icon
        {
            get
            {
                var linhDan = LinhDan;
                if (linhDan == null)
                {
                    return null;
                }

                return linhDan.Icon;
            }
        }

        [field: SerializeField] public List<Recipe> Recipes { get; private set; }

        public LinhDan LinhDan
        {
            get { return LuyenDanResources.Instance.LinhDans.FirstOrDefault(x => x.Id == LinhDanId); }
        }

        public string Name => $"Đan phương {LinhDan.Name}" ;

        public Tier Tier => LinhDan.Tier;

        public string Description => $"Công thức chế tạo {LinhDan.Name}";
        public void Deserialize(IReader reader, ref RecipeData value)
        {
            value.Id = reader.Read<int>();
            value.LinhDanId = reader.Read<int>();
            value.Recipes = reader.ReadArray<Recipe>().ToList();
        }

        public int ByteSize()
        {
            return Id.ByteSize() + LinhDanId.ByteSize() + Recipes.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(LinhDanId);
            writer.WriteArray(Recipes);
        }
    }
}