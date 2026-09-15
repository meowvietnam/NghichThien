using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using CreateCharacter;
using CreateCharacter.CharStats;
using MemoryPack;
using Network.Core;
using Network.Extensions;
using Network.Models.Clans;
using UnityEngine;

namespace Network.Models
{
    [MemoryPackable]
    public partial class CharacterSnapshot
    {
        public int Id { get; set; }
        public int ThanThuc { get; set; }
        public int TocDoBay { get; set; }
        public int NgoTinh { get; set; }
        public int TuChat { get; set; }
        public int ThoNguyen { get; set; }
        public int SinhMenh { get; set; }
        public int Hp { get; set; }
        public int DanDoc { get; set; }
        public Ability Ability { get; set; }
        public float Kim { get; set; }
        public float Moc { get; set; }
        public float Thuy { get; set; }
        public float Hoa { get; set; }
        public float Tho { get; set; }
        public float Ma { get; set; }
        public HashSet<NgoDao> NgoDaos { get; set; }
        public HashSet<DanCombatUsed> DanCombatUseds { get; set; }
        public Equipment VuKhi { get; set; }
        public Equipment PhongCu { get; set; }
        public Equipment TrangSuc { get; set; }
        public TimeSpan TuoiTac { get; set; }
        public EMonPhai MonPhai { get; set; }
        public int Level { get; set; }
        public Dictionary<string, Color> Colors { get; set; }
        public Dictionary<string, string> Skins { get; set; }
        public Gender Gender { get; set; }
        public string Name { get; set; }
        public List<int> ThanThongEquip { get; set; }
        public int PhuTu4Used { get; set; }
        public int PhuTu3Used { get; set; }
        public int PhuTu2Used { get; set; }
        public int PhuTu1Used { get; set; }
        public int ChuTu2Used { get; set; }
        public int ChuTu1Used { get; set; }
        public int PointArena { get; set; }
        public bool HasClan { get; set; }
        public int ClanId { get; set; }
        public string ClanName { get; set; } = string.Empty;
        public string ClanImageUrl { get; set; }
        public int ClanLevel { get; set; }
        public long Hon { get; set; }
        public ClanRole ClanRole { get; set; }
        public ClanInheritanceEffect[] ClanInheritanceEffects { get; set; } = new ClanInheritanceEffect[0];

        [MemoryPackConstructor]
        public CharacterSnapshot()
        {
        }

        public CharacterSnapshot(CharacterData c)
        {
            this.Id = c.Id;
            var equipment = c.EquipmentManager;
            this.Name = c.Name;
            this.Gender = c.Gender;
            this.Skins = c.Appearance.Skins;
            this.Colors = c.Appearance.Colors;
            this.Level = c.Progression.Level;
            this.MonPhai = c.MonPhai;
            this.TuoiTac = c.Stats.TuoiTac;
            this.SinhMenh = c.Stats.SinhMenh;
            this.ThoNguyen = c.Stats.ThoNguyen;
            this.TuChat = c.Stats.TuChat;
            this.NgoTinh = c.Stats.NgoTinh;
            this.TocDoBay = c.Stats.TocDoBay;
            this.ThanThuc = c.Stats.ThanThuc;
            this.ChuTu1Used = equipment.ChuTu1Used;
            this.ChuTu2Used = equipment.ChuTu2Used;
            this.PhuTu1Used = equipment.PhuTu1Used;
            this.PhuTu2Used = equipment.PhuTu2Used;
            this.PhuTu3Used = equipment.PhuTu3Used;
            this.PhuTu4Used = equipment.PhuTu4Used;
            this.ThanThongEquip = equipment.ThanThongEquips
                .OrderBy(x => x.SlotIndex)
                .Select(x => (int)x.Id)
                .ToList();
            this.DanCombatUseds = equipment.DanCombatUseds;
            this.NgoDaos = c.InventoryManager.NgoDaos;
            this.VuKhi = c.InventoryManager.VuKhis.FirstOrDefault(x => x.SelfId == c.EquipmentManager.VuKhi);
            this.PhongCu = c.InventoryManager.PhongCus.FirstOrDefault(x => x.SelfId == c.EquipmentManager.PhongCu);
            this.TrangSuc = c.InventoryManager.TrangSucs.FirstOrDefault(x => x.SelfId == c.EquipmentManager.TrangSuc);
            this.Kim = c.Stats.Kim;
            this.Moc = c.Stats.Moc;
            this.Thuy = c.Stats.Thuy;
            this.Hoa = c.Stats.Hoa;
            this.Tho = c.Stats.Tho;
            this.Ma = c.Stats.Ma;
            this.Ability = c.Stats.Ability;
            this.DanDoc = c.Stats.DanDoc;
            this.Hp = c.Stats.Hp;
            this.PointArena = c.ArenaManager.Points;
            var clan = Client.Instance?.CoreModule?.GetHandler<ClanHandler>()?.CurrentClan;
            var member = clan?.Members?.FirstOrDefault(x => x.CharId == c.Id);
            if (clan != null && member != null)
            {
                this.HasClan = true;
                this.ClanId = clan.Id;
                this.ClanName = clan.Name;
                this.ClanImageUrl = clan.ImageUrl;
                this.ClanLevel = clan.Level;
                this.Hon = clan.Hon;
                this.ClanRole = member.Role;
                this.ClanInheritanceEffects = (clan.StartTurnEffects ?? new List<ClanInheritanceEffect>())
                    .Concat(clan.EndTurnEffects ?? new List<ClanInheritanceEffect>())
                    .ToArray();
            }
        }
    }
}
