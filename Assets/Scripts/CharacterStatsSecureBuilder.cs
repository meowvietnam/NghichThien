using System.Collections.Generic;
using System.Linq;
using Network.Models;
using Base;

public class CharacterStatsSecureBuilder
{
    private readonly CharacterStatsSecure _stats = new CharacterStatsSecure();

    public CharacterStatsSecureBuilder WithTuoiTac(int value)
    {
        _stats.tuoiTac = new SecureInt(value);
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

    public CharacterStatsSecure Build()
    {
        return _stats;
    }

    public static CharacterStatsSecure FromCharacterSnapshot(CharacterSnapshot character)
    {
        return new CharacterStatsSecureBuilder()
            .WithTuoiTac(GameManager.GetIntTuoiTac(character.TuoiTac))
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
            .Build();
    }
}
