using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Base;
using CreateCharacter;

public class EnemyDataGenerator : EditorWindow
{
    private ComboSkillResource comboResource;
    private string basePath = "Assets/Scripts/ComboSkillResource/EnemyData";
    private int targetMaxLevel = 15; // Bạn có thể sửa số này lên 20, 30 tùy ý sau này

    [MenuItem("Tools/NPC/Generate Enemy Data")]
    public static void ShowWindow()
    {
        GetWindow<EnemyDataGenerator>("Enemy Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Enemy Generation & Update Settings", EditorStyles.boldLabel);
        comboResource = (ComboSkillResource)EditorGUILayout.ObjectField("Combo Resource", comboResource, typeof(ComboSkillResource), false);
        targetMaxLevel = EditorGUILayout.IntField("Target Max Level", targetMaxLevel);

        if (GUILayout.Button("Generate/Update Enemies"))
        {
            if (comboResource == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign ComboSkillResource!", "OK");
                return;
            }
            GenerateOrUpdate();
        }
    }

    private void GenerateOrUpdate()
    {
        EnsureFolderExists(basePath);

        // 1. Quét ID lớn nhất để không bị trùng ID khi tạo mới
        int maxId = 0;
        string[] allGuids = AssetDatabase.FindAssets("t:EnemyData", new[] { basePath });
        foreach (string guid in allGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EnemyData enemy = AssetDatabase.LoadAssetAtPath<EnemyData>(path);
            if (enemy != null && enemy.charStatsCreateRef != null)
            {
                if (enemy.charStatsCreateRef.idChar.Value > maxId)
                    maxId = enemy.charStatsCreateRef.idChar.Value;
            }
        }

        int currentRunningId = maxId;
        int createdCount = 0;

        // 2. Duyệt qua từng Combo
        foreach (var comboType in comboResource.comboNPC)
        {
            if (string.IsNullOrEmpty(comboType.nameCombo)) continue;

            string folderPath = $"{basePath}/{comboType.nameCombo}";
            EnsureFolderExists(folderPath);

            // 3. Kiểm tra từng Level từ 1 đến targetMaxLevel
            for (int lv = 1; lv <= targetMaxLevel; lv++)
            {
                string fileName = $"{comboType.nameCombo}_Lv{lv:00}.asset";
                string assetPath = $"{folderPath}/{fileName}";

                // KIỂM TRA: Nếu file level này đã tồn tại thì bỏ qua, chỉ tạo cái chưa có
                if (AssetDatabase.LoadAssetAtPath<EnemyData>(assetPath) != null)
                {
                    continue;
                }

                // Nếu chưa có, tiến hành tạo mới
                currentRunningId++;
                CreateEnemyUnit(assetPath, comboType, lv, currentRunningId);
                createdCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", $"Cập nhật hoàn tất! Đã tạo thêm {createdCount} Enemy mới.", "OK");
    }

    private void CreateEnemyUnit(string assetPath, ComboSkillResource.ComboWithType comboType, int level, int newId)
    {
        EnemyData enemy = ScriptableObject.CreateInstance<EnemyData>();

        enemy.comboChar = new ComboSkillResource.ComboWithType
        {
            nameCombo = comboType.nameCombo,
            typeCombo = comboType.typeCombo,
            monPhai = comboType.monPhai,
            eTypeSkill = comboType.eTypeSkill,
            comboWithDaiCanhGioi = comboType.comboWithDaiCanhGioi
        };

        enemy.charStatsCreateRef = new CharacterStatsSecure();
        enemy.charStatsCreateRef.idChar = new SecureInt(newId);
        enemy.charStatsCreateRef.level = new SecureInt(level);
        enemy.charStatsCreateRef.monPhai = comboType.monPhai;
        enemy.charStatsCreateRef.Gender = (Gender)Random.Range(0, 2);

        // Stats tăng dần theo Level
        enemy.charStatsCreateRef.hpMax = new SecureInt(Random.Range(50, 100) + (level * 25));
        enemy.charStatsCreateRef.hp = new SecureInt(enemy.charStatsCreateRef.hpMax.Value);
        enemy.charStatsCreateRef.tuChat = new SecureInt(Random.Range(60, 120));
        enemy.charStatsCreateRef.ngoTinh = new SecureInt(Random.Range(60, 120));
        enemy.charStatsCreateRef.thoNguyen = new SecureInt(100);

        // Tự động tạo tên quái vật phong cách Tu Tiên
        ApplyElementalStats(enemy.charStatsCreateRef, comboType.eTypeSkill);

        AssetDatabase.CreateAsset(enemy, assetPath);
        EditorUtility.SetDirty(enemy);
    }

    // Giữ nguyên các hàm ApplyElementalStats và EnsureFolderExists như cũ...
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