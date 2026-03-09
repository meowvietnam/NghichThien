using System;
using LuyenDan.Inventory;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    public class HerbData : IInventoryItemData
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public Tier Tier { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string ChuDuoc { get; private set; }
        [field: SerializeField] public string PhuDuoc { get; private set; }
        [field: SerializeField] public string ThuocDan { get; private set; }
        [field: SerializeField] public string Locations { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        public HerbData(int id, Tier tier, string name, string chuduoc, string phuduoc, string thuocdan,
            string locations)
        {
            Id = id;
            Tier = tier;
            Name = name;
            ChuDuoc = chuduoc;
            PhuDuoc = phuduoc;
            ThuocDan = thuocdan;
            Locations = locations;
        }
    }
}
