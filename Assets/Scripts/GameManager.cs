using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using CreateCharacter.CharStats;
using LuyenDan.Inventory;
using Network;
using System.Collections;
using Sirenix.Utilities;
using LuyenKhi;
using AllMaps;
using Network.Models;
using Newtonsoft.Json.Linq;
using CongPhap = Skill.CongPhap;
using ThanThong = Skill.ThanThong;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : BaseBehaviour
{
    [SerializeField] private List<string> listSceneLoaded = new List<string>
    {
        "AllMaps",
    };

    [SerializeField] public EMapType currentEMapType = EMapType.None;
    public EMonPhai CurrentEMonPhai => ConvertEMapToEMonPhai(currentEMapType);

    public static Dictionary<EMapType, string> dictMapTypeWithSceneName = new Dictionary<EMapType, string>()
    {
        { EMapType.None, "" },
        { EMapType.KimHongKiemPhai, "S3" },
        { EMapType.TrucSonTong, "S1" },
        { EMapType.TinhHaKiemPhai, "S4" },
        { EMapType.LyHoaMon, "S2" },
        { EMapType.HoaTranGiao, "S5" },
        { EMapType.BachDeLau, "S19" },
        { EMapType.VanTichThanh, "S22" },
        { EMapType.ThienTinhThanh, "S23" },
        { EMapType.DatPhongThanh, "S20" },
        { EMapType.QuangLangThanh, "S29" },
        { EMapType.ThienCoCac, "S21" },
    };

    public static Dictionary<EMapType, string> dictMapTypeWithName = new Dictionary<EMapType, string>()
    {
        { EMapType.None, "" },
        { EMapType.KimHongKiemPhai, "Kim Hồng Kiếm Phái" },
        { EMapType.TrucSonTong, "Trúc Sơn Tông" },
        { EMapType.TinhHaKiemPhai, "Tinh Hà Kiếm Phái" },
        { EMapType.LyHoaMon, "Ly Hỏa Môn" },
        { EMapType.HoaTranGiao, "Hóa Trần Giáo" },
        { EMapType.BachDeLau, "Bạch Đế Lâu" },
        { EMapType.VanTichThanh, "Vân Tịch Thành" },
        { EMapType.ThienTinhThanh, "Thiên Tinh Thành" },
        { EMapType.DatPhongThanh, "Dật Phong Thành" },
        { EMapType.QuangLangThanh, "Quảng Lăng Thành" },
        { EMapType.ThienCoCac, "Thiên Cơ Các" },
    };

    public static (string mySelf, string myCallTarget, string targetCallMe) XungHoText(int targetLevel,
        Gender targetGender)
    {
        var charData = Client.Instance.CharacterData;
        var myGender = charData.Gender;
        int myLevel = charData.Progression.Level;
        int levelDiff = myLevel - targetLevel;

        // --- CASE 0: Bằng vai phải lứa ---
        if (levelDiff == 0)
            return ("Ta", "Đạo hữu", "Đạo hữu");

        // --- CASE 1: Chênh lệch trong vòng 1-2 đại cảnh giới (1 đến 6 lv) ---
        if (Math.Abs(levelDiff) <= 6)
        {
            if (levelDiff > 0) // Mình là bề trên (Huynh/Tỷ)
            {
                string mySelf = (myGender == Gender.Female) ? "Sư tỷ" : "Sư huynh";
                string myCallTarget = (targetGender == Gender.Female) ? "Sư muội" : "Sư đệ";
                string targetCallMe = mySelf; // Đối phương phải gọi mình là Sư huynh/tỷ
                return (mySelf, myCallTarget, targetCallMe);
            }
            else // Mình là bề dưới (Đệ/Muội)
            {
                string mySelf = (myGender == Gender.Female) ? "Sư muội" : "Sư đệ";
                string myCallTarget = (targetGender == Gender.Female) ? "Sư tỷ" : "Sư huynh";
                string targetCallMe = mySelf; // Đối phương gọi mình là Sư đệ/muội
                return (mySelf, myCallTarget, targetCallMe);
            }
        }

        // --- CASE 2: Chênh lệch lớn (>= 7 lv) ---
        if (levelDiff >= 7) // Mình là Tiền bối
        {
            string mySelf = "Bổn tọa"; // Hoặc "Lão phu/Bổn tọa" nếu muốn ngầu
            string myCallTarget = (targetGender == Gender.Female) ? "Tiểu cô nương" : "Tiểu tử";
            string targetCallMe = "Tiền bối";
            return (mySelf, myCallTarget, targetCallMe);
        }
        else // Mình là Hậu bối (levelDiff <= -7)
        {
            string mySelf = "Vãn bối";
            string myCallTarget = "Tiền bối";
            string targetCallMe = (myGender == Gender.Female) ? "Tiểu cô nương" : "Tiểu tử";
            return (mySelf, myCallTarget, targetCallMe);
        }
    }

    public static Dictionary<LinhCan, string> tuChatWithText = new Dictionary<LinhCan, string>
    {
        { LinhCan.NguLinhCan, "Tư chất ngu dốt" }, // Use { key, value }
        { LinhCan.TuLinhCan, "Tư chất thông thường" },
        { LinhCan.TamLinhCan, "Nhạy bén thông minh" },
        { LinhCan.SongLinhCan, "Khoáng thế kỳ tài" },
        { LinhCan.ThienLinhCan, "Tài năng ngút trời" }
    };

    public static Dictionary<EMonPhai, string> monPhaiText = new Dictionary<EMonPhai, string>
    {
        { EMonPhai.None, "Tán Tu" }, // Use { key, value }
        { EMonPhai.BachDeLau, "Bạch Đế Lâu" },
        { EMonPhai.KimHongKiemPhai, "Kim Hồng Kiếm Phái" },
        { EMonPhai.TrucSonTong, "Trúc Sơn Tông" },
        { EMonPhai.TinhHaKiemPhai, "Tinh Hà Kiếm Phái" },
        { EMonPhai.LyHoaMon, "Ly Hỏa Môn" },
        { EMonPhai.HoaTranGiao, "Hóa Trần Giáo" },
    };

    public static Dictionary<int, string> chucVuText = new Dictionary<int, string>
    {
        { 1, "Ngoại Môn" }, // Use { đại cảnh giới, text }
        { 2, "Nội Môn" },
        { 3, "Trưởng Lão" },
        { 4, "Đại Trưởng Lão" },
        { 5, "Thái Thượng Trưởng Lão" },
    };

    public static Dictionary<int, string> levelWithText = new Dictionary<int, string>
    {
        // Luyện Khí
        { 1, "Luyện Khí Sơ Kì" },
        { 2, "Luyện Khí Trung Kì" },
        { 3, "Luyện Khí Hậu Kì" },

        // Trúc Cơ
        { 4, "Trúc Cơ Sơ Kì" },
        { 5, "Trúc Cơ Trung Kì" },
        { 6, "Trúc Cơ Hậu Kì" },

        // Kim Đan
        { 7, "Kim Đan Sơ Kì" },
        { 8, "Kim Đan Trung Kì" },
        { 9, "Kim Đan Hậu Kì" },

        // Nguyên Anh
        { 10, "Nguyên Anh Sơ Kì" },
        { 11, "Nguyên Anh Trung Kì" },
        { 12, "Nguyên Anh Hậu Kì" },

        // Hóa Thần
        { 13, "Hóa Thần Sơ Kì" },
        { 14, "Hóa Thần Trung Kì" },
        { 15, "Hóa Thần Hậu Kì" },
    };

    public static EMonPhai ConvertEMapToEMonPhai(EMapType eMap)
    {
        switch (eMap)
        {
            case EMapType.None:
                return EMonPhai.None;
            case EMapType.KimHongKiemPhai:
                return EMonPhai.KimHongKiemPhai;
            case EMapType.TrucSonTong:
                return EMonPhai.TrucSonTong;
            case EMapType.TinhHaKiemPhai:
                return EMonPhai.TinhHaKiemPhai;
            case EMapType.LyHoaMon:
                return EMonPhai.LyHoaMon;
            case EMapType.HoaTranGiao:
                return EMonPhai.HoaTranGiao;
            case EMapType.BachDeLau:
                return EMonPhai.BachDeLau;
        }

        return EMonPhai.None;
    }

    public static Dictionary<BaseSkill.ETierSkill, int> tierSkillWithLevel = new Dictionary<BaseSkill.ETierSkill, int>
    {
        { BaseSkill.ETierSkill.NhanGiaiHa, 1 },
        { BaseSkill.ETierSkill.NhanGiaiTrung, 3 },
        { BaseSkill.ETierSkill.NhanGiaiThuong, 6 },
        { BaseSkill.ETierSkill.DiaGiaiHa, 8 },
        { BaseSkill.ETierSkill.DiaGiaiTrung, 10 },
        { BaseSkill.ETierSkill.DiaGiaiThuong, 12 },
        { BaseSkill.ETierSkill.ThienGiaiHa, 13 },
        { BaseSkill.ETierSkill.ThienGiaiTrung, 14 },
        { BaseSkill.ETierSkill.ThienGiaiThuong, 15 },
    };

    public static Dictionary<Tier, int> tierItemWithLevel = new Dictionary<Tier, int>
    {
        { Tier.NhatPham, 1 },
        { Tier.NhiPham, 3 },
        { Tier.TamPham, 5 },
        { Tier.TuPham, 8 },
        { Tier.NguPham, 11 },
        { Tier.LucPham, 15 },
    };

    public static Dictionary<EquipmentTier, int> tierEquipmentWithLevel = new Dictionary<EquipmentTier, int>
    {
        { EquipmentTier.PhuKhi, 3 },
        { EquipmentTier.PhapKhi, 6 },
        { EquipmentTier.PhapBao, 9 },
        { EquipmentTier.ThuanDuong, 12 },
        { EquipmentTier.ThongThien, 15 },
    };

    public static Dictionary<int, ETypeVisualChar> levelWithVisualChar = new Dictionary<int, ETypeVisualChar>
    {
        { 1, ETypeVisualChar.EnemyLuyenKhiSoKi }, // Use { đại cảnh giới, text }
        { 2, ETypeVisualChar.EnemyLuyenKhiTrungKi },
        { 3, ETypeVisualChar.EnemyLuyenKhiHauKi },
        { 4, ETypeVisualChar.EnemyTrucCoSoKi },
        { 5, ETypeVisualChar.EnemyTrucCoTrungKi },
        { 6, ETypeVisualChar.EnemyTrucCoHauKi },
        { 7, ETypeVisualChar.EnemyKetDanSoKi },
        { 8, ETypeVisualChar.EnemyKetDanTrungKi },
        { 9, ETypeVisualChar.EnemyKetDanHauKi },
        { 10, ETypeVisualChar.EnemyNguyenAnhSoKi },
        { 11, ETypeVisualChar.EnemyNguyenAnhTrungKi },
        { 12, ETypeVisualChar.EnemyNguyenAnhHauKi },

        { 13, ETypeVisualChar.EnemyKetDanHauKi },
        { 14, ETypeVisualChar.EnemyNguyenAnhSoKi },
        { 15, ETypeVisualChar.EnemyTrucCoHauKi },
    };

    [Serializable]
    public class InforLevel
    {
        public int tuviRequireNextLevel;
        public int hpMaxBuff;
        public int thanThucBuff;
        public int donTocBuff;
        public int thoNguyenBuff;
    }

    public InforLevel[] inforLevels = new InforLevel[]
    {
        new InforLevel
            { tuviRequireNextLevel = 20000, hpMaxBuff = 15, thanThucBuff = 1, donTocBuff = 1, thoNguyenBuff = 0 },
        new InforLevel
            { tuviRequireNextLevel = 50000, hpMaxBuff = 15, thanThucBuff = 1, donTocBuff = 1, thoNguyenBuff = 0 },
        new InforLevel
            { tuviRequireNextLevel = 144000, hpMaxBuff = 80, thanThucBuff = 2, donTocBuff = 2, thoNguyenBuff = 100 },
        new InforLevel
            { tuviRequireNextLevel = 400000, hpMaxBuff = 40, thanThucBuff = 2, donTocBuff = 2, thoNguyenBuff = 0 },
        new InforLevel
            { tuviRequireNextLevel = 800000, hpMaxBuff = 40, thanThucBuff = 2, donTocBuff = 2, thoNguyenBuff = 0 },
        new InforLevel
            { tuviRequireNextLevel = 1200000, hpMaxBuff = 300, thanThucBuff = 8, donTocBuff = 8, thoNguyenBuff = 300 },
        new InforLevel
            { tuviRequireNextLevel = 1577000, hpMaxBuff = 150, thanThucBuff = 4, donTocBuff = 4, thoNguyenBuff = 0 },
        new InforLevel
            { tuviRequireNextLevel = 3211000, hpMaxBuff = 150, thanThucBuff = 4, donTocBuff = 4, thoNguyenBuff = 0 },
        new InforLevel
        {
            tuviRequireNextLevel = 6480000, hpMaxBuff = 800, thanThucBuff = 16, donTocBuff = 16, thoNguyenBuff = 500
        },
        new InforLevel
            { tuviRequireNextLevel = 19440000, hpMaxBuff = 400, thanThucBuff = 8, donTocBuff = 8, thoNguyenBuff = 0 },
        new InforLevel
            { tuviRequireNextLevel = 38880000, hpMaxBuff = 400, thanThucBuff = 8, donTocBuff = 8, thoNguyenBuff = 0 },
        new InforLevel
        {
            tuviRequireNextLevel = 77760000, hpMaxBuff = 8000, thanThucBuff = 32, donTocBuff = 32, thoNguyenBuff = 1000
        },
        new InforLevel
        {
            tuviRequireNextLevel = 94422000, hpMaxBuff = 4000, thanThucBuff = 16, donTocBuff = 16, thoNguyenBuff = 0
        },
        new InforLevel
        {
            tuviRequireNextLevel = 144411000, hpMaxBuff = 4000, thanThucBuff = 16, donTocBuff = 16, thoNguyenBuff = 0
        },
        new InforLevel
            { tuviRequireNextLevel = 200000000, hpMaxBuff = 0, thanThucBuff = 0, donTocBuff = 0, thoNguyenBuff = 0 }
    };

    public static string GetChucVuText(EMonPhai monPhai, int level)
    {
        return monPhaiText[monPhai] + " " + chucVuText[ThanThong.GetDaiCanhGioi(level)];
    }

    public void OnBackScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        int index = listSceneLoaded.IndexOf(currentScene);
        if (index == 0)
        {
            currentEMapType = EMapType.None;
            return;
        }

        string backScene = listSceneLoaded[index - 1];
        listSceneLoaded.Remove(currentScene);
        SceneManager.LoadScene(backScene);
    }

    public void OnLoadScene(params object[] data)
    {
        string nameScene = (string)data[0];
        if (string.IsNullOrEmpty(nameScene))
        {
            System.Random _random = new System.Random();
            Array values = Enum.GetValues(typeof(Gender));
            Gender randomGender = (Gender)values.GetValue(_random.Next(values.Length));
            var xungHo = XungHoText(6, randomGender);
            SayDialogManager.Instance.ShowDialogRandomSkin("Người Gác Cổng", randomGender,
                $"{xungHo.targetCallMe} xin dừng bước hiện tại nơi này chưa thể vào hãy kiên nhẫn chờ bản cập nhật tiếp theo !");
            return;
        }

        if (data.Length > 1)
        {
            EMapType eMap = (EMapType)data[1];
            currentEMapType = eMap;
        }

        var sceneManger = Client.Instance.CoreModule.GetHandler<Network.Core.SceneManager>();
        sceneManger.ChangeScene(nameScene, OnChangeSceneResult);
    }

    private void OnChangeSceneResult(ChangeSceneResult result)
    {
        NetworkManager.Instance.Invoke(() =>
        {
            if (result.Success)
            {
                ChangeSceneSuccess(result.SceneName);
            }
            else
            {
                var notify = NotifyGroup.Instance;
                if (notify != null)
                {
                    notify.Show(result.Message);
                }
            }
        });
    }

    private void ChangeSceneSuccess(string nameScene)
    {
        listSceneLoaded.Add(nameScene);
        SceneManager.LoadScene(nameScene);
    }

    public class CharCombatInit
    {
        public ETypeVisualChar eTypeVisual;
        public CharacterStatsSecure charStats;
    }

    public CharCombatInit enemyCombat = null;

    public void GoToCombat(CharCombatInit enemyCombat)
    {
        this.enemyCombat = enemyCombat;
        if (enemyCombat == null) return;
        Pool.Get<UIManager>().panelGroup.SetActive(false);
        SceneManager.LoadScene("Combat");
    }

    public void BackAllMaps()
    {
        Pool.Get<UIManager>().panelGroup.SetActive(true);
        SceneManager.LoadScene(listSceneLoaded[0]);
    }

    public static CharacterStatsSecure ConvertCharServerDataToCharClientData()
    {
        var charData = Client.Instance.CharacterData;
        var stat = charData.Stats;
        var vuKhiData = charData.InventoryManager.VuKhis.FirstOrDefault();
        var phongCuData = charData.InventoryManager.PhongCus.FirstOrDefault();
        var trangSucData = charData.InventoryManager.TrangSucs.LastOrDefault(); // Dùng LastOrDefault cho gọn

        return new CharacterStatsSecure
        {
            nameUser = charData.Name,
            level = charData.Progression.Level,
            hp = stat.Hp,
            hpMax = stat.SinhMenh,
            ability = stat.Ability,
            percentKim = stat.Kim,  
            percentMoc = stat.Moc,
            percentThuy = stat.Thuy,
            percentHoa = stat.Hoa,
            percentTho = stat.Tho,
            percentMa = stat.Ma,
            tocDoBay = stat.TocDoBay,
            thanThuc = stat.ThanThuc,
            thoNguyen = stat.ThoNguyen,
            thanThongEquip = charData.EquipmentManager.ThanThongEquips.Select(x => new SecureInt(x.Id)).ToList(),
            chuTu1Used = charData.EquipmentManager.ChuTu1Used,
            chuTu2Used = charData.EquipmentManager.ChuTu2Used,
            phuTu1Used = charData.EquipmentManager.PhuTu1Used,
            phuTu2Used = charData.EquipmentManager.PhuTu2Used,
            phuTu3Used = charData.EquipmentManager.PhuTu3Used,
            phuTu4Used = charData.EquipmentManager.PhuTu4Used,
            monPhai = charData.MonPhai,
            danDoc = stat.DanDoc,
            ngoDaoSaves = charData.InventoryManager.NgoDaos.Select(x => new SecureInt(x.Id)).ToList(),
            skinList = charData.Appearance.Skins.Select(kvp => new CharacterStatsSecure.SkinData
                { key = kvp.Key, value = kvp.Value }).ToList(),
            colorList = charData.Appearance.Colors.Select(kvp => new CharacterStatsSecure.ColorData
                { key = kvp.Key, value = kvp.Value }).ToList(),

            // Nếu vuKhiData null, nguyên cả cụm vukhi sẽ là null
            vuKhi = vuKhiData == null ? new EquipmentClient() : new EquipmentClient
            {
                id = vuKhiData.Id,
                nameEquip = vuKhiData.Name,
                listEffectEquip = vuKhiData.Effects?.Select(effect => new EquipmentClient.EffectClient
                {
                    Type = effect.Type,
                    Value = new SecureInt(effect.Value)
                }).ToList() ?? new List<EquipmentClient.EffectClient>()
            },

            // Tương tự cho phongCu
            phongCu = phongCuData == null ? new EquipmentClient() : new EquipmentClient
            {
                id = phongCuData.Id,
                nameEquip = phongCuData.Name,
                listEffectEquip = phongCuData.Effects?.Select(effect => new EquipmentClient.EffectClient
                {
                    Type = effect.Type,
                    Value = new SecureInt(effect.Value)
                }).ToList() ?? new List<EquipmentClient.EffectClient>()
            },

            // Tương tự cho trangSuc
            trangSuc = trangSucData == null ? new EquipmentClient() : new EquipmentClient
            {
                id = trangSucData.Id,
                nameEquip = trangSucData.Name,
                listEffectEquip = trangSucData.Effects?.Select(effect => new EquipmentClient.EffectClient
                {
                    Type = effect.Type,
                    Value = new SecureInt(effect.Value)
                }).ToList() ?? new List<EquipmentClient.EffectClient>()
            }

        };
    }

    private void Awake()
    {
        // Kiểm tra nếu trong Pool đã có một Manager loại này rồi
        var existing = Pool.Get<GameManager>();

        if (existing != null && existing != this)
        {
            // Nếu đã có bản cũ (từ Scene trước), xóa bản mới này đi
            Destroy(gameObject);
            return;
        }

        // Nếu chưa có, mình là "trùm", đăng ký vào Pool và giữ lại
        Pool.Set(this);
        DontDestroyOnLoad(gameObject);
        Observer.AddObserver(EventList.GamePlay.LOAD_SCENE, OnLoadScene);
        Observer.AddObserver(EventList.GamePlay.BACK_SCENE, OnBackScene);
    }

    public ThanThong[] thanThongs;
    public CongPhap[] congPhaps;

    public ThanThong GetThanThongWithId(int id)
    {
        return thanThongs.FirstOrDefault(x => x.Id == id);
    }

    public CongPhap GetCongPhapWithId(int id)
    {
        return congPhaps.FirstOrDefault(x => x.Id == id);
    }

    public ThanThong FindThanThongWithItemRef(ItemRef itemRef)
    {
        if (itemRef.Type == ItemType.ThanThong)
        {
            return GetThanThongWithId(itemRef.Id);
        }

        return null;
    }

    public CongPhap FindCongPhapWithItemRef(ItemRef itemRef)
    {
        if (itemRef.Type == ItemType.CongPhap)
        {
            return GetCongPhapWithId(itemRef.Id);
        }

        return null;
    }

    public static Dictionary<Tier, string> TextWithTier = new Dictionary<Tier, string>
    {
        { Tier.NhatPham, "Nhất Phẩm" },
        { Tier.NhiPham, "Nhị Phẩm" },
        { Tier.TamPham, "Tam Phẩm" },
        { Tier.TuPham, "Tứ Phẩm" },
        { Tier.NguPham, "Ngũ Phẩm" },
        { Tier.LucPham, "Lục Phẩm" },
    };
    public static Dictionary<EquipmentTier, string> TextWithTierEquirement = new Dictionary<EquipmentTier, string>
    {
        { EquipmentTier.PhuKhi, "Phù Khí" },
        { EquipmentTier.PhapKhi, "Pháp Khí" },
        { EquipmentTier.PhapBao, "Pháp Bảo" },
        { EquipmentTier.ThuanDuong, "Thuần Dương" },
        { EquipmentTier.ThongThien, "Thông Thiên" },
    };
    public static Dictionary<Tier, int> PriceTienNgocVatLieuAndHerbWithTier = new Dictionary<Tier, int>
    {
        { Tier.NhatPham, 70 },
        { Tier.NhiPham, 60 },
        { Tier.TamPham, 50 },
        { Tier.TuPham, 40 },
        { Tier.NguPham, 30 },
        { Tier.LucPham, 20 },
    };

    public static Dictionary<Tier, int> PriceVatLieuWithTier = new Dictionary<Tier, int>
    {
        { Tier.NhatPham, 4 },
        { Tier.NhiPham, 36 },
        { Tier.TamPham, 265 },
        { Tier.TuPham, 2285 },
        { Tier.NguPham, 22912 },
        { Tier.LucPham, 256608 },
    };

    public static Dictionary<Tier, int> PriceHerbWithTier = new Dictionary<Tier, int>
    {
        { Tier.NhatPham, 3 },
        { Tier.NhiPham, 12 },
        { Tier.TamPham, 135 },
        { Tier.TuPham, 1440 },
        { Tier.NguPham, 13500 },
        { Tier.LucPham, 81000 },
    };

    public static Dictionary<Tier, int> PriceFurnaceWithTier = new Dictionary<Tier, int>
    {
        { Tier.NhatPham, 200 },
        { Tier.NhiPham, 600 },
        { Tier.TamPham, 2000 },
        { Tier.TuPham, 5000 },
        { Tier.NguPham, 7500 },
        { Tier.LucPham, 10000 },
    };

    public static Dictionary<BaseSkill.ETierSkill, int> PriceTTWithTier = new Dictionary<BaseSkill.ETierSkill, int>
    {
        { BaseSkill.ETierSkill.NhanGiaiHa, 25 },
        { BaseSkill.ETierSkill.NhanGiaiTrung, 50 },
        { BaseSkill.ETierSkill.NhanGiaiThuong, 75 },
        { BaseSkill.ETierSkill.DiaGiaiHa, 600 },
        { BaseSkill.ETierSkill.DiaGiaiTrung, 800 },
        { BaseSkill.ETierSkill.DiaGiaiThuong, 1000 },
        { BaseSkill.ETierSkill.ThienGiaiHa, 18000 },
        { BaseSkill.ETierSkill.ThienGiaiTrung, 30000 },
        { BaseSkill.ETierSkill.ThienGiaiThuong, 50000 },
    };

    public static Dictionary<BaseSkill.ETierSkill, int> PriceCPWithTier = new Dictionary<BaseSkill.ETierSkill, int>
    {
        { BaseSkill.ETierSkill.NhanGiaiHa, 50 },
        { BaseSkill.ETierSkill.NhanGiaiTrung, 75 },
        { BaseSkill.ETierSkill.NhanGiaiThuong, 100 },
        { BaseSkill.ETierSkill.DiaGiaiHa, 2000 },
        { BaseSkill.ETierSkill.DiaGiaiTrung, 3000 },
        { BaseSkill.ETierSkill.DiaGiaiThuong, 4000 },
        { BaseSkill.ETierSkill.ThienGiaiHa, 240000 },
        { BaseSkill.ETierSkill.ThienGiaiTrung, 400000 },
        { BaseSkill.ETierSkill.ThienGiaiThuong, 600000 },
    };

    public static int GetPriceItemSell(ItemRef itemRef, IItemData data)
    {
        int price = 0;
        switch (itemRef.Type)
        {
            case ItemType.Herb:
                price = PriceHerbWithTier[data.Tier];
                break;
            case ItemType.Furnace:
                int basePrice = PriceFurnaceWithTier[data.Tier];
                int currentDoBen = Client.Instance.CharacterData.InventoryManager.Furnaces
                    .FirstOrDefault(x => x.Id == itemRef.Id)!.Durability;
                price = basePrice * currentDoBen / 100;

                break;
            case ItemType.LinhDan:
                if (data != null && data is LinhDan linhDan)
                {
                    price = linhDan.PriceV130;
                }

                break;
            case ItemType.ThanThong:
                price = PriceTTWithTier[Pool.Get<GameManager>().GetThanThongWithId(itemRef.Id).eTierSkill];

                break;
            case ItemType.CongPhap:
                price = PriceCPWithTier[Pool.Get<GameManager>().GetCongPhapWithId(itemRef.Id).eTierSkill];

                break;
            case ItemType.VuKhi:
                break;
            case ItemType.PhongCu:
                break;
            case ItemType.TrangSuc:
                break;
            case ItemType.Recipe:
                if (data != null && data is RecipeData recipe)
                {
                    price = recipe.LinhDan.PriceV130 * 5;
                }

                break;
            default:
                break;
        }

        return price;
    }

    public static int GetPriceItemBuy(ItemRef itemRef, IItemData data)
    {
        return GetPriceItemSell(itemRef, data) * 3;
    }

    [Button]
    private void OnUpdateDataThanThong()
    {
#if UNITY_EDITOR
        // Tìm tất cả GUID của các asset có type là ThanThong
        string[] guids = AssetDatabase.FindAssets("t:ThanThong");

        thanThongs = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<ThanThong>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(asset => asset != null)
            .ToArray();

        EditorUtility.SetDirty(this); // Đánh dấu object đã thay đổi để Unity lưu lại
        Debug.Log($"Đã cập nhật {thanThongs.Length} ThanThong vào mảng.");
#endif
    }

    [Button]
    private void OnUpdateDataCongPhap()
    {
#if UNITY_EDITOR
        // Tìm tất cả GUID của các asset có type là CongPhap
        string[] guids = AssetDatabase.FindAssets("t:CongPhap");

        congPhaps = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<CongPhap>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(asset => asset != null)
            .ToArray();

        EditorUtility.SetDirty(this);
        Debug.Log($"Đã cập nhật {congPhaps.Length} CongPhap vào mảng.");
#endif
    }


    public static string[] tamCanh = new string[]
    {
        "Tâm phiền ý loạn",
        "Thần tĩnh khí an",
        "Không một gợn sóng",
        "Tâm tình yên lặng",
        "Khám phá hồng trần",
        "Cao siêu ngoại vật",
        "Vật ta hợp nhất",
    };

    public static int[] tamCanhRequireEnd = new int[]
    {
        0,
        49,
        199,
        599,
        999,
        1999,
    };

    public static string[] danDocText = new string[]
    {
        "Thân thể khỏe mạnh",
        "Thân thể không ngại",
        "Có chút khó chịu",
        "Gân mạch ngưng trệ",
        "Bệnh nguy kịch",
        "Độc phát thân vong",
    };

    public static int[] danDocRequireEnd = new int[]
    {
        20,
        50,
        70,
        100,
        120,
    };

    public static (string text, int nextMilestone, int index) GetDanDocValue(int currentDanDoc)
    {
        for (int i = 0; i < danDocRequireEnd.Length; i++)
        {
            if (currentDanDoc <= danDocRequireEnd[i])
            {
                return (danDocText[i], danDocRequireEnd[i], i);
            }
        }
        return (danDocText[danDocText.Length - 1], (int)currentDanDoc, danDocText.Length - 1);
    }
    public static (string name, int nextMilestone, int index) GetTamCanhValue(int currentPoint)
    {
        // Trường hợp đặc biệt: Nhỏ hơn 0
        if (currentPoint < 0)
            return (tamCanh[0], (int)0, 0);

        // Duyệt qua các mốc yêu cầu
        for (int i = 0; i < tamCanhRequireEnd.Length; i++)
        {
            // Nếu điểm hiện tại nằm trong khoảng từ mốc i đến mốc i+1
            if (currentPoint >= tamCanhRequireEnd[i] &&
                (i == tamCanhRequireEnd.Length - 1 || currentPoint < tamCanhRequireEnd[i + 1]))
            {
                // Trả về tên tương ứng (i + 1 vì 0 là dành cho < 0)
                // Và trả về mốc tiếp theo (nếu có)
                int nextLevel = (i + 1 < tamCanhRequireEnd.Length)
                    ? tamCanhRequireEnd[i + 1]
                    : (int)currentPoint; // Hoặc giá trị tối đa tùy bạn

                return (tamCanh[i + 1], nextLevel, i + 1);
            }
        }

        // Trường hợp cuối cùng: Lớn hơn mốc cao nhất (> 1999)
        return (tamCanh[tamCanh.Length - 1], (int)currentPoint, tamCanh.Length - 1);
    }

    public static int GetTimeWorldInt()
    {
        // 1. Lấy giá trị TimeSpan gốc
        TimeSpan originalTime = Client.Instance.CharacterData.Stats.TuoiTac;

        // 2. Chuyển đổi tổng số ngày sang đơn vị tháng
        // Giả sử quy ước: 1 tháng = 30 ngày
        int totalDays = (int)originalTime.TotalDays;
        int totalMonths = totalDays / 30;

        // 3. Trừ đi 15 năm gốc (15 năm * 12 tháng = 180 tháng)
        return totalMonths - 180;
    }

    public static TimeSpan GetTimeWorldTimeSpan()
    {
        return ConvertMonthToTimeSpan(GetTimeWorldInt());
    }
    public static int GetIntTuoiTac(TimeSpan timeSpanTuoiTac)
    {
        return (int)(timeSpanTuoiTac.TotalDays / 365);
    }
    public static TimeSpan ConvertMonthToTimeSpan(int months)
    {
        return TimeSpan.FromDays(months * 30);
    }
    // Hàm bổ trợ
    public static IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        // Ngăn không cho scene tự động kích hoạt khi load xong
        op.allowSceneActivation = false;

        // Chờ đợi theo thời gian bạn muốn
        yield return new WaitForSeconds(delay);

        // Đợi thêm cho đến khi scene thực sự load xong trong background (progress đạt 0.9)
        while (op.progress < 0.9f)
        {
            yield return null;
        }

        // Cuối cùng mới cho phép chuyển scene
        op.allowSceneActivation = true;
    }

    public static T GetItemPool<T>(T prefabItem) where T : Component
    {
        // Lấy Transform của cha để duyệt qua các con
        Transform parent = prefabItem.transform.parent;
        T item = null;
        // Duyệt từ index 1 (bỏ qua index 0 thường là chính cái prefab/template)
        for (int i = 1; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            // Nếu object con đang không hoạt động (đang rảnh trong pool)
            if (!child.gameObject.activeSelf)
            {
                item = child.GetComponent<T>();
                item.gameObject.SetActive(true);
                return item;
            }
        }

        item = Instantiate(prefabItem, parent);
        item.gameObject.SetActive(true);
        // Nếu không tìm thấy cái nào rảnh, khởi tạo cái mới
        return item;
    }
    public static string GetDescEquipment(IEnumerable<dynamic> effects, EquipmentType equipmentType)
    {
        // 1. Dòng đầu tiên: [Chủ động] hoặc [Bị động]
        string textDesc = GetDescEquipment(equipmentType);

        var listEffectTypeStartCombat = new HashSet<EffectType>
    {
        EffectType.KimTrong, EffectType.MocTrong, EffectType.ThuyTrong,
        EffectType.HoaTrong, EffectType.ThoTrong,
    };

        // Nhóm các effect dựa trên việc nó là Special hay không
        // Sau đó duyệt qua từng nhóm để thêm SubHeader một lần duy nhất cho nhóm đó
        var groupedEffects = effects
            .GroupBy(e => listEffectTypeStartCombat.Contains((EffectType)e.Type)) // Lưu ý ép kiểu nếu cần
            .OrderByDescending(g => g.Key); // Ưu tiên hiện "Bắt đầu chiến đấu" trước (tùy chọn)

        foreach (var group in groupedEffects)
        {
            bool isSpecial = group.Key;

            // 2. Lấy dòng tiền tố (Chỉ lấy 1 lần cho cả nhóm)
            string subHeader = GetDescEquipment(equipmentType, isSpecial);
            if (!string.IsNullOrEmpty(subHeader))
            {
                textDesc += "\n" + subHeader;
            }

            // 3. Duyệt qua các effect trong nhóm để nối nội dung chi tiết
            foreach (var effect in group)
            {
                string effectDetail = GetEffectDescription(effect);
                if (!string.IsNullOrEmpty(effectDetail))
                {
                    // Thêm một khoảng thụt đầu dòng (tùy chọn) để dễ nhìn hơn
                    textDesc += "\n - " + effectDetail;
                }
            }
        }

        return textDesc;
    }
  
    private static string GetEffectDescription(dynamic effect)
    {

        switch (effect.Type)
        {
            // --- NHÓM CASTER THU HOẠCH (BUFF/DEBUFF) ---
            case EffectType.Bui:
                return $"Thu hoạch được {effect.Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}.";
            case EffectType.ChanHoa:
            case EffectType.ChanKhi:
            case EffectType.ChanKiem:
            case EffectType.ChanKim:
            case EffectType.ChanMoc:
            case EffectType.ChanThan:
            case EffectType.ChanTho:
            case EffectType.ChanThuy:
            case EffectType.Diem:
            case EffectType.GiamTonThuong:
            case EffectType.HoaDiem:
            case EffectType.HoTrao:
            case EffectType.KiemKhi:
            case EffectType.NguHoa:
            case EffectType.NguHoaHiep:
            case EffectType.NguKhiHiep:
            case EffectType.NguKiemHiep:
            case EffectType.NguKim:
            case EffectType.NguKimHiep:
            case EffectType.NguMoc:
            case EffectType.NguMocHiep:
            case EffectType.NgungThan:
            case EffectType.NguTho:
            case EffectType.NguThoHiep:
            case EffectType.NguThuy:
            case EffectType.NguThuyHiep:
            case EffectType.PhanTonThuong:
            case EffectType.SucThe:
            case EffectType.TichDoc:
            case EffectType.TichHoa:
            case EffectType.TichKim:
            case EffectType.TichMoc:
            case EffectType.TichThuy:
                // Map ngược từ EffectType sang EBuffAndDeBuff để lấy Keyword
                EBuffAndDeBuff buffEnum = (EBuffAndDeBuff)System.Enum.Parse(typeof(EBuffAndDeBuff), effect.Type.ToString());
                return $"Thu hoạch được {effect.Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(buffEnum)}.";

            case EffectType.Nhanh:
                return $"Thu hoạch được {effect.Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TangToc)}.";
            case EffectType.TriLieuLieu:
                return $"Thu hoạch được {effect.Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)}.";
            case EffectType.NguThanHiep:
                return $"Thu hoạch được {effect.Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NguThanThucHiep)}.";

            // --- NHÓM ĐỐI THỦ NHẬN PHẢI (DEBUFF) ---
            case EffectType.Cat:
                return $"Đối thủ nhận phải {effect.Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cat)}.";
            case EffectType.ChayMau:
            case EffectType.DeTonThuong:
            case EffectType.PhaThe:
            case EffectType.SuongGia:
            case EffectType.ThanThuong:
            case EffectType.ThieuDot:
            case EffectType.TrungDoc:
                EBuffAndDeBuff debuffEnum = (EBuffAndDeBuff)System.Enum.Parse(typeof(EBuffAndDeBuff), effect.Type.ToString());
                return $"Đối thủ nhận phải {effect.Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(debuffEnum)}.";

            case EffectType.QuanQuanh:
                return $"Đối thủ nhận phải {effect.Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}.";
            case EffectType.TroiDu:
                return $"Đối thủ nhận phải {effect.Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TroiChan)}.";

            // --- NHÓM HẤP THỤ LINH KHÍ ---
            case EffectType.HapThuLinhKhi:
                return $"Hấp thụ ngẫu nhiên {effect.Value} Linh Khí.";
            case EffectType.HapThuHoaLinhKhi:
                return $"Hấp thụ {effect.Value} điểm Hỏa Linh Khí.";
            case EffectType.HapThuKimLinhKhi:
                return $"Hấp thụ {effect.Value} điểm Kim Linh Khí.";
            case EffectType.HapThuMocLinhKhi:
                return $"Hấp thụ {effect.Value} điểm Mộc Linh Khí.";
            case EffectType.HapThuThoLinhKhi:
                return $"Hấp thụ {effect.Value} điểm Thổ Linh Khí.";
            case EffectType.HapThuThuyLinhKhi:
                return $"Hấp thụ {effect.Value} điểm Thủy Linh Khí.";

            // --- NHÓM HỒI PHỤC ---
            case EffectType.KhoiPhucGiaTriSinhMenh:
            case EffectType.KhoiPhucGiaTriSinhMenhDauLuot:
                return $"Khôi phục {effect.Value} điểm HP.";

            // --- NHÓM SÁT THƯƠNG ---
            case EffectType.TaoThanhTonThuong:
                return $"Tạo thành {effect.Value} điểm tổn thương.";
            case EffectType.TaoThanhXLanXDiemThuongTon:
                return $"Tạo thành {effect.Value} lần {effect.Value} điểm tổn thương.";

            // --- NHÓM "TRONG" (START COMBAT) ---
            case EffectType.KimTrong:
                return $"Tăng {effect.Value}% tỉ lệ hấp thụ Kim linh khí.";

            case EffectType.MocTrong:
                return $"Tăng {effect.Value}% tỉ lệ hấp thụ Mộc linh khí.";

            case EffectType.ThuyTrong:
                return $"Tăng {effect.Value}% tỉ lệ hấp thụ Thủy linh khí.";

            case EffectType.HoaTrong:
                return $"Tăng {effect.Value}% tỉ lệ hấp thụ Hỏa linh khí.";

            case EffectType.ThoTrong:
                return $"Tăng {effect.Value}% tỉ lệ hấp thụ Thổ linh khí.";

            default:
                return "";
        }
    }
    public static string GetEffectDescription(EffectType type)
    {
        string suffix = " (X là giá trị ngẫu nhiên được luyện thành)";
        string description = "";

        switch (type)
        {
            // --- NHÓM CASTER THU HOẠCH (BUFF/DEBUFF) ---
            case EffectType.Bui:
                description = $"Thu hoạch được X tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}.";
                break;

            case EffectType.ChanHoa:
            case EffectType.ChanKhi:
            case EffectType.ChanKiem:
            case EffectType.ChanKim:
            case EffectType.ChanMoc:
            case EffectType.ChanThan:
            case EffectType.ChanTho:
            case EffectType.ChanThuy:
            case EffectType.Diem:
            case EffectType.GiamTonThuong:
            case EffectType.HoaDiem:
            case EffectType.HoTrao:
            case EffectType.KiemKhi:
            case EffectType.NguHoa:
            case EffectType.NguHoaHiep:
            case EffectType.NguKhiHiep:
            case EffectType.NguKiemHiep:
            case EffectType.NguKim:
            case EffectType.NguKimHiep:
            case EffectType.NguMoc:
            case EffectType.NguMocHiep:
            case EffectType.NgungThan:
            case EffectType.NguTho:
            case EffectType.NguThoHiep:
            case EffectType.NguThuy:
            case EffectType.NguThuyHiep:
            case EffectType.PhanTonThuong:
            case EffectType.SucThe:
            case EffectType.TichDoc:
            case EffectType.TichHoa:
            case EffectType.TichKim:
            case EffectType.TichMoc:
            case EffectType.TichThuy:
                EBuffAndDeBuff buffEnum = (EBuffAndDeBuff)System.Enum.Parse(typeof(EBuffAndDeBuff), type.ToString());
                description = $"Thu hoạch được X tầng {BaseSkill.GetKeyWordBuffOrDeBuff(buffEnum)}.";
                break;

            case EffectType.Nhanh:
                description = $"Thu hoạch được X tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TangToc)}.";
                break;
            case EffectType.TriLieuLieu:
                description = $"Thu hoạch được X tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)}.";
                break;
            case EffectType.NguThanHiep:
                description = $"Thu hoạch được X tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NguThanThucHiep)}.";
                break;

            // --- NHÓM ĐỐI THỦ NHẬN PHẢI (DEBUFF) ---
            case EffectType.Cat:
                description = $"Đối thủ nhận phải X tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cat)}.";
                break;

            case EffectType.ChayMau:
            case EffectType.DeTonThuong:
            case EffectType.PhaThe:
            case EffectType.SuongGia:
            case EffectType.ThanThuong:
            case EffectType.ThieuDot:
            case EffectType.TrungDoc:
                EBuffAndDeBuff debuffEnum = (EBuffAndDeBuff)System.Enum.Parse(typeof(EBuffAndDeBuff), type.ToString());
                description = $"Đối thủ nhận phải X tầng {BaseSkill.GetKeyWordBuffOrDeBuff(debuffEnum)}.";
                break;

            case EffectType.QuanQuanh:
                description = $"Đối thủ nhận phải X tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}.";
                break;
            case EffectType.TroiDu:
                description = $"Đối thủ nhận phải X tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TroiChan)}.";
                break;

            // --- NHÓM HẤP THỤ LINH KHÍ ---
            case EffectType.HapThuLinhKhi:
                description = "Hấp thụ ngẫu nhiên X Linh Khí.";
                break;
            case EffectType.HapThuHoaLinhKhi:
                description = "Hấp thụ X điểm Hỏa Linh Khí.";
                break;
            case EffectType.HapThuKimLinhKhi:
                description = "Hấp thụ X điểm Kim Linh Khí.";
                break;
            case EffectType.HapThuMocLinhKhi:
                description = "Hấp thụ X điểm Mộc Linh Khí.";
                break;
            case EffectType.HapThuThoLinhKhi:
                description = "Hấp thụ X điểm Thổ Linh Khí.";
                break;
            case EffectType.HapThuThuyLinhKhi:
                description = "Hấp thụ X điểm Thủy Linh Khí.";
                break;

            // --- NHÓM HỒI PHỤC ---
            case EffectType.KhoiPhucGiaTriSinhMenh:
            case EffectType.KhoiPhucGiaTriSinhMenhDauLuot:
                description = "Khôi phục X điểm HP.";
                break;

            // --- NHÓM SÁT THƯƠNG ---
            case EffectType.TaoThanhTonThuong:
                description = "Tạo thành X điểm tổn thương.";
                break;
            case EffectType.TaoThanhXLanXDiemThuongTon:
                description = "Tạo thành X lần X điểm tổn thương.";
                break;

            // --- NHÓM "TRONG" (START COMBAT) ---
            case EffectType.KimTrong:
                description = "Tăng X% tỉ lệ hấp thụ Kim linh khí.";
                break;
            case EffectType.MocTrong:
                description = "Tăng X% tỉ lệ hấp thụ Mộc linh khí.";
                break;
            case EffectType.ThuyTrong:
                description = "Tăng X% tỉ lệ hấp thụ Thủy linh khí.";
                break;
            case EffectType.HoaTrong:
                description = "Tăng X% tỉ lệ hấp thụ Hỏa linh khí.";
                break;
            case EffectType.ThoTrong:
                description = "Tăng X% tỉ lệ hấp thụ Thổ linh khí.";
                break;

            default:
                return "";
        }

        return description + suffix;
    }
    private static string GetDescEquipment(EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                return "<color=#ffb143>[Chủ động] </color>";
            case EquipmentType.Armor:
                return "<color=#6495ED>[Bị động] </color>";
            case EquipmentType.Jewelry:
                return "<color=#6495ED>[Bị động] </color>";
            default:
                break;
        }
        return "";
    }
    private static string GetDescEquipment(EquipmentType equipmentType,bool isSpecial)
    {
        if(!isSpecial)
        {
            switch (equipmentType)
            {
                case EquipmentType.Weapon:
                    return "";
                case EquipmentType.Armor:
                    return "<color=#6495ED>[Bắt đầu hiệp] </color>";
                case EquipmentType.Jewelry:
                    return "<color=#6495ED>[Kết thúc hiệp] </color>";
                default:
                    break;
            }
        }    
        else
        {
            return "<color=#6495ED>[Bắt đầu chiến đấu] </color>";
        }
        return "";
    }
}