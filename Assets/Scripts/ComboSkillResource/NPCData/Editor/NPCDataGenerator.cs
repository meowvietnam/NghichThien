using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Base;
using CreateCharacter;

public class NPCDataGenerator : EditorWindow
{
    private ComboSkillResource comboResource;
    private string basePath = "Assets/Scripts/ComboSkillResource/NPCData";

    [MenuItem("Tools/NPC/Generate NPC Data")]
    public static void ShowWindow()
    {
        GetWindow<NPCDataGenerator>("NPC Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("NPC Generation Settings", EditorStyles.boldLabel);
        comboResource = (ComboSkillResource)EditorGUILayout.ObjectField("Combo Resource", comboResource, typeof(ComboSkillResource), false);

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

        // 1. Quét toàn bộ NPC hiện có để lấy Max ID và các Type đã tồn tại
        int maxId = 0;
        HashSet<ComboSkillResource.ETypeCombo> existingTypes = new HashSet<ComboSkillResource.ETypeCombo>();

        // Tìm tất cả NPCData trong toàn bộ thư mục gốc và thư mục con
        string[] allNpcGuids = AssetDatabase.FindAssets("t:NPCData", new[] { basePath });
        foreach (string guid in allNpcGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            NPCData npc = AssetDatabase.LoadAssetAtPath<NPCData>(path);
            if (npc != null && npc.charStatsCreateRef != null)
            {
                // Cập nhật Max ID
                if (npc.charStatsCreateRef.idChar.Value > maxId) maxId = npc.charStatsCreateRef.idChar.Value;

                // Thu thập Type đã tồn tại từ dữ liệu bên trong file
                if (npc.comboChar != null)
                {
                    existingTypes.Add(npc.comboChar.typeCombo);
                }
            }
        }

        int currentMaxId = maxId;

        // 2. Duyệt qua danh sách Combo từ Resource
        foreach (var comboType in comboResource.comboNPC)
        {
            if (string.IsNullOrEmpty(comboType.nameCombo)) continue;

            // KIỂM TRA: Nếu Type này đã tồn tại trong bất kỳ folder nào rồi thì bỏ qua
            if (existingTypes.Contains(comboType.typeCombo))
            {
                Debug.Log($"<color=yellow>Skip:</color> Combo Type <b>{comboType.typeCombo}</b> đã được tạo trước đó. Bỏ qua.");
                continue;
            }

            // Tạo folder cho loại Combo mới
            string folderPath = $"{basePath}/{comboType.nameCombo}";
            EnsureFolderExists(folderPath);

            // Tạo các Batch Level và truyền ID hiện tại vào để tăng tiến
            currentMaxId = CreateNPCBatch(folderPath, comboType, 2, 13, 15, currentMaxId);
            currentMaxId = CreateNPCBatch(folderPath, comboType, 3, 10, 12, currentMaxId);
            currentMaxId = CreateNPCBatch(folderPath, comboType, 4, 7, 9, currentMaxId);
            currentMaxId = CreateNPCBatch(folderPath, comboType, 5, 4, 6, currentMaxId);
            currentMaxId = CreateNPCBatch(folderPath, comboType, 6, 1, 3, currentMaxId);

            Debug.Log($"<color=green>Success:</color> Đã tạo xong 20 NPC cho Type: {comboType.typeCombo}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", "NPC Generation Completed!", "OK");
    }

    private int CreateNPCBatch(string path, ComboSkillResource.ComboWithType comboType, int count, int minLv, int maxLv, int startId)
    {
        int runningId = startId;
        for (int i = 0; i < count; i++)
        {
            string fileName = $"{comboType.nameCombo}_Lv{maxLv}_{i + 1}.asset";
            string assetPath = $"{path}/{fileName}";

            if (AssetDatabase.LoadAssetAtPath<NPCData>(assetPath) != null) continue;

            NPCData npc = ScriptableObject.CreateInstance<NPCData>();
            npc.comboChar = new ComboSkillResource.ComboWithType
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