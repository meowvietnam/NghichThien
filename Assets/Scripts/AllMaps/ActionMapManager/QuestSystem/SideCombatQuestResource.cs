using AllMaps;
using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using Network;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(SideCombatQuestResource), menuName = "Action Map/Side Quest/" + nameof(SideCombatQuestResource))]
public class SideCombatQuestResource : SideQuestResource
{
    public override ETypeSideQuest Type => ETypeSideQuest.Combat;

    [SerializeField] public EnemyData[] listEnemy;
    [SerializeField] public NPCData[] npcDatas;
    [SerializeField] public bool isSideCombatQuest2;
    public override List<string> CompleteSideQuest => GetCompleteSideQuest();
    public List<string> GetCompleteSideQuest()
    {
        if (isSideCombatQuest2)
        {
            return completeQuest2;
        }    
        return completeQuest1;
    }
    private List<string> completeQuest1 = new List<string>
    {
        "Ha ha ha! Khá khen cho nghĩa cử hiệp đạo của ngươi! Đúng là anh hùng xuất thiếu niên, tương lai vô hạn.",
        "Đây là chút báu vật ta vô tình nhặt được, coi như trả ơn cứu mạng này. Cầm lấy đi!"
    };
    private List<string> completeQuest2 = new List<string>
    {
        "Ha ha ha! Tuyệt vời! Con ác thú này cuối cùng cũng phải gục ngã trước sự phối hợp ăn ý của chúng ta.",
        "Đúng là không bõ công mạo hiểm một chuyến. Trận này ngươi góp công lớn nhất, chiếc rương báu này thuộc về ngươi!",
    };
    public List<string> GetStartSideQuest()
    {
        if (isSideCombatQuest2)
        {
            return startDialog2;
        }
        return startDialog;
    }
    private List<string> startDialog = new List<string>
    {
            "Đạo hữu cứu mạng! Đám tà tu này muốn giết người cướp của, xin hãy ra tay tương trợ!",
    };
    private List<string> startDialog2 = new List<string>
    {
        $"Ta nghe đồn sâu trong hang có viễn cổ bảo vật, nhưng lại bị một con hung thú chiếm giữ. Một mình ta không tự tin lắm...",
        $"Ngươi có muốn cùng ta liên thủ tiến vào săn quái đoạt bảo không? Được ăn cả ngã về không!",
    };
    public override void ActiveEventMap(Map map)
    {
        base.ActiveEventMap(map);
     
        var targetLevel = Client.Instance.CharacterData.Progression.Level -1;
        targetLevel = Mathf.Clamp(targetLevel, 1, int.MaxValue);
        var charRandom = BaseEnemyCombatResource.GetNPCDataCombatWithTargetLevel(npcDatas, targetLevel);
        var enemyRandom = isSideCombatQuest2 ? BaseEnemyCombatResource.GetNPCDataCombatWithTargetLevel(listEnemy,charRandom.charStats.level) : BaseEnemyCombatResource.GetNPCDataCombatWithTargetLevel(npcDatas, charRandom.charStats.level);

        Pool.Get<SideQuestManager>().InitShowQuestHaveChar(() =>
        {
            Pool.Get<GameManager>().GoToCombat(new GameManager.CharCombatInit { eTypeVisual = enemyRandom.eTypeVisualChar, charStats = enemyRandom.charStats }, ETypeCombat.AllMaps);
        }, GetStartSideQuest(),charRandom.charStats );    
    }
    public override void QuestDone()
    {
        var character = Pool.Get<SideQuestManager>().characterStats;
        Observer.Notify(EventList.Network.COMPLETE_SIDEQUEST, this, Type , character.idChar);
        NotifyGroup.Instance.Show($"Hảo cảm của {character.nameUser} tăng thêm 10");
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
    [Button]
    private void OnUpdateDataNPCData()
    {
#if UNITY_EDITOR
        // Tìm tất cả GUID của các asset có type là CongPhap
        string[] guids = AssetDatabase.FindAssets("t:NPCData");

        npcDatas = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<NPCData>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(asset => asset != null)
            .ToArray();

        EditorUtility.SetDirty(this);
        Debug.Log($"Đã cập nhật {npcDatas.Length} NPC vào mảng.");
#endif
    }

    
}
