using UnityEngine;

[System.Serializable]
public class CharacterStats
{    // chỉ số
    public SecureFloat percentKim;
    public SecureFloat percentMoc;
    public SecureFloat percentThuy;
    public SecureFloat percentHoa;
    public SecureFloat percentTho;
    public SecureFloat percentMa;

    public SecureInt level;
    public SecureInt currentExp;
    public SecureFloat hp;
    public SecureFloat hpMax;
    public string nameUser;
    public SecureInt tuoiTac;
    public SecureInt thoNguyen;
    public SecureInt tuChat;
    public SecureInt thanThuc;
    public SecureInt ngoTinh;
    public SecureInt tocDoBay;
    public SecureInt tamCanh;
    public SecureInt danDoc;
    public SecureInt money;
    public SecureInt idLinhCan;
    // tông môn
    public SecureInt chucVi;
    public SecureInt monPhai;
    // skill
    public SecureInt[] thanThongOwner;
    public SecureInt[] thanThongEquip;
    public class CongPhapSave
    {
        public SecureInt id;
        public SecureInt level;
    }
    public CongPhapSave[] congPhapOwner;
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
        return (CharacterStats)this.MemberwiseClone();
    }



}
