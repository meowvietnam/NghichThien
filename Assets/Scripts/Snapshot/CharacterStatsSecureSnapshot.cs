using System.Collections.Generic;
using System.Linq;
using Base;
using CreateCharacter;
using CreateCharacter.CharStats;
using MemoryPack;
using UnityEngine;

namespace Snapshot
{
    [MemoryPackable]
    public partial class CharacterStatsSecureSnapshot
    {
        public int Id { get; set; }
        public Gender Gender { get; set; }
        public string Name { get; set; }
        public EMonPhai MonPhai { get; set; }
        public ProgressionSnapshot Progression { get; set; } = new();
        public StatsSnapshot Stats { get; set; } = new();
        public AppearanceSnapshot Appearance { get; set; } = new();
        public InventorySnapshot Inventory { get; set; } = new();
        public EquipmentSnapshot Equipment { get; set; } = new();

        [MemoryPackConstructor]
        public CharacterStatsSecureSnapshot()
        {
        }

        public CharacterStatsSecureSnapshot(CharacterStatsSecure stats)
        {
            if (stats == null)
            {
                return;
            }

            Id = stats.idChar;
            Gender = stats.Gender;
            Name = stats.nameUser;
            MonPhai = stats.monPhai;
            Progression = CreateProgression(stats);
            Stats = CreateStats(stats);
            Appearance = CreateAppearance(stats);
            Inventory = CreateInventory(stats);
            Equipment = CreateEquipment(stats);
        }

        private static ProgressionSnapshot CreateProgression(CharacterStatsSecure stats)
        {
            return new ProgressionSnapshot
            {
                Level = stats.level,
                CurrentExp = stats.currentExp,
                NgoDaoDiem = stats.ngoDaoDiem,
                IdCurrentNode = stats.idNode
            };
        }

        private static StatsSnapshot CreateStats(CharacterStatsSecure stats)
        {
            return new StatsSnapshot
            {
                Kim = stats.percentKim,
                Moc = stats.percentMoc,
                Thuy = stats.percentThuy,
                Hoa = stats.percentHoa,
                Tho = stats.percentTho,
                Ma = stats.percentMa,
                TuChat = stats.tuChat,
                NgoTinh = stats.ngoTinh,
                ThanThuc = stats.thanThuc,
                TocDoBay = stats.tocDoBay,
                LinhThach = stats.linhThach,
                Hp = stats.hp,
                SinhMenh = stats.hpMax,
                ThoNguyen = stats.thoNguyen,
                TamCanh = stats.tamCanh,
                LinhCan = (LinhCan)(int)stats.idLinhCan,
                Ability = stats.ability,
                DanDoc = stats.danDoc,
                TuoiTac = System.TimeSpan.FromDays((int)stats.tuoiTac * 365)
            };
        }

        private static AppearanceSnapshot CreateAppearance(CharacterStatsSecure stats)
        {
            return new AppearanceSnapshot
            {
                Skins = stats.skinList?.ToDictionary(x => x.key, x => x.value) ?? new Dictionary<string, string>(),
                Colors = stats.colorList?.ToDictionary(x => x.key, x => x.value) ?? new Dictionary<string, Color>()
            };
        }

        private static InventorySnapshot CreateInventory(CharacterStatsSecure stats)
        {
            return new InventorySnapshot
            {
                CamNgos = stats.camNgoSaves?.Select(ToCamNgo).ToHashSet() ?? new HashSet<CamNgo>(),
                CongPhapOwners = stats.congPhapOwner?.Select(ToCongPhap).ToHashSet() ?? new HashSet<CongPhap>(),
                ThanThongOwners = stats.thanThongOwner?.Select(ToThanThong).ToHashSet() ?? new HashSet<ThanThong>(),
                VuKhis = CreateEquipmentSet(stats.vuKhi),
                PhongCus = CreateEquipmentSet(stats.phongCu),
                TrangSucs = CreateEquipmentSet(stats.trangSuc)
            };
        }

        private static EquipmentSnapshot CreateEquipment(CharacterStatsSecure stats)
        {
            return new EquipmentSnapshot
            {
                ThanThongEquips = stats.thanThongEquip?.Select((id, index) => new ThanThongEquip
                {
                    Id = ToUShort(id),
                    SlotIndex = (byte)index
                }).ToHashSet() ?? new HashSet<ThanThongEquip>(),
                DanCombatUseds = stats.danDuocCombat?.Select(ToDanCombatUsed).ToHashSet() ??
                                 new HashSet<DanCombatUsed>(),
                DonThuatUsed = ToUShort(stats.donThuatUsed),
                PhuTu1Used = ToUShort(stats.phuTu1Used),
                PhuTu2Used = ToUShort(stats.phuTu2Used),
                PhuTu3Used = ToUShort(stats.phuTu3Used),
                PhuTu4Used = ToUShort(stats.phuTu4Used),
                ChuTu1Used = ToUShort(stats.chuTu1Used),
                ChuTu2Used = ToUShort(stats.chuTu2Used),
                VuKhi = ToUShort(stats.vuKhi?.id),
                PhongCu = ToUShort(stats.phongCu?.id),
                TrangSuc = ToUShort(stats.trangSuc?.id)
            };
        }

