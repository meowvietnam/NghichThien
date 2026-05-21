using AllMaps;
using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using Network;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class QuestResource : ActionMapResourceSO
{
    public override void ActiveEventMap(Map map)
    {
        Pool.Get<SideQuestManager>().InitQuest(this);
    }
    public virtual List<string> CompleteSideQuest { get; }

    public abstract void QuestDone();
}
public abstract class SideQuestResource : QuestResource,INetworkEvent.ICompleteSideQuest
{
    [SerializeField] public NPCData[] npcDatas => Pool.Get<NPCManager>().npcDatas;

    public enum ETypeSideQuest
    {
        Quiz,
        Combat,
        Delivery,

    }
    public virtual ETypeSideQuest Type { get; }
    public virtual List<CharacterStatsSecure> listNPCInteractive { get; }
    public List<int> listIdNPCInteractive =>
        listNPCInteractive != null
            ? listNPCInteractive.Select(npc => npc.idChar.Value).ToList()
            : new List<int>(); // Nếu listNPC null thì trả về list rỗng để tránh lỗi NullReferenceException
    public override void ActiveEventMap(Map map)
    {
        base.ActiveEventMap(map);
        var targetLevel = Client.Instance.CharacterData.Progression.Level;
        targetLevel = Mathf.Clamp(targetLevel, 1, int.MaxValue);
        var charRandom = GetNPCDataSideQuestWithTargetLevel(npcDatas, targetLevel);
        charRandom.charStats.idNode = map.Id;
        Pool.Get<UINPCJiaoHu>().OnReloadNpcThisNode();
        Pool.Get<SideQuestManager>().SetCharProviderQuest(charRandom.charStats);
    }
    public override void QuestDone()
    {
        Observer.Notify(EventList.Network.COMPLETE_SIDEQUEST, this, Type, listIdNPCInteractive);
        foreach (var item in listNPCInteractive)
        {
            NotifyGroup.Instance.Show($"Hảo cảm của {item.nameUser} tăng thêm 10");

        }

    }
    public NPCData GetNPCDataSideQuestWithTargetLevel(NPCData[] listEnemy, int levelTarget)
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
            // 75% cùng hoặc dưới cảnh giới (playerLevel - 1 -> playerLevel)
            targetLevel = UnityEngine.Random.Range(playerLevel-1, playerLevel + 1);
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
    public virtual void SuccessCompleteSideQuest(int moneyVip)
    {
        Pool.Get<EventRewardPanel>().InitShowEventReward(new RewardSnapshot { MoneyVip = moneyVip }, $"{CompleteSideQuest[CompleteSideQuest.Count-1]}");

    }

    public void FailCompleteSideQuest(string message)
    {
    }
}
public class MainQuestResource : QuestResource
{
 
    public override void QuestDone()
    {

    }    
}