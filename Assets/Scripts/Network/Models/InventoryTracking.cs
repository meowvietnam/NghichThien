using CreateCharacter;
using LuyenDan;

namespace Network.Models
{
    public class InventoryTracking
    {
        public Modify<ItemRef> Items { get; set; } = new();
        public Modify<FurnaceData> Furnaces { get; set; } = new();
        public Modify<NgoDao> NgoDaos { get; set; } = new();
        public Modify<CamNgo> CamNgos { get; set; } = new();
        public Modify<CongPhap> CongPhaps { get; set; } = new();
        public Modify<ThanThong> ThanThongs { get; set; } = new();
        public Modify<Equipment> Equipments { get; set; } = new();
        public void Clear()
        {
            Items.Clear();
            Furnaces.Clear();
            NgoDaos.Clear();
            CamNgos.Clear();
            CongPhaps.Clear();
            ThanThongs.Clear();
            Equipments.Clear();
        }


        public void Apply(InventoryManager inventory)
        {
            foreach (var item in Items.Deletes)
            {
                inventory.Remove(item);
            }
            foreach (var item in Items.Updates)
            {
                inventory.Update(item);
            }
            foreach (var item in Items.Adds)
            {
                inventory.Add(item);
            }
            foreach (var item in Equipments.Deletes)
            {
                inventory.Remove(item);
            }
            foreach (var item in Equipments.Updates)
            {
                inventory.Update(item);
            }
            foreach (var item in Equipments.Adds)
            {
                inventory.Add(item);
            }
            Furnaces.Apply(inventory.Furnaces);
            NgoDaos.Apply(inventory.NgoDaos);
            CamNgos.Apply(inventory.CamNgos);
            CongPhaps.Apply(inventory.CongPhapOwners);
            ThanThongs.Apply(inventory.ThanThongOwners);
        }
    }
}