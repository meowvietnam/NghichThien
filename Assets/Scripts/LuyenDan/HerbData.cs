using System;
using System.Linq;
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
        public void Modify()
        {
            Name = Normalize1(Name);
            ChuDuoc = Normalize2(ChuDuoc);
            PhuDuoc = Normalize2(PhuDuoc);
            ThuocDan = Normalize2(ThuocDan);
            Locations = Locations.Trim();
        }
        private string Normalize(string text)
        {
            if(string.IsNullOrEmpty(text)) return string.Empty;
            return text.Substring(0, 1).ToUpper() + text.Substring(1).ToLower();
        }
        private string Normalize1(string text)
        {
            text = text.Trim();
            var parts = text.Split(" ");
            return string.Join(" ", parts.Select(Normalize));
        }

        private string Normalize2(string text)
        {
            text = text.Trim();
            return Normalize(text);
        }
    }
}
