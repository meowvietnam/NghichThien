using AllMaps;
using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using Network;
using System.Collections.Generic;
using System.Linq;
using LongDHModules;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class QuestResource : ActionMapResourceSO
{
   
    public virtual List<string> CompleteQuest { get; }
    public virtual bool CanActiveEventMap()
    {
        return true;
    }
    public override void ActiveEventMap(Map map)
    {
        if (CanActiveEventMap())
        {
            Pool.Get<QuestManager>().InitQuest(this);


        }
    }
    public abstract void QuestDoneData();
    public abstract void QuestDoneUI();

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
    public override void QuestDoneData()
    {
        Observer.Notify(EventList.Network.COMPLETE_SIDEQUEST, this, listIdNPCInteractive);
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
        int minTarget75Percent = playerLevel - 1;
        int maxTarget75Percent = playerLevel + 1;
        int maxTarget25Percent = playerLevel + 3;

        minTarget75Percent = minTarget75Percent <= 0 ? 1 : minTarget75Percent;
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
            // 75% cùng hoặc dưới cảnh giới (playerLevel - 1 -> playerLevel)
            targetLevel = UnityEngine.Random.Range(minTarget75Percent, maxTarget75Percent);
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
        Pool.Get<EventRewardPanel>().InitShowEventReward(new RewardSnapshot { MoneyVip = moneyVip }, $"{CompleteQuest[CompleteQuest.Count-1]}");

    }

    public void FailCompleteSideQuest(string message)
    {
        NotifyGroup.Instance.Show(message);
    }
}
