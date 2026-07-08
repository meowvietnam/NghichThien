using AllMaps;
using Base;
using CreateCharacter;
using CreateCharacter.CharStats;
using DG.Tweening.Core.Easing;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using LuyenKhi;
using Network;
using Network.Extensions;
using Network.Models;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using CongPhap = Skill.CongPhap;
using ThanThong = Skill.ThanThong;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : BaseBehaviour , INetworkEvent.ILoadCharCombat
{
    [SerializeField] public List<string> listSceneLoaded = new List<string>
    {
        "AllMaps",
    };

    [SerializeField] public EMapType currentEMapType => Pool.Get<MapManager>().FindMap(Client.Instance.CharacterData.Progression.IdCurrentNode) is Stronghold strongHold ? strongHold.eCurrentMapType : EMapType.None;
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
    public static EMapType ConvertEMonPhaiToEMap(EMonPhai eMonPhai)
    {
        return eMonPhai switch
        {
            EMonPhai.None => EMapType.None,
            EMonPhai.KimHongKiemPhai => EMapType.KimHongKiemPhai,
            EMonPhai.TrucSonTong => EMapType.TrucSonTong,
            EMonPhai.TinhHaKiemPhai => EMapType.TinhHaKiemPhai,
            EMonPhai.LyHoaMon => EMapType.LyHoaMon,
            EMonPhai.HoaTranGiao => EMapType.HoaTranGiao,
            EMonPhai.BachDeLau => EMapType.BachDeLau,
            _ => EMapType.None // Dấu gạch dưới '_' thay thế cho 'default' nếu truyền vào một giá trị lạ
        };
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
    public InfoLevel[] inforLevels => LevelResource.Instance.inforLevels;

    public static string GetChucVuText(EMonPhai monPhai, int level)
    {
        string textChucVu = monPhaiText[monPhai];
        if(monPhai != EMonPhai.None)
        {
            textChucVu += " " + chucVuText[ThanThong.GetDaiCanhGioi(level)];
        }
        return textChucVu;
    }   

    public void OnBackScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        int index = listSceneLoaded.IndexOf(currentScene);
        if (index == 0)
        {
            return;
        }
        Pool.Get<UIManager>().panelGroup.SetActive(true);
        Pool.Get<UIManager>().ListIcon.gameObject.SetActive(true);
        string backScene = listSceneLoaded[index - 1];
        listSceneLoaded.Remove(currentScene);
        Pool.Get<UIManager>().loadScene.gameObject.SetActive(true);
        var sceneManger = Client.Instance.CoreModule.GetHandler<Network.Core.SceneManager>();
        sceneManger.ChangeScene(backScene, OnChangeSceneResult);
    }

    public void OnLoadScene(params object[] data)
    {
        string nameScene = (string)data[0];
        if (string.IsNullOrEmpty(nameScene))
        {
            if (data.Length > 2)
            {
                Map currentMapLoad = (Map)data[2];
                if(!currentMapLoad.IsEmptyAction)
                {
                    return;
                }    
            }
            
            SetDialogGacCong("xin dừng bước hiện tại nơi này chưa thể vào hãy kiên nhẫn chờ bản cập nhật tiếp theo!");

            return;
        }

        if (data.Length > 1)
        {
            EMapType eMap = (EMapType)data[1];
        }
        Pool.Get<UIManager>().loadScene.gameObject.SetActive(true);
        var sceneManger = Client.Instance.CoreModule.GetHandler<Network.Core.SceneManager>();
        sceneManger.ChangeScene(nameScene, OnChangeSceneResult);
    }
    public static void SetDialogGacCong(string content , System.Action action = null,bool isAutoHide=true)
    {
        System.Random _random = new System.Random();
        Array values = Enum.GetValues(typeof(Gender));
        Gender randomGender = (Gender)values.GetValue(_random.Next(values.Length));
        var xungHo = XungHoText(6, randomGender);
        SayDialogManager.Instance.ShowDialogRandomSkin("Người Gác Cổng", randomGender,
           new List<string> { $"{xungHo.targetCallMe} {content}" },null,null,action,isAutoHide);
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
                Pool.Get<UIManager>().loadScene.gameObject.SetActive(false);
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
    public CharCombatInit user = null;
    public ETypeCombat currentETypeCombat = ETypeCombat.None;
    public ETypeResultCombat currentECombatResult = ETypeResultCombat.None;
    public Sprite spriteBGCombat;

    public void SetBGCombat(Sprite  sprite)
    {
        this.spriteBGCombat = sprite;
    }
    public void GoToCombatArena()
    {
        if (enemyCombat == null) return;
        Pool.Get<UIManager>().panelGroup.SetActive(false);
        Pool.Get<UIManager>().commonMask.gameObject.SetActive(false);
        Pool.Get<UIManager>().ListIcon.gameObject.SetActive(false);
        Pool.Get<UIManager>().loadScene.gameObject.SetActive(true);
        Observer.Notify(EventList.GamePlay.LOAD_SCENE, "Combat");


    }
    public void GoToCombat()
    {
        if (enemyCombat == null) return;
        Pool.Get<UIManager>().panelGroup.SetActive(false);
        Pool.Get<UIManager>().commonMask.gameObject.SetActive(false);
        Pool.Get<UIManager>().ListIcon.gameObject.SetActive(false);
        Pool.Get<PrepareCombatPanel>().Hide();
        Pool.Get<UIManager>().loadScene.gameObject.SetActive(true);
        Observer.Notify(EventList.Network.LOAD_CHAR_COMBAT, this);


    }
    public void GoToPrepareCombatArena(CharacterSnapshot user,CharacterSnapshot enemy)
    {
        this.user = new CharCombatInit
        {
            eTypeVisual = ETypeVisualChar.Character,
            charStats = CharacterStatsSecureBuilder.ArenaFromCharacterSnapshot(user),
        };
        enemy = null;
        if(enemy != null)
        {
            this.enemyCombat = new CharCombatInit
            {
                eTypeVisual = ETypeVisualChar.Character,
                charStats = CharacterStatsSecureBuilder.ArenaFromCharacterSnapshot(enemy),
            };
        }
        else
        {
            this.enemyCombat = new CharCombatInit
            {
                eTypeVisual = ETypeVisualChar.Character,
                charStats = Pool.Get<NPCManager>().npcDatas.FirstOrDefault(x=>x.charStats.level.Value == user.Level).charStats,
            };
        }
        if (enemyCombat == null) return;
        currentETypeCombat = ETypeCombat.DauTruong;
        currentECombatResult = ETypeResultCombat.None;
    }
    public void GoToPrepareCombat(CharCombatInit enemyCombat, ETypeCombat eTypeCombat)
    {
        this.enemyCombat = enemyCombat;
        if (enemyCombat == null) return;
        currentETypeCombat = eTypeCombat;
        Pool.Get<PrepareCombatPanel>().Show();
        currentECombatResult = ETypeResultCombat.None;
    }
    public void BackAllMapFromCombat(ETypeResultCombat eCombatResult)
    {

        Pool.Get<UIManager>().loadScene.gameObject.SetActive(true);
        Pool.Get<UIManager>().panelGroup.SetActive(true);
        Pool.Get<UIManager>().ListIcon.gameObject.SetActive(true);
        currentECombatResult = eCombatResult;
        string backScene = listSceneLoaded[listSceneLoaded.Count-1];
        Observer.Notify(EventList.GamePlay.BACK_SCENE, backScene);
        SoundManager.Instance.PlayMusicHome();


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

    private void OnDestroy()
    {
        Observer.RemoveObserver(EventList.GamePlay.LOAD_SCENE, OnLoadScene);
        Observer.RemoveObserver(EventList.GamePlay.BACK_SCENE, OnBackScene);
    }

    public ThanThong[] thanThongs;
    public CongPhap[] congPhaps;
    public AllMaps.NgoDao.NgoDao[] ngoDaos;
    public BaseDanDuocCombat[] danDuocs;

    public ThanThong GetThanThongWithId(int id)
    {
        return thanThongs.FirstOrDefault(x => x.Id == id);
    }

    public CongPhap GetCongPhapWithId(int id)
    {
        return congPhaps.FirstOrDefault(x => x.Id == id);
    }
    public AllMaps.NgoDao.NgoDao GetNgoDaoWithId(int id)
    {
        return ngoDaos.FirstOrDefault(x => x.Id == id);
    }

    public BaseDanDuocCombat GetDanDuocWithId(int id)
    {
        return danDuocs.FirstOrDefault(x => x.Id == id);
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

    public static Dictionary<Tier, int> PriceFurnaceWithTierPerDurability => PriceHerbWithTier;
    public static Dictionary<ItemType, EquipmentType> ItemTypeToEquipemtType = new Dictionary<ItemType, EquipmentType>
    {
        { ItemType.VuKhi, EquipmentType.Weapon },
        { ItemType.PhongCu, EquipmentType.Armor },
        { ItemType.TrangSuc, EquipmentType.Jewelry },

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

    [Button]
    private void OnUpdateDataNgoDao()
    {
#if UNITY_EDITOR
    // Xác định folder quét để tránh quét nhầm các asset khác cùng type
    string[] foldersToSearch = { "Assets/Scripts/AllMaps/NgoDao" };
    
    // Tìm GUID của các asset có type là NgoDaoCombat (ScriptableObject)
    string[] guids = AssetDatabase.FindAssets("t:NgoDaoCombat", foldersToSearch);

    ngoDaos = guids
        .Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid)))
        // Ép kiểu về NgoDao an toàn, nếu NgoDao không kế thừa UnityEngine.Object trực tiếp thì ép qua ScriptableObject trước
        .OfType<AllMaps.NgoDao.NgoDao>() 
        .ToArray();

    EditorUtility.SetDirty(this);
    Debug.Log($"Đã cập nhật {ngoDaos.Length} Ngộ Đạo từ thư mục AllMaps/NgoDao.");
#endif
    }
    [Button]
    private void OnUpdateDataDanDuoc()
    {
#if UNITY_EDITOR
    // Xác định folder quét
    string[] foldersToSearch = { "Assets/Scripts/Combat/DanDuocCombat" };

    // Tìm tất cả GUID của các asset có type gốc là BaseDanDuocCombat
    string[] guids = AssetDatabase.FindAssets("t:BaseDanDuocCombat", foldersToSearch);

    danDuocs = guids
        .Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid)))
        // Cần đảm bảo Class đích (BaseDanDuocCombat hoặc DanDuocCombat) tồn tại trong Namespace
        .OfType<BaseDanDuocCombat>() 
        .ToArray();

    EditorUtility.SetDirty(this);
    Debug.Log($"Đã cập nhật {danDuocs.Length} Đan Dược từ thư mục Combat/DanDuocCombat.");
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
        "Thân thể khỏe mạnh",  // i = 0
        "Thân thể không ngại", // i = 1
        "Có chút khó chịu",    // i = 2
        "Gân mạch ngưng trệ",  // i = 3
        "Bệnh nguy kịch",      // i = 4
    };

    public static int[] danDocRequireEnd = new int[]
    {
        20,  // Tương ứng với "Thân thể khỏe mạnh" (<= 20)
        50,  // Tương ứng với "Thân thể không ngại" (21 - 50)
        70,  // Tương ứng với "Có chút khó chịu" (51 - 70)
        100, // Tương ứng với "Gân mạch ngưng trệ" (71 - 100)
             // Đã xóa mốc 120
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
        // Khi vượt quá 100 điểm, trả về "Bệnh nguy kịch"
        return (danDocText[danDocText.Length - 1], currentDanDoc, danDocText.Length - 1);
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
        return (tamCanh[tamCanh.Length - 1], (int)danDocRequireEnd[danDocRequireEnd.Length-1], tamCanh.Length - 1);
    }

  
 
    public static int GetIntTuoiTac(TimeSpan timeSpanTuoiTac)
    {
        return (int)(timeSpanTuoiTac.TotalDays / 365);
    }
    public static TimeSpan ConvertMonthToTimeSpan(float months)
    {
        return TimeSpan.FromDays(months * 30);
    }
    // Quy ước tu tiên
    private const int DAYS_IN_MONTH = 30;
    private const int MONTHS_IN_YEAR = 12;

    // 1. Hàm tính tổng số tháng (vẫn giữ nguyên để babe làm logic khác nếu cần)
    public static int GetDayTimeWorldIntPlayer()
    {
        TimeSpan originalTime = Client.Instance.CharacterData.Stats.TuoiTac;
        int totalDays = (int)originalTime.TotalDays;
        return totalDays - (15 * MONTHS_IN_YEAR * DAYS_IN_MONTH); // Trừ 15 tuổi gốc
    }
    public static float GetMonthTimeWorldIntPlayer()
    {
        return (float)GetDayTimeWorldIntPlayer()/(float)DAYS_IN_MONTH; // Trừ 15 tuổi gốc
    }
    // 2. HÀM SHOW TEXT MỚI: Truyền thẳng TimeSpan tuổi vào để bóc tách
    public static string GetDateTimeText(int day)
    {
        // Tổng số ngày tuổi hiện tại của người chơi

        // Tính số ngày của thế giới (Trừ đi 15 tuổi gốc = 15 * 360 = 5400 ngày)
        int totalWorldDays = day;

        // Nếu nhân vật chưa đủ 15 tuổi (đề phòng lỗi)
        if (totalWorldDays < 0) totalWorldDays = 0;

        // Tiến hành bổ nhỏ ngày thế giới ra Năm / Tháng / Ngày theo quy ước 30 ngày
        int DAYS_IN_YEAR = DAYS_IN_MONTH * MONTHS_IN_YEAR; // 360

        int years = totalWorldDays / DAYS_IN_YEAR;
        int remainingDaysAfterYears = totalWorldDays % DAYS_IN_YEAR;

        int months = remainingDaysAfterYears / DAYS_IN_MONTH;
        int days = remainingDaysAfterYears % DAYS_IN_MONTH; // Lấy phần dư ngày lẻ cuối cùng

        return $"{years} năm {months} tháng {days} ngày";
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

    public static T GetItemPoolWithPrefabInParent<T>(T prefabItem) where T : Component
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
  
    private static string GetEffectDescription(EffectType type,string Value)
    {
        switch (type)
        {
            // --- NHÓM CASTER THU HOẠCH (BUFF/DEBUFF) ---
            case EffectType.Bui:
                return $"Thu hoạch được {Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}.";
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
                EBuffAndDeBuff buffEnum = (EBuffAndDeBuff)System.Enum.Parse(typeof(EBuffAndDeBuff), type.ToString());
                return $"Thu hoạch được {Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(buffEnum)}.";

            case EffectType.Nhanh:
                return $"Thu hoạch được {Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TangToc)}.";
            case EffectType.TriLieuLieu:
                return $"Thu hoạch được {Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)}.";
            case EffectType.NguThanHiep:
                return $"Thu hoạch được {Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NguThanThucHiep)}.";

            // --- NHÓM ĐỐI THỦ NHẬN PHẢI (DEBUFF) ---
            case EffectType.Cat:
                return $"Đối thủ nhận phải {Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cat)}.";
            case EffectType.ChayMau:
            case EffectType.DeTonThuong:
            case EffectType.PhaThe:
            case EffectType.SuongGia:
            case EffectType.ThanThuong:
            case EffectType.ThieuDot:
            case EffectType.TrungDoc:
                EBuffAndDeBuff debuffEnum = (EBuffAndDeBuff)System.Enum.Parse(typeof(EBuffAndDeBuff), type.ToString());
                return $"Đối thủ nhận phải {Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(debuffEnum)}.";

            case EffectType.QuanQuanh:
                return $"Đối thủ nhận phải {Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}.";
            case EffectType.TroiDu:
                return $"Đối thủ nhận phải {Value} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TroiChan)}.";

            // --- NHÓM HẤP THỤ LINH KHÍ ---
            case EffectType.HapThuLinhKhi:
                return $"Hấp thụ ngẫu nhiên {Value} Linh Khí.";
            case EffectType.HapThuHoaLinhKhi:
                return $"Hấp thụ {Value} điểm Hỏa Linh Khí.";
            case EffectType.HapThuKimLinhKhi:
                return $"Hấp thụ {Value} điểm Kim Linh Khí.";
            case EffectType.HapThuMocLinhKhi:
                return $"Hấp thụ {Value} điểm Mộc Linh Khí.";
            case EffectType.HapThuThoLinhKhi:
                return $"Hấp thụ {Value} điểm Thổ Linh Khí.";
            case EffectType.HapThuThuyLinhKhi:
                return $"Hấp thụ {Value} điểm Thủy Linh Khí.";

            // --- NHÓM HỒI PHỤC ---
            case EffectType.KhoiPhucGiaTriSinhMenh:
            case EffectType.KhoiPhucGiaTriSinhMenhDauLuot:
                return $"Khôi phục {Value} điểm HP.";

            // --- NHÓM SÁT THƯƠNG ---
            case EffectType.TaoThanhTonThuong:
                return $"Tạo thành {Value} điểm tổn thương.";
            case EffectType.TaoThanhXLanXDiemThuongTon:
                return $"Tạo thành {Value} lần {Value} điểm tổn thương.";

            // --- NHÓM "TRONG" (START COMBAT) ---
            case EffectType.KimTrong:
                return $"Tăng {Value}% tỉ lệ hấp thụ Kim linh khí.";

            case EffectType.MocTrong:
                return $"Tăng {Value}% tỉ lệ hấp thụ Mộc linh khí.";

            case EffectType.ThuyTrong:
                return $"Tăng {Value}% tỉ lệ hấp thụ Thủy linh khí.";

            case EffectType.HoaTrong:
                return $"Tăng {Value}% tỉ lệ hấp thụ Hỏa linh khí.";

            case EffectType.ThoTrong:
                return $"Tăng {Value}% tỉ lệ hấp thụ Thổ linh khí.";

            default:
                return "";
        }
    }
    private static string GetEffectDescription(dynamic effect)
    {
        // BƯỚC QUAN TRỌNG: Kiểm tra và bóc tách giá trị số thực tế
        int actualValue;

        // Nếu nó là SecureInt (trong EffectClient)
        if (effect.Value is SecureInt secure)
        {
            actualValue = secure.Value;
        }
        // Nếu nó là byte/int (trong Effect)
        else
        {
            actualValue = (int)effect.Value;
        }

        // Bây giờ gọi hàm mô tả với kiểu dữ liệu "thuần" (EffectType, int)
        return GetEffectDescription((EffectType)effect.Type, actualValue.ToString());
    }
    public static string GetEffectDescription(EffectType type)
    {
        string suffix = " (X là giá trị ngẫu nhiên được luyện thành)";
        string description = GetEffectDescription(type,"X");
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
    [Button]
    public void ShowSpriteSize()
    {
        var sprites = thanThongs.OfType<BaseData>()
            .Concat(congPhaps)
            .Concat(ngoDaos)
            .Concat(danDuocs)
            .Select(x => x.spriteItem)
            .Where(x => x != null)
            .ToHashSet();
        var size = AssetSize.CalculateSize(sprites);
        Debug.Log(size);
    }

    [Button]
    public void ExportNDTTCP()
    {
        var guids = AssetDatabase.FindAssets("t:BaseData");
        var assets = guids.Select(x => AssetDatabase.GUIDToAssetPath(x))
            .Select(x => AssetDatabase.LoadAssetAtPath<BaseData>(x))
            .OrderBy(x => x.GetType().Name)
            .ToList();

        var thanThongs = assets.OfType<ThanThong>().ToList();
        var ngoDaos = assets.OfType<AllMaps.NgoDao.NgoDao>().ToList();
        var congPhaps = assets.OfType<CongPhap>().ToList();
        this.thanThongs = thanThongs.ToArray();
        this.ngoDaos = ngoDaos.ToArray();
        this.congPhaps = congPhaps.ToArray();
        var ndttcp = new NDCPTTManager()
        {
            ThanThongs = thanThongs.ToList(),
            NgoDaos = ngoDaos.ToList(),
            CongPhaps = congPhaps.ToList(),
        };
        var writer = new Writer(ndttcp.ByteSize());
        writer.Write(ndttcp);
        var path = Path.Combine(Application.dataPath, "Exports", "NDCPTTManager.bin");
        File.WriteAllBytes(path, writer.Buffer);
    }

    [Button]
    public void ExportNDResources()
    {
        var guids = AssetDatabase.FindAssets("t:NgoDaoResource");
        var assets = guids.Select(x => AssetDatabase.GUIDToAssetPath(x))
            .Select(x => AssetDatabase.LoadAssetAtPath<NgoDaoResource>(x))
            .ToList();
        var writer = new Writer(assets.ByteSize());
        writer.WriteArray(assets);
        var path = Path.Combine(Application.dataPath, "Exports", "NgoDaoManager.bin");
        File.WriteAllBytes(path, writer.Buffer);
        Debug.Log(path);
    }

    [Button]
    public void SetTimeLearnSkill()
    {
        foreach (var item in thanThongs)
        {
            item.SetBaseTimeLearn();
#if UNITY_EDITOR
            // Đánh dấu file Thần Thông này đã bị sửa đổi
            EditorUtility.SetDirty(item);
#endif
        }
        foreach (var item in congPhaps)
        {
            item.SetBaseTimeLearn();
#if UNITY_EDITOR
            // Đánh dấu file Công Pháp này đã bị sửa đổi
            EditorUtility.SetDirty(item);
#endif

        }
#if UNITY_EDITOR
        // Đánh dấu file tổng này thay đổi (để lưu danh sách nếu có thay đổi)
        EditorUtility.SetDirty(this);

        // Ép Unity ghi toàn bộ các file đã đánh dấu ở trên xuống ổ cứng
        AssetDatabase.SaveAssets();

        // Buộc Editor vẽ lại giao diện ngay lập tức để cập nhật số liệu mới hiển thị
        AssetDatabase.Refresh();

        Debug.Log("Đã lưu data thành công cho từng Thần Thông và Công Pháp!");
#endif
    }

    public void SuccessLoadCharCombat(CharacterSnapshot user)
    {
        this.user = new CharCombatInit
        {
            eTypeVisual = ETypeVisualChar.Character,
            charStats = CharacterStatsSecureBuilder.FromCharacterSnapshot(user),
        };

        Observer.Notify(EventList.GamePlay.LOAD_SCENE, "Combat");
    }

    public void FailLoadCharCombat(string message)
    {
        Debug.Log(message);
    }
}
