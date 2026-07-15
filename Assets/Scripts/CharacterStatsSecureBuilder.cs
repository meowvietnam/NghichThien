using Base;
using CreateCharacter.CharStats;
using Network.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine; // Thêm nếu class Color yêu cầu UnityEngine, hoặc dùng namespace chứa Color của bạn

public class CharacterStatsSecureBuilder
{
    private readonly CharacterStatsSecure _stats = new CharacterStatsSecure();

    public CharacterStatsSecureBuilder WithGender(Gender value)
    {
        _stats.Gender = value;
        return this;
    }

    public CharacterStatsSecureBuilder WithTuoiTac(int value)
    {
        _stats.tuoiTac = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithHp(int value)
    {
        _stats.hp = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithHpMax(int value)
    {
        _stats.hpMax = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithLevel(int value)
    {
        _stats.level = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithThoNguyen(int value)
    {
        _stats.thoNguyen = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithTuChat(int value)
    {
        _stats.tuChat = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithNgoTinh(int value)
    {
        _stats.ngoTinh = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithTocDoBay(int value)
    {
        _stats.tocDoBay = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithThanThuc(int value)
    {
        _stats.thanThuc = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithNameUser(string name)
    {
        _stats.nameUser = name;
        return this;
    }

    public CharacterStatsSecureBuilder WithMonPhai(EMonPhai monPhai)
    {
        _stats.monPhai = monPhai;
        return this;
    }
    public CharacterStatsSecureBuilder WithPointArena(int value)
    {
        _stats.pointArena = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithThanThongEquip(List<SecureInt> items)
    {
        _stats.thanThongEquip = items ?? new List<SecureInt>();
        return this;
    }

    public CharacterStatsSecureBuilder WithChuTu1Used(int value)
    {
        _stats.chuTu1Used = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithChuTu2Used(int value)
    {
        _stats.chuTu2Used = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithPhuTu1Used(int value)
    {
        _stats.phuTu1Used = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithPhuTu2Used(int value)
    {
        _stats.phuTu2Used = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithPhuTu3Used(int value)
    {
        _stats.phuTu3Used = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithPhuTu4Used(int value)
    {
        _stats.phuTu4Used = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithAbility(Ability value)
    {
        _stats.ability = value;
        return this;
    }

    public CharacterStatsSecureBuilder WithPercentKim(float value)
    {
        _stats.percentKim = new SecureFloat(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithPercentMoc(float value)
    {
        _stats.percentMoc = new SecureFloat(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithPercentThuy(float value)
    {
        _stats.percentThuy = new SecureFloat(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithPercentHoa(float value)
    {
        _stats.percentHoa = new SecureFloat(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithPercentTho(float value)
    {
        _stats.percentTho = new SecureFloat(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithPercentMa(float value)
    {
        _stats.percentMa = new SecureFloat(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithDanDoc(int value)
    {
        _stats.danDoc = new SecureInt(value);
        return this;
    }

    public CharacterStatsSecureBuilder WithNgoDaoSaves(List<SecureInt> items)
    {
        _stats.ngoDaoSaves = items ?? new List<SecureInt>();
        return this;
    }

    public CharacterStatsSecureBuilder WithSkinList(List<CharacterStatsSecure.SkinData> skins)
    {
        _stats.skinList = skins ?? new List<CharacterStatsSecure.SkinData>();
        return this;
    }

    public CharacterStatsSecureBuilder WithColorList(List<CharacterStatsSecure.ColorData> colors)
    {
        _stats.colorList = colors ?? new List<CharacterStatsSecure.ColorData>();
        return this;
    }

    public CharacterStatsSecureBuilder WithVuKhi(EquipmentClient equipment)
    {
        _stats.vuKhi = equipment;
        return this;
    }

    public CharacterStatsSecureBuilder WithPhongCu(EquipmentClient equipment)
    {
        _stats.phongCu = equipment;
        return this;
    }

    public CharacterStatsSecureBuilder WithTrangSuc(EquipmentClient equipment)
    {
        _stats.trangSuc = equipment;
        return this;
    }

    public CharacterStatsSecureBuilder WithDanDuocCombat(List<CharacterStatsSecure.DataSave> items)
    {
        _stats.danDuocCombat = items ?? new List<CharacterStatsSecure.DataSave>();
        return this;
    }

    public CharacterStatsSecure Build()
    {
        return _stats;
    }
    public static CharacterStatsSecure ArenaFromCharacterSnapshot(CharacterSnapshot character)
    {
        var stats = FromCharacterSnapshot(character);
        if (stats != null)
        {
            // Vì Arena luôn lấy Hp = HpMax (SinhMenh)
            stats.hp = stats.hpMax;
        }
        return stats;
    }
    public static CharacterStatsSecure FromCharacterSnapshot(CharacterSnapshot character)
    {
        if (character == null) return null;

        return new CharacterStatsSecureBuilder()
            .WithGender(character.Gender) // KHÔNG QUÊN GENDER NỮA NHÉ BABE!
            .WithTuoiTac(GameManager.GetIntTuoiTac(character.TuoiTac))
            .WithHp(character.Hp)
            .WithHpMax(character.SinhMenh)
            .WithLevel(character.Level)
            .WithThoNguyen(character.ThoNguyen)
            .WithTuChat(character.TuChat)
            .WithNgoTinh(character.NgoTinh)
            .WithTocDoBay(character.TocDoBay)
            .WithThanThuc(character.ThanThuc)
            .WithNameUser(character.Name)
            .WithMonPhai(character.MonPhai)
            .WithThanThongEquip(character.ThanThongEquip.ConvertAll(id => new SecureInt(id)))
            .WithChuTu1Used(character.ChuTu1Used)
            .WithChuTu2Used(character.ChuTu2Used)
            .WithPhuTu1Used(character.PhuTu1Used)
            .WithPhuTu2Used(character.PhuTu2Used)
            .WithPhuTu3Used(character.PhuTu3Used)
            .WithPhuTu4Used(character.PhuTu4Used)
            .WithPointArena(character.PointArena)
            // --- ĐÃ ĐIỀN ĐỦ ĐOẠN KHUYẾT THIẾU ---
            .WithAbility(character.Ability)
            .WithPercentKim(character.Kim)
            .WithPercentMoc(character.Moc)
            .WithPercentThuy(character.Thuy)
            .WithPercentHoa(character.Hoa)
            .WithPercentTho(character.Tho)
            .WithPercentMa(character.Ma)
            .WithDanDoc(character.DanDoc)
            .WithNgoDaoSaves(character.NgoDaos?.Select(x => new SecureInt(x.Id)).ToList())
            .WithSkinList(character.Skins?.Select(kvp => new CharacterStatsSecure.SkinData { key = kvp.Key, value = kvp.Value }).ToList())
            .WithColorList(character.Colors?.Select(kvp => new CharacterStatsSecure.ColorData { key = kvp.Key, value = kvp.Value }).ToList())
           // Giả định class EquipmentClientBuilder của bạn xử lý nhận Id ushort gốc từ snapshot
           .WithVuKhi(character.VuKhi != null ? new EquipmentClient
           {
               id = new SecureInt(character.VuKhi.Id),
               nameEquip = character.VuKhi.Name,
               listEffectEquip = character.VuKhi.Effects?.Select(e => new EquipmentClient.EffectClient
               {
                   Type = e.Type,
                   Value = new SecureInt(e.Value)
               }).ToList()
           } : new EquipmentClient())

            .WithPhongCu(character.PhongCu != null ? new EquipmentClient
            {
                id = new SecureInt(character.PhongCu.Id),
                nameEquip = character.PhongCu.Name,
                listEffectEquip = character.PhongCu.Effects?.Select(e => new EquipmentClient.EffectClient
                {
                    Type = e.Type,
                    Value = new SecureInt(e.Value)
                }).ToList()
            } : new EquipmentClient())

            .WithTrangSuc(character.TrangSuc != null ? new EquipmentClient
            {
                id = new SecureInt(character.TrangSuc.Id),
                nameEquip = character.TrangSuc.Name,
                listEffectEquip = character.TrangSuc.Effects?.Select(e => new EquipmentClient.EffectClient
                {
                    Type = e.Type,
                    Value = new SecureInt(e.Value)
                }).ToList()
            } : new EquipmentClient())
            .WithDanDuocCombat(character.DanCombatUseds?.Select(x => new CharacterStatsSecure.DataSave { id = new SecureInt(x.Id), value = new SecureInt(x.Count) }).ToList())
            .Build();
    }
}