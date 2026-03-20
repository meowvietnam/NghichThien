using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenDan.Inventory
{
    [Serializable]
    public class LinhDan : IItemData
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField, PreviewField(75)] public Sprite Icon { get; private set;}
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
    }
}