using Base;
using LongDH.LongDHModules;
using LuyenDan;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameConfig : BaseBehaviour
{
    [SerializeField] private List<string> listSceneLoaded = new List<string>
    {
        "AllMaps",
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
    [Serializable]
    public class InforLevel
    {
        public int tuviRequireNextLevel;
        public int hpMaxBuff;
        public int thanThucBuff;
        public int donTocBuff;
        public int thoNguyenBuff;
    }
    public InforLevel[] inforLevels;
    public static string GetChucVuText(EMonPhai monPhai, int level)
    {
        return monPhaiText[monPhai]+" "+ chucVuText[ThanThong.GetDaiCanhGioi(level)];
    }
    public void OnBackScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        int index = listSceneLoaded.IndexOf(currentScene);
        if (index == 0) return;
        SceneManager.LoadScene(listSceneLoaded[index-1]);



    }
    public void OnLoadScene(params object[] data)
    {
        string nameScene = (string)data[0];
        listSceneLoaded.Add(nameScene);
        SceneManager.LoadScene(nameScene);



    }
    private void Awake()
    {
        // Kiểm tra nếu trong Pool đã có một Manager loại này rồi
        var existing = Pool.Get<GameConfig>();

        if (existing != null && existing != this)
        {
            // Nếu đã có bản cũ (từ Scene trước), xóa bản mới này đi
            Destroy(gameObject);
            return;
        }

        // Nếu chưa có, mình là "trùm", đăng ký vào Pool và giữ lại
        Pool.Set(this);
        DontDestroyOnLoad(gameObject);
        Observer.AddObserver(EventList.GamePlay.LOAD_SCENE,OnLoadScene);
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
    public static (string text, int nextMilestone,int index) GetDanDocValue(int currentDanDoc)
    {
        // Mảng các mốc giới hạn (20, 50, 70, 100, 120)
        // 0 -> 20: Thân thể khỏe mạnh (Chỉ số mảng 0)
        // > 20   : Thân thể không ngại (Chỉ số mảng 1)
        // ...
        // > 120  : Độc phát thân vong (Chỉ số mảng 5)

        for (int i = 0; i < danDocRequireEnd.Length; i++)
        {
            // Nếu chỉ số hiện tại nhỏ hơn hoặc bằng mốc đang xét
            if (currentDanDoc <= danDocRequireEnd[i])
            {
                return (danDocText[i], danDocRequireEnd[i], i);
            }
        }

        // Nếu vượt qua mốc cuối cùng (120)
        return (danDocText[danDocText.Length - 1], (int)currentDanDoc, danDocText.Length - 1);
    }
    public static (string name, int nextMilestone , int index) GetTamCanhValue(int currentPoint)
    {
        // Trường hợp đặc biệt: Nhỏ hơn 0
        if (currentPoint < 0)
            return (tamCanh[0], (int)0 , 0);

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

                return (tamCanh[i + 1], nextLevel, i+1);
            }
        }

        // Trường hợp cuối cùng: Lớn hơn mốc cao nhất (> 1999)
        return (tamCanh[tamCanh.Length - 1], (int)currentPoint, tamCanh.Length - 1);
    }
  
}
