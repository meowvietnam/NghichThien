using CreateCharacter;
using CreateCharacter.CharStats;
using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    public SecureFloat percentKim;
    public SecureFloat percentMoc;
    public SecureFloat percentThuy;
    public SecureFloat percentHoa;
    public SecureFloat percentTho;
    public SecureFloat percentMa;

    public SecureInt tuChat;
    public SecureInt ngoTinh;
    public SecureInt thanThuc;
    public SecureInt tocDoBay;
    public SecureInt money;
    public SecureInt hp;
    public SecureInt hpMax;
    public SecureInt thoNguyen;
    public SecureInt tamCanh;
    public SecureInt idLinhCan;
    public Ability ability;
    public SecureInt ngoDaoDiem;
    public SecureInt danDoc;

    public string nameUser;
    public SecureInt level;
    public SecureInt tuoiTac;
    public SecureInt currentExp;

    public SecureInt[] ngoDaoSaves;
    public DataSave[] camNgoSaves;

    public class DataSave
    {
        public SecureInt id;
        public SecureInt value;
    }

    // tông môn
    public SecureInt chucVi;

    public SecureInt monPhai;

    // skill
    public SecureInt[] thanThongOwner;
    public SecureInt[] thanThongEquip;

    public DataSave[] congPhapOwner;

    // công pháp used
    // -1 hoặc là 0 là chưa trang bị
    public SecureInt donThuatUsed;
    public SecureInt phuTu1Used;
    public SecureInt phuTu2Used;
    public SecureInt phuTu3Used;
    public SecureInt phuTu4Used;
    public SecureInt chuTu1Used;
    public SecureInt chuTu2Used;

    public CharacterStats Clone()
    {
        return (CharacterStats)MemberwiseClone();
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
        money = stats.LinhThach;
        hp = stats.Hp;
        hpMax = stats.SinhMenh;
        thoNguyen = stats.ThoNguyen;
        tamCanh = stats.TamCanh;
        idLinhCan = (int)stats.LinhCan;
        ability = stats.Ability;
        danDoc = stats.DanDoc;
        tuoiTac = stats.TuoiTac;
    }
}