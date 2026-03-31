using CreateCharacter;
using Sirenix.OdinInspector;
using UnityEngine;

public class NPCData : ScriptableObject
{
    private static string[] Surnames = {
        "Diệp", "Tiêu", "Lâm", "Sở", "Hàn", "Thần", "Lục", "Tô", "Tần", "Mặc",
        "Cố", "Thẩm", "Phương", "Bạch", "Long", "Phượng", "Vân", "Tống", "Khương", "Vũ",
        "Trình", "Bùi", "Liễu", "Tạ", "Viên", "Dương", "Chu", "Phó", "Trang", "Hoắc"
    };

    private static string[] MiddleNames = {
        "Thiên", "Thanh", "Vũ", "Vân", "Nhược", "Tuyết", "Mặc", "Tử", "Minh", "Tinh",
        "Linh", "Trường", "Nhất", "Cửu", "Vô", "Thái", "Mộng", "Kiếm", "Ẩn", "Dật",
        "Tĩnh", "Thần", "Hạo", "Giai", "Trường", "Tuyên", "Đông", "Tây", "Bắc", "Nam"
    };

    private static string[] LastNames = {
        "Phàm", "Phong", "Trần", "Hiên", "Tuyệt", "Dao", "Thần", "Vũ", "Nguyệt", "Kiếm",
        "Tâm", "Viêm", "Tôn", "Hoàng", "Không", "Ảnh", "Sát", "Ngạo", "Thương", "Lỗi",
        "Sương", "Yên", "Tịch", "Hùng", "Hải", "Sơn", "Lâm", "Hỏa", "Thổ", "Kim"
    };

    public static string GenerateName()
    {
        string ho = Surnames[Random.Range(0, Surnames.Length)];
        string dem = MiddleNames[Random.Range(0, MiddleNames.Length)];
        string ten = LastNames[Random.Range(0, LastNames.Length)];

        // Tỉ lệ 30% tên chỉ có 2 chữ (Họ + Tên) cho đa dạng
        if (Random.value > 0.7f)
        {
            return ho + " " + ten;
        }

        return ho + " " + dem + " " + ten;
    }
    [SerializeField] public CharacterStatsSecure charStats;
    [SerializeField] public CharacterStatsSecure charStatsCreateRef;
    [SerializeField] public ComboSkillResource.ComboWithType comboChar;
    [SerializeField] public ComboSkillResource.Combo CurrentCombo => comboChar.comboWithDaiCanhGioi[ThanThong.GetDaiCanhGioi(charStats.level)-1];
    [Button]
    public void LogData()
    {
        Debug.Log(charStats.percentKim.Value);
        Debug.Log("skin tao " + charStats.Skins.Count);
    }
   
    [Button]
    public void SetBaseStats(GameConfig gameConfig)
    {
        if(charStatsCreateRef.skinList.Count <= 0)
        {
            var skinRandom = CharacterGraphic.RandomSkinStatic(charStatsCreateRef.Gender, Random.Range(0, 2) == 0);
            foreach (var kvp in skinRandom.skins)
            {
                charStatsCreateRef.skinList.Add(new CharacterStatsSecure.SkinData { key = kvp.Key,value  = kvp.Value});
            }
            foreach (var kvp in skinRandom.colors)
            {
                charStatsCreateRef.colorList.Add(new CharacterStatsSecure.ColorData { key = kvp.Key, value = kvp.Value });
            }
        }    
        if(string.IsNullOrEmpty(charStatsCreateRef.nameUser))
        {
            charStatsCreateRef.nameUser = GenerateName();

        }

        charStats = charStatsCreateRef.Clone();
        for (int i = 0; i < charStats.level; i++)
        {
            charStats.hpMax += gameConfig.inforLevels[i].hpMaxBuff;
            charStats.hp = charStats.hpMax;
            charStats.thanThuc += gameConfig.inforLevels[i].thanThucBuff;
            charStats.tocDoBay += gameConfig.inforLevels[i].donTocBuff;
            charStats.thoNguyen += gameConfig.inforLevels[i].thoNguyenBuff;

        }
        int daiCanhGioi = ThanThong.GetDaiCanhGioi(charStats.level);
        int minRandom = 0;
        int maxRandom = 0;
        if(daiCanhGioi == 1)
        {
            minRandom = 15;
            maxRandom = 65;

        }
        else if (daiCanhGioi == 2)
        {
            minRandom = 35;
            maxRandom = 125;

        }
        else if (daiCanhGioi == 3)
        {
            minRandom = 65;
            maxRandom = 150;


        }
        else if (daiCanhGioi == 4)
        {
            minRandom = 100;
            maxRandom = 200;

        }
        else if (daiCanhGioi == 5)
        {
            minRandom = 500;
            maxRandom = 1000;

        }
        charStats.tuoiTac = Random.Range(minRandom, maxRandom);
        charStats.ngoDaoSaves.Clear();
        charStats.congPhapOwner.Clear();
        charStats.thanThongOwner.Clear(); 
        charStats.thanThongEquip.Clear();
        foreach (var item in CurrentCombo.ngoDaos)
        {
            charStats.ngoDaoSaves.Add(item.Id);
        }
        foreach (var item in CurrentCombo.congPhaps)
        {
            charStats.congPhapOwner.Add(new CharacterStatsSecure.DataSave { id = item.Id , value = 5});
            

        }
        foreach (var item in CurrentCombo.thanThongs)
        {
            charStats.thanThongEquip.Add(item.Id);
            charStats.thanThongOwner.Add(item.Id);
        }
        charStats.chuTu1Used = charStats.congPhapOwner.Count > 0 ? charStats.congPhapOwner[0].id : -1;
        charStats.phuTu1Used = charStats.congPhapOwner.Count > 1 ? charStats.congPhapOwner[1].id : -1;
        charStats.phuTu2Used = charStats.congPhapOwner.Count > 2 ? charStats.congPhapOwner[2].id : -1;
        charStats.phuTu3Used = charStats.congPhapOwner.Count > 3 ? charStats.congPhapOwner[3].id : -1;
        charStats.phuTu4Used = charStats.congPhapOwner.Count > 4 ? charStats.congPhapOwner[4].id : -1;
        charStats.chuTu2Used = charStats.congPhapOwner.Count > 5 ? charStats.congPhapOwner[5].id : -1;
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this); // Nếu charStatsCreateRef nằm trong script này
        UnityEditor.AssetDatabase.SaveAssets(); // Lưu lại file vật lý
#endif
    }
}
