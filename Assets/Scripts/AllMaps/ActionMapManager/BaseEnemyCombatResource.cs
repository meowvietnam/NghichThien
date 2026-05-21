using AllMaps;
using LongDH.LongDHModules;
using Network;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(BaseEnemyCombatResource), menuName = "Action Map/" + nameof(BaseEnemyCombatResource))]

public class BaseEnemyCombatResource : ActionMapResourceSO
{
    [SerializeField]public EnemyData[] listEnemy;
    public override void ActiveEventMap(Map map)
    {
        var enemy = GetEnemyDataCombatWithPlayerLevel(listEnemy);
        Pool.Get<GameManager>().GoToCombat(new GameManager.CharCombatInit { eTypeVisual = enemy.eTypeVisualChar, charStats = enemy.charStats }, ETypeCombat.AllMaps);

    }
    public NPCData GetEnemyDataCombatWithTargetLevel(NPCData[] listEnemy,int levelTarget)
    {
        int playerLevel = levelTarget;

        int targetLevel = playerLevel;
        float rand = UnityEngine.Random.value;

        // Xác định level mục tiêu cho vị trí này
        if (rand <= 0.25f)
        {
            // 25% vượt cấp (+1 hoặc +2)
            targetLevel = UnityEngine.Random.Range(playerLevel + 1, playerLevel + 3);
        }
        else
        {
            // 75% cùng hoặc dưới cảnh giới (1 -> playerLevel)
            targetLevel = UnityEngine.Random.Range(1, playerLevel + 1);
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
        // Tìm tất cả GUID của các asset có type là CongPhap
        string[] guids = AssetDatabase.FindAssets("t:EnemyData");

        listEnemy = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<EnemyData>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(asset => asset != null)
            .ToArray();

        EditorUtility.SetDirty(this);
        Debug.Log($"Đã cập nhật {listEnemy.Length} NPC vào mảng.");
#endif
    }
}
