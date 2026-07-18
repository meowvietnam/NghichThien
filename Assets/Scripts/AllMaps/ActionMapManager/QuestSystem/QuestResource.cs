using AllMaps;
using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using Network;
using System.Collections.Generic;
using System.Linq;
using AllMaps.EventReward;
using ComboSkillResource.NPCData;
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
        if(map != null)
        {
            charRandom.charStats.idNode = map.Id;
        }    
        Pool.Get<UINPCJiaoHu>().OnReloadNpcThisNode();
        Pool.Get<SideQuestManager>().SetCharProviderQuest(charRandom.charStats);
    }
    public override void QuestDoneData()
    {
        Observer.Notify(EventList.Network.COMPLETE_SIDEQUEST, this, listIdNPCInteractive);
        foreach (var item in listNPCInteractive)
        {
            Client.Instance.CharacterData.Social.RelationshipAddValue((ushort)item.idChar,10);
            NotifyGroup.Instance.Show($"Hảo cảm của {item.nameUser} tăng thêm 10"); 
          

        }
        Client.Instance.CharacterData.QuestManager.CurrentCountSideQuest += 1;
        int newTamCanh = Client.Instance.CharacterData.Stats.TamCanh + Client.Instance.CharacterData.Progression.Level;
        NotifyGroup.Instance.Show($"Tâm cảnh của bạn tăng thêm {Client.Instance.CharacterData.Progression.Level}");
        Client.Instance.CharacterData.Stats.SetValue(CreateCharacter.CharStats.StatType.TamCanh, newTamCanh);
        Pool.Get<UINPCJiaoHu>().OnReloadNpcThisNode();


    }
    public NPCData GetNPCDataSideQuestWithTargetLevel(NPCData[] listEnemy, int levelTarget)
    {
        int playerLevel = levelTarget;
        int targetLevel = playerLevel;
        float rand = UnityEngine.Random.value;
        int maxLevelConfig = Pool.Get<GameManager>().inforLevels.Length;

        // --- TỰ ĐỊNH NGHĨA KHOẢNG CỦA BẠN Ở ĐÂY ---
        int minUnder = playerLevel - 2; // Ví dụ: dưới cấp tối đa là thấp hơn 2 cấp (thay vì cố định là 1)
        int maxOver = playerLevel + 2; // Ví dụ: vượt cấp tối đa là cao hơn 2 cấp

        // Giới hạn các biến để không vượt quá giới hạn của game (1 -> maxLevelConfig)
        minUnder = Mathf.Clamp(minUnder, 1, maxLevelConfig);
        maxOver = Mathf.Clamp(maxOver, 1, maxLevelConfig);
        // ------------------------------------------

        if (rand <= 0.50f)
        {
            // 1. [50%] Bằng chính xác level của player
            targetLevel = playerLevel;
        }
        else if (rand <= 0.75f)
        {
            // 2. [25%] Vượt cấp (Từ playerLevel + 1 đến maxOver)
            int startOver = playerLevel + 1;

            if (startOver > maxOver)
            {
                targetLevel = maxOver; // Nếu đã chạm trần thì lấy luôn maxOver
            }
            else
            {
                // Cộng 1 vào tham số max vì Random.Range của số nguyên không lấy giá trị cuối
                targetLevel = UnityEngine.Random.Range(startOver, maxOver + 1);
            }
        }
        else
        {
            // 3. [25%] Dưới cấp (Từ minUnder đến playerLevel - 1)
            int endUnder = playerLevel - 1;

            if (minUnder > endUnder)
            {
                targetLevel = minUnder; // Nếu đã chạm sàn thì lấy luôn minUnder
            }
            else
            {
                // Cộng 1 vào endUnder để đảm bảo lấy được giá trị (playerLevel - 1)
                targetLevel = UnityEngine.Random.Range(minUnder, endUnder + 1);
            }
        }

        // Chốt chặn cuối cùng bảo vệ mảng inforLevels
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
    public virtual void SuccessCompleteSideQuest(int moneyVip, int linhThach)
    {
        Pool.Get<EventRewardPanel>().InitShowEventReward(new RewardSnapshot { MoneyVip = moneyVip , Money = linhThach }, $"{CompleteQuest[CompleteQuest.Count-1]}");
       
    }

    public void FailCompleteSideQuest(string message)
    {
        NotifyGroup.Instance.Show(message);
    }
}
