using Base;
using CreateCharacter;
using CreateCharacter.CharStats;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class CharacterStatsSecure
{
    [System.Serializable]
    public class SkinData
    {
        public string key;
        public string value;
    }

    [System.Serializable]
    public class ColorData
    {
        public string key;
        public Color value;
    }

    // Truyền 0 hoặc 0f vào constructor của SecureInt/SecureFloat
    public SecureInt idChar = new SecureInt(0);
    public SecureFloat percentKim = new SecureFloat(0f);
    public SecureFloat percentMoc = new SecureFloat(0f);
    public SecureFloat percentThuy = new SecureFloat(0f);
    public SecureFloat percentHoa = new SecureFloat(0f);
    public SecureFloat percentTho = new SecureFloat(0f);
    public SecureFloat percentMa = new SecureFloat(0f);

    public SecureInt tuChat = new SecureInt(0);
    public SecureInt ngoTinh = new SecureInt(0);
    public SecureInt thanThuc = new SecureInt(0);
    public SecureInt tocDoBay = new SecureInt(0);
    public SecureInt linhThach = new SecureInt(0);
    public SecureInt hp = new SecureInt(0);
    public SecureInt hpMax = new SecureInt(0);
    public SecureInt thoNguyen = new SecureInt(0);
    public SecureInt tamCanh = new SecureInt(0);
    public SecureInt idLinhCan = new SecureInt(0);

    public Ability ability = new Ability(); // Đảm bảo Ability có constructor ko tham số

    public SecureInt ngoDaoDiem = new SecureInt(0);
    public SecureInt danDoc = new SecureInt(0);

    public string nameUser = "";
    public SecureInt level = new SecureInt(1); // Thường level bắt đầu từ 1
    public SecureInt tuoiTac = new SecureInt(18); // Ví dụ tuổi mặc định
    public SecureInt currentExp = new SecureInt(0);

    public List<SecureInt> ngoDaoSaves = new List<SecureInt>();
    public List<DataSave> camNgoSaves = new List<DataSave>();

    public class DataSave
    {
        // Trong class con cũng phải truyền giá trị
        public SecureInt id = new SecureInt(0);
        public SecureInt value = new SecureInt(0);
    }

    public EMonPhai monPhai;

    public List<SecureInt> thanThongOwner = new List<SecureInt>();
    public List<SecureInt> thanThongEquip = new List<SecureInt>();
    public List<DataSave> congPhapOwner = new List<DataSave>();

    // Công pháp used
    public SecureInt donThuatUsed = new SecureInt(0);
    public SecureInt phuTu1Used = new SecureInt(0);
    public SecureInt phuTu2Used = new SecureInt(0);
    public SecureInt phuTu3Used = new SecureInt(0);
    public SecureInt phuTu4Used = new SecureInt(0);
    public SecureInt chuTu1Used = new SecureInt(0);
    public SecureInt chuTu2Used = new SecureInt(0);

    public List<DataSave> relationShipNPC = new List<DataSave>();
    public Gender Gender;
    public List<SkinData> skinList = new List<SkinData>();
    public List<ColorData> colorList = new List<ColorData>();
    public Dictionary<string, string> Skins => skinList.ToDictionary(x => x.key, x => x.value);

    public Dictionary<string, Color> Colors => colorList.ToDictionary(x => x.key, x => x.value);
    public SecureInt idNode = new SecureInt(0);

    public CharacterStatsSecure Clone()
    {
        // 1. Sao chép nông các chỉ số cơ bản (HP, MP, ID...)
        CharacterStatsSecure clone = (CharacterStatsSecure)MemberwiseClone();


        return clone;
    }


    public void Clone(CharacterData data)
    {
        nameUser = data.Name;
        level = data.Level;
        currentExp = data.CurrentExp;
        ngoDaoDiem = data.NgoDaoDiem;

        Clone(data.Stats);
    }

    private void Clone(CreateCharacter.CharStats.CharacterStats stats)
    {
        percentKim = stats.Kim;
        percentMoc = stats.Moc;
        percentThuy = stats.Thuy;
        percentHoa = stats.Hoa;
        percentTho = stats.Tho;
        percentMa = stats.Ma;
        tuChat = stats.TuChat;
        ngoTinh = stats.NgoTinh;
        thanThuc = stats.ThanThuc;
        tocDoBay = stats.TocDoBay;
        linhThach = stats.LinhThach;
        hp = stats.Hp;
        hpMax = stats.SinhMenh;
        thoNguyen = stats.ThoNguyen;
        tamCanh = stats.TamCanh;
        idLinhCan = (int)stats.LinhCan;
        ability = stats.Ability;
        danDoc = stats.DanDoc;
        tuoiTac = stats.TuoiTac.Days / 365;
    }
}