        private static HashSet<Equipment> CreateEquipmentSet(EquipmentClient equipment)
        {
            var value = ToEquipment(equipment);
            return value == null ? new HashSet<Equipment>() : new HashSet<Equipment> { value };
        }

        private static Equipment ToEquipment(EquipmentClient equipment)
        {
            if (equipment == null)
            {
                return null;
            }

            var id = ToUShort(equipment.id);
            if (id == ushort.MaxValue)
            {
                return null;
            }

            return new Equipment
            {
                SelfId = id,
                Id = id,
                Name = equipment.nameEquip,
                Effects = equipment.listEffectEquip?.Select(effect => new Effect
                {
                    Type = effect.Type,
                    Value = (byte)effect.Value
                }).ToArray() ?? System.Array.Empty<Effect>(),
                MaterialIds = System.Array.Empty<ushort>()
            };
        }

        private static CamNgo ToCamNgo(CharacterStatsSecure.DataSave data)
        {
            return new CamNgo
            {
                Id = (BaseSkill.ETypeSkill)(int)data.id,
                Value = data.value
            };
        }

        private static CongPhap ToCongPhap(CharacterStatsSecure.DataSave data)
        {
            return new CongPhap
            {
                Id = ToUShort(data.id),
                Level = ToUShort(data.value)
            };
        }

        private static ThanThong ToThanThong(SecureInt id)
        {
            return new ThanThong
            {
                Id = ToUShort(id)
            };
        }

        private static DanCombatUsed ToDanCombatUsed(CharacterStatsSecure.DataSave data)
        {
            return new DanCombatUsed
            {
                Id = ToUShort(data.id),
                Count = ToUShort(data.value)
            };
        }

        private static ushort ToUShort(SecureInt value)
        {
            return value == null ? ushort.MaxValue : (ushort)Mathf.Clamp((int)value, ushort.MinValue, ushort.MaxValue);
        }

        [MemoryPackable]
        public partial class ProgressionSnapshot
        {
            public int Level { get; set; }
            public int CurrentExp { get; set; }
            public int NgoDaoDiem { get; set; }
            public int IdCurrentNode { get; set; }
        }

        [MemoryPackable]
        public partial class StatsSnapshot
        {
            public float Kim { get; set; }
            public float Moc { get; set; }
            public float Thuy { get; set; }
            public float Hoa { get; set; }
            public float Tho { get; set; }
            public float Ma { get; set; }
            public int TuChat { get; set; }
            public int NgoTinh { get; set; }
            public int ThanThuc { get; set; }
            public int TocDoBay { get; set; }
            public int LinhThach { get; set; }
            public int Hp { get; set; }
            public int SinhMenh { get; set; }
            public int ThoNguyen { get; set; }
            public int TamCanh { get; set; }
            public LinhCan LinhCan { get; set; }
            public Ability Ability { get; set; }
            public int DanDoc { get; set; }
            public System.TimeSpan TuoiTac { get; set; }
        }

        [MemoryPackable]
        public partial class AppearanceSnapshot
        {
            public Dictionary<string, string> Skins { get; set; } = new();
            public Dictionary<string, Color> Colors { get; set; } = new();
        }

        [MemoryPackable]
        public partial class InventorySnapshot
        {
            public HashSet<CamNgo> CamNgos { get; set; } = new();
            public HashSet<CongPhap> CongPhapOwners { get; set; } = new();
            public HashSet<ThanThong> ThanThongOwners { get; set; } = new();
            public HashSet<Equipment> VuKhis { get; set; } = new();
            public HashSet<Equipment> PhongCus { get; set; } = new();
            public HashSet<Equipment> TrangSucs { get; set; } = new();
        }

        [MemoryPackable]
        public partial class EquipmentSnapshot
        {
            public HashSet<ThanThongEquip> ThanThongEquips { get; set; } = new();
            public HashSet<DanCombatUsed> DanCombatUseds { get; set; } = new();
            public ushort DonThuatUsed { get; set; } = ushort.MaxValue;
            public ushort PhuTu1Used { get; set; } = ushort.MaxValue;
            public ushort PhuTu2Used { get; set; } = ushort.MaxValue;
            public ushort PhuTu3Used { get; set; } = ushort.MaxValue;
            public ushort PhuTu4Used { get; set; } = ushort.MaxValue;
            public ushort ChuTu1Used { get; set; } = ushort.MaxValue;
            public ushort ChuTu2Used { get; set; } = ushort.MaxValue;
            public ushort VuKhi { get; set; } = ushort.MaxValue;
            public ushort PhongCu { get; set; } = ushort.MaxValue;
            public ushort TrangSuc { get; set; } = ushort.MaxValue;
        }
    }
}