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
    public override List<CharacterStatsSecure> listNPCInteractive => new List<CharacterStatsSecure> { Pool.Get<SideQuestManager>().characterProviderQuest };
    public Sprite[] spriteCombat;

    public override ETypeSideQuest Type => ETypeSideQuest.Combat;

    [SerializeField] public EnemyData[] listEnemy;
    [SerializeField] public bool isSideCombatQuest2;
    public override List<string> CompleteQuest => GetCompleteSideQuest();
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


        var charProvider = Pool.Get<SideQuestManager>().characterProviderQuest;
        var enemyRandom = isSideCombatQuest2 ? GetNPCDataSideQuestWithTargetLevel(listEnemy, charProvider.level) : GetNPCDataSideQuestWithTargetLevel(npcDatas, charProvider.level);
        Pool.Get<QuestManager>().enemyCombats = new List<NPCData> { enemyRandom};
        Pool.Get<SideQuestManager>().InitShowQuestHaveChar(() =>
        {
            Pool.Get<GameManager>().GoToPrepareCombat(new GameManager.CharCombatInit { eTypeVisual = enemyRandom.eTypeVisualChar, charStats = enemyRandom.charStats }, ETypeCombat.AllMaps);
        }, GetStartSideQuest(), charProvider);
        Pool.Get<GameManager>().SetBGCombat(spriteCombat[Random.Range(0, spriteCombat.Length)]);

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
    public override void QuestDoneUI()
    {
        var sideQManager = Pool.Get<SideQuestManager>();
        sideQManager.DoneQuest(sideQManager.characterProviderQuest, () =>
        {
            sideQManager.characterProviderQuest = null;
        });
    }
}
