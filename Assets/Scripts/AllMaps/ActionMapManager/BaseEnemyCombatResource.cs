using AllMaps;
using LongDH.LongDHModules;
using Network;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using ComboSkillResource.NPCData;
using LongDHModules;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(BaseEnemyCombatResource), menuName = "Action Map/" + nameof(BaseEnemyCombatResource))]

public class BaseEnemyCombatResource : ActionMapResourceSO
{
    [SerializeField]public EnemyData[] listEnemy;
    public Sprite[] spriteCombat;

    public override void ActiveEventMap(Map map)
    {
        var enemy = GetEnemyDataCombatWithPlayerLevel(listEnemy);
        Pool.Get<GameManager>().GoToPrepareCombat(new GameManager.CharCombatInit { eTypeVisual = enemy.eTypeVisualChar, charStats = enemy.charStats }, ETypeCombat.AllMaps);
        Pool.Get<GameManager>().SetBGCombat(spriteCombat[Random.Range(0, spriteCombat.Length)]);

    }
    public NPCData GetEnemyDataCombatWithTargetLevel(NPCData[] listEnemy,int levelTarget)
    {
        int playerLevel = levelTarget;

        int targetLevel = playerLevel;
        float rand = UnityEngine.Random.value; // Trả về giá trị từ 0.0 đến 1.0
        int maxLevelConfig = Pool.Get<GameManager>().inforLevels.Length;

        if (rand <= 0.50f)
        {
            // 50% bằng chính xác level của player
            targetLevel = playerLevel;
        }
        else if (rand <= 0.75f)
        {
            // 25% vượt cấp (từ playerLevel + 1 đến playerLevel + 2)
            // +3 ở max vì Random.Range cho int không lấy giá trị cuối
            int minOver = playerLevel + 1;
            int maxOver = playerLevel + 3;

            // Giới hạn không vượt quá max level của game
            minOver = minOver > maxLevelConfig ? maxLevelConfig : minOver;
            maxOver = maxOver > maxLevelConfig ? maxLevelConfig : maxOver;

            if (minOver == maxOver)
            {
                targetLevel = minOver;
            }
            else
            {
                targetLevel = UnityEngine.Random.Range(minOver, maxOver);
            }
        }
        else
        {
            // 25% dưới cảnh giới (từ 1 đến playerLevel - 1)
            // Để lấy được đến playerLevel - 1 thì max của Random.Range phải là playerLevel
            if (playerLevel > 1)
            {
                targetLevel = UnityEngine.Random.Range(1, playerLevel);
            }
            else
            {
                targetLevel = 1; // Nếu player đang ở level 1 thì không thể thấp hơn được nữa
            }
        }

        // Bọc thêm một lớp bảo mật cuối cùng để đảm bảo targetLevel luôn hợp lệ
        targetLevel = Mathf.Clamp(targetLevel, 1, maxLevelConfig);

        // 3. Tìm danh sách enemy phù hợp với level này
        var possibleEnemies = listEnemy
            .Where(e => e != null &&
                        e.charStatsCreateRef != null &&
                        e.charStatsCreateRef.level.Value == targetLevel)
            .ToList();

        // Bảo hiểm: Nếu level đó không có quái, lấy quái bằng level player
        if (possibleEnemies.Count == 0)
        {
            possibleEnemies = listEnemy
                .Where(e => e != null && e.charStatsCreateRef.level.Value == playerLevel)
                .ToList();
        }

        // Chọn ngẫu nhiên 1 con từ danh sách phù hợp
        if (possibleEnemies.Count > 0)
        {
            NPCData randomEnemy = possibleEnemies[UnityEngine.Random.Range(0, possibleEnemies.Count)];
            return randomEnemy;
        }
        return null;
    }
    public NPCData GetEnemyDataCombatWithPlayerLevel(NPCData[] listEnemy)
    {
        int playerLevel = Client.Instance.CharacterData.Progression.Level;

      
        return GetEnemyDataCombatWithTargetLevel(listEnemy, playerLevel);
    }
    [Button]
    private void OnUpdateDataEnemyData()
    {
#if UNITY_EDITOR
        // 1. Điền CHÍNH XÁC đường dẫn thư mục chứa Enemy của bạn tại đây (Bắt đầu bằng Assets/...)
        string[] searchFolders = new string[] { "Assets/Scripts/ComboSkillResource/EnemyData" };

        // 2. Truyền searchFolders vào làm tham số thứ 2 để giới hạn vùng tìm kiếm
        string[] guids = AssetDatabase.FindAssets("t:EnemyData", searchFolders);

        listEnemy = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<EnemyData>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(asset => asset != null)
            .ToArray();

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets(); // Lưu lại thay đổi xuống ổ cứng ngay lập tức

        Debug.Log($"Đã cập nhật {listEnemy.Length} Enemy từ folder chỉ định vào mảng.");
#endif
    }
}
