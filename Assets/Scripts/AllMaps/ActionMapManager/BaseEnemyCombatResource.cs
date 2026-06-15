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
        float rand = UnityEngine.Random.value;
        int maxTarget75Percent = playerLevel + 1;
        int maxTarget25Percent = playerLevel + 3;

        maxTarget75Percent = maxTarget75Percent > Pool.Get<GameManager>().inforLevels.Length ? Pool.Get<GameManager>().inforLevels.Length : maxTarget75Percent;
        maxTarget25Percent = maxTarget25Percent > Pool.Get<GameManager>().inforLevels.Length ? Pool.Get<GameManager>().inforLevels.Length : maxTarget25Percent;
        // Xác định level mục tiêu cho vị trí này
        if (rand <= 0.25f)
        {
            // 25% vượt cấp (+1 hoặc +2)
            targetLevel = UnityEngine.Random.Range(maxTarget75Percent, maxTarget25Percent);
        }
        else
        {
            // 75% cùng hoặc dưới cảnh giới (1 -> playerLevel)
            targetLevel = UnityEngine.Random.Range(1, maxTarget75Percent);
        }

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
