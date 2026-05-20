using Base;
using LongDH.LongDHModules;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestManager : ActionEnterMapManager
{
    [SerializeField] protected QuestResource currentQuest;
    public string nameChar;
    public CharacterStatsSecure characterStats;
    public void SetSkinSideQuestCurrent(CharacterStatsSecure characterStats)
    {
       this.characterStats = characterStats;
    }
    [SerializeField] public Dictionary<string, string> skins => characterStats.Skins;
    [SerializeField] public Dictionary<string, Color> color => characterStats.Colors;
    [SerializeField] public Gender gender => characterStats.Gender;
    public virtual void InitQuest(QuestResource quest)
    {
        this.currentQuest = quest;
    }
    public void QuestCombatTrigger(GameManager.CharCombatInit charCombat, ETypeCombat typeCombat, ECombatResult result)
    {
        if (typeCombat == ETypeCombat.AllMaps && result == ECombatResult.Win)
        {
            DoneQuest();

        }
    }
    public void DoneQuest()
    {
        SayDialogManager.Instance.ShowDialog(nameChar, gender, skins, color, currentQuest.CompleteSideQuest, null, null, () => {
            currentQuest.QuestDone();
            // Fake Reward
            Pool.Get<EventRewardPanel>().InitShowEventReward(new RewardSnapshot { MoneyVip = 50 }, $"{currentQuest.CompleteSideQuest[currentQuest.CompleteSideQuest.Count - 1]}");
            currentQuest = null;



        });
    }
    public void InitShowQuestRandomSkin(Action actionComplete, List<string> startDialog)
    {
        var skinRandom = SayDialogManager.Instance.ShowDialogRandomSkin(nameChar, startDialog, null, null, () =>
        {
            actionComplete?.Invoke();
        }, false);
        CharacterStatsSecure charRandom = new CharacterStatsSecure();
        foreach (var kvp in skinRandom.skin)
        {
            charRandom.skinList.Add(new CharacterStatsSecure.SkinData { key = kvp.Key, value = kvp.Value });
        }
        foreach (var kvp in skinRandom.color)
        {
            charRandom.colorList.Add(new CharacterStatsSecure.ColorData { key = kvp.Key, value = kvp.Value });
        }
        charRandom.Gender = skinRandom.gender;
        SetSkinSideQuestCurrent(charRandom);
    }
    public void InitShowQuestHaveChar(Action actionComplete, List<string> startDialog , CharacterStatsSecure character)
    {
        nameChar = character.nameUser;
        SayDialogManager.Instance.ShowDialog(character.nameUser,character.Gender,character.Skins,character.Colors, startDialog, null, null, () =>
        {
            actionComplete?.Invoke();
        });

        SetSkinSideQuestCurrent(character);
    }
   
}
