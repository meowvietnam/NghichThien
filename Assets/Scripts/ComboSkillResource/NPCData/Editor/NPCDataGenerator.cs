using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Base;
using ComboSkillResource.NPCData;
using CreateCharacter;

public class NPCDataGenerator : EditorWindow
{
    private ComboSkillResource.ComboSkillResource comboResource;
    private string basePath = "Assets/Scripts/ComboSkillResource/NPCData";

    [MenuItem("Tools/NPC/Generate NPC Data")]
    public static void ShowWindow()
    {
        GetWindow<NPCDataGenerator>("NPC Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("NPC Generation Settings", EditorStyles.boldLabel);
        comboResource = (ComboSkillResource.ComboSkillResource)EditorGUILayout.ObjectField("Combo Resource",
            comboResource, typeof(ComboSkillResource.ComboSkillResource), false);

        if (GUILayout.Button("Generate NPCs (Check Type & Auto ID)"))
        {
            if (comboResource == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign ComboSkillResource!", "OK");
                return;
            }

            Generate();
        }
    }

    private void Generate()
    {
        EnsureFolderExists(basePath);

        // 1. Thay đổi cấu trúc: Lưu lại danh sách Level đã tồn tại cho TỪNG loại Combo
        // Key: Combo Type, Value: Bộ danh sách các Level đã có file NPCData
        Dictionary<ComboSkillResource.ETypeCombo, HashSet<int>> existingComboLevels =
            new Dictionary<ComboSkillResource.ETypeCombo, HashSet<int>>();
        int maxId = 0;

        // Tìm tất cả NPCData hiện có trong thư mục
        string[] allNpcGuids = AssetDatabase.FindAssets("t:NPCData", new[] { basePath });
        foreach (string guid in allNpcGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            NPCData npc = AssetDatabase.LoadAssetAtPath<NPCData>(path);

            if (npc != null && npc.charStatsCreateRef != null)
            {
                // Cập nhật Max ID để tránh trùng ID nhân vật
                if (npc.charStatsCreateRef.idChar.Value > maxId)
                    maxId = npc.charStatsCreateRef.idChar.Value;

                // Thu thập dữ liệu loại Combo và Cấp độ để kiểm tra trùng lặp
                if (npc.comboChar != null && npc.charStatsCreateRef.level != null)
                {
                    var type = npc.comboChar.typeCombo;
                    int lv = npc.charStatsCreateRef.level; // Giả định trường lấy Level của bạn là kiểu int

                    if (!existingComboLevels.ContainsKey(type))
                    {
                        existingComboLevels[type] = new HashSet<int>();
                    }

                    existingComboLevels[type].Add(lv);
                }
            }
        }

        int currentMaxId = maxId;

        // Định nghĩa danh sách các bộ Level cần sinh theo đúng yêu cầu của bạn:
        // 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 1, 1, 12, 13, 14, 15
        // LƯU Ý: Đoạn chuỗi bạn ghi "1 1 12" tôi hiểu là bạn gõ nhầm số 11 thành số 1, nên mảng dưới đây sẽ sửa lại chuẩn từ 1 đến 15 nhé.
        int[] levelsToGenerate = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

        // 2. Duyệt qua danh sách Combo từ Resource
        foreach (var comboType in comboResource.comboNPC)
        {
            if (string.IsNullOrEmpty(comboType.nameCombo)) continue;

            string folderPath = $"{basePath}/{comboType.nameCombo}";
            EnsureFolderExists(folderPath);

            int countCreatedInBatch = 0;

            // VÒNG LẶP ĐỘNG: Duyệt qua danh sách level quy định ở trên
            // Quét ngược từ Level cao xuống Level thấp (hoặc từ đầu tới cuối mảng tùy bạn, ở đây tôi duyệt từ cuối mảng lên giống code cũ của bạn)
            for (int i = levelsToGenerate.Length - 1; i >= 0; i--)
            {
                int targetLv = levelsToGenerate[i];

                // 🔍 KIỂM TRA ĐIỀU KIỆN: 
                // Nếu Type này đã từng được tạo NPC có level tương ứng -> BỎ QUA KHÔNG TẠO TRÙNG
                if (existingComboLevels.ContainsKey(comboType.typeCombo) &&
                    existingComboLevels[comboType.typeCombo].Contains(targetLv))
                {
                    continue;
                }

                // Nếu chưa có file cho Level này, tiến hành tạo mới và cập nhật ID tăng tiến
                currentMaxId = CreateNPCBatch(folderPath, comboType, 3, targetLv, targetLv, currentMaxId);
                countCreatedInBatch++;
            }

            if (countCreatedInBatch > 0)
            {
                Debug.Log(
                    $"<color=green>Success:</color> Đã cập nhật bổ sung {countCreatedInBatch} NPC mới cho Type: {comboType.typeCombo}");
            }
            else
            {
                Debug.Log(
                    $"<color=cyan>Skip:</color> Combo Type <b>{comboType.typeCombo}</b> đã có đầy đủ các file level từ trước. Không tạo thêm.");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", "NPC Generation/Update Completed!", "OK");
    }

    private int CreateNPCBatch(string path, ComboSkillResource.ComboSkillResource.ComboWithType comboType, int count,
        int minLv, int maxLv, int startId)
    {
        int runningId = startId;
        for (int i = 0; i < count; i++)
        {
            string fileName = $"{comboType.nameCombo}_Lv{maxLv}_{i + 1}.asset";
            string assetPath = $"{path}/{fileName}";

            if (AssetDatabase.LoadAssetAtPath<NPCData>(assetPath) != null) continue;

            NPCData npc = ScriptableObject.CreateInstance<NPCData>();
            npc.comboChar = new ComboSkillResource.ComboSkillResource.ComboWithType
            {
                nameCombo = comboType.nameCombo,
                typeCombo = comboType.typeCombo,
                monPhai = comboType.monPhai,
                eTypeSkill = comboType.eTypeSkill,
                comboWithDaiCanhGioi = comboType.comboWithDaiCanhGioi
            };

            npc.charStatsCreateRef = new CharacterStatsSecure();

            // --- SET ID MỚI ---
            runningId++;
            npc.charStatsCreateRef.idChar = new SecureInt(runningId);

            npc.charStatsCreateRef.level = new SecureInt(Random.Range(minLv, maxLv + 1));
            npc.charStatsCreateRef.monPhai = comboType.monPhai;
            npc.charStatsCreateRef.Gender = (Gender)Random.Range(0, 2);


            npc.charStatsCreateRef.hpMax = new SecureInt(Random.Range(30, 61));
            npc.charStatsCreateRef.hp = new SecureInt(npc.charStatsCreateRef.hpMax.Value);
            npc.charStatsCreateRef.tuChat = new SecureInt(Random.Range(50, 101));
            npc.charStatsCreateRef.ngoTinh = new SecureInt(Random.Range(50, 101));
            npc.charStatsCreateRef.thoNguyen = new SecureInt(100);

            // --- APPLY ELEMENTAL STATS ---
            ApplyElementalStats(npc.charStatsCreateRef, comboType.eTypeSkill);

            AssetDatabase.CreateAsset(npc, assetPath);
            EditorUtility.SetDirty(npc);
        }

        return runningId; // Trả về ID lớn nhất sau khi tạo batch
    }

    private void ApplyElementalStats(CharacterStatsSecure stats, BaseSkill.ETypeSkill[] types)
    {
        stats.percentKim = new SecureFloat(10f);
        stats.percentMoc = new SecureFloat(10f);
        stats.percentThuy = new SecureFloat(10f);
        stats.percentHoa = new SecureFloat(10f);
        stats.percentTho = new SecureFloat(10f);

        var validElements = types.Where(t => t >= BaseSkill.ETypeSkill.Kim && t <= BaseSkill.ETypeSkill.Tho).ToList();

        if (validElements.Count > 0)
        {
            float bonusPool = 50f;
            float perElement = bonusPool / validElements.Count;

            foreach (var type in validElements)
            {
                if (type == BaseSkill.ETypeSkill.Kim) stats.percentKim = new SecureFloat(perElement);
                else if (type == BaseSkill.ETypeSkill.Moc) stats.percentMoc = new SecureFloat(perElement);
                else if (type == BaseSkill.ETypeSkill.Thuy) stats.percentThuy = new SecureFloat(perElement);
                else if (type == BaseSkill.ETypeSkill.Hoa) stats.percentHoa = new SecureFloat(perElement);
                else if (type == BaseSkill.ETypeSkill.Tho) stats.percentTho = new SecureFloat(perElement);
            }
        }
        else
        {
            // Tương sinh ngẫu nhiên: Kim(0)->Thủy(1)->Mộc(2)->Hỏa(3)->Thổ(4)
            int randBase = Random.Range(0, 5);
            int randNext = (randBase + 1) % 5;
            SetElementByIndex(stats, randBase, 25f);
            SetElementByIndex(stats, randNext, 25f);
        }
    }

    private void SetElementByIndex(CharacterStatsSecure stats, int index, float value)
    {
        if (index == 0) stats.percentKim = new SecureFloat(value);
        else if (index == 1) stats.percentThuy = new SecureFloat(value);
        else if (index == 2) stats.percentMoc = new SecureFloat(value);
        else if (index == 3) stats.percentHoa = new SecureFloat(value);
        else if (index == 4) stats.percentTho = new SecureFloat(value);
    }

    private void EnsureFolderExists(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string[] folders = path.Split('/');
        string currentPath = folders[0];
        for (int i = 1; i < folders.Length; i++)
        {
            string nextPath = $"{currentPath}/{folders[i]}";
            if (!AssetDatabase.IsValidFolder(nextPath))
                AssetDatabase.CreateFolder(currentPath, folders[i]);
            currentPath = nextPath;
        }
    }
}