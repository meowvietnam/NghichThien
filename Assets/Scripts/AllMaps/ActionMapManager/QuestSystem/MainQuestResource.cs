using AllMaps;
using CreateCharacter;
using DG.Tweening;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using Network.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MainQuestResource), menuName = "Action Map/Main Quest/" + nameof(MainQuestResource))]
public class MainQuestResource : QuestResource, INetworkEvent.ICompleteMainQuest , IMemorySerializable<MainQuestResource>
{
    public override List<string> CompleteQuest => completeQuest;
    public List<string> completeQuest;
    public List<string> startQuest;
    public virtual string RequireActive => $"Tu vi đạt tới {GameManager.levelWithText.GetValueOrDefault(levelRequire)} mới có thể tiếp tục cốt truyện.";
    public List<AudioClip> voiceStartQuest;
    public List<AudioClip> voiceComplete;
    public List<Sprite> spriteListAvatarStart;
    public List<Sprite> spriteListAvatarDone;
    public NPCData npcVisualDialog;
    public Sprite spriteDoneQuest;
    public RewardSnapshot rewardRef;
    public NPCData[] enemyNeedCombat;
    public string nameCharDialog;
    public float scaleThisChar = 3;
    public virtual int[] IdNode => idNode;
    [SerializeField]private int[] idNode;
    public string descDoneQuest;
    public Sprite spriteCombat;
    public int tipLevel => enemyNeedCombat.Length<=0 ? 1 : enemyNeedCombat.Max(x=>x.charStats.level.Value);
    public int levelRequire = 1;
    public override bool CanActiveEventMap()
    {
        return Client.Instance.CharacterData.Progression.Level >= levelRequire;
    }
    public override void QuestDoneData()
    {
        Debug.Log("Quest done data");
        Observer.Notify(EventList.Network.COMPLETE_MAINQUEST, this, Pool.Get<MainQuestManager>().currentMainQuest.Value);
        GetDialogVisual(() =>
        {
            string detailedDesc = !string.IsNullOrEmpty(descDoneQuest) ? $"\n{descDoneQuest}" : "";

            Pool.Get<EventRewardPanel>().InitShowEventReward(rewardRef, $"Hoàn thành chương {Id}{detailedDesc}");
        },completeQuest,spriteListAvatarDone,voiceComplete,spriteDoneQuest);
      
    }

    public void FailMainQuest(string message)
    {
        NotifyGroup.Instance.Show(message);

    }


    public void SuccessCompleteMainQuest()
    {
        var charData = Client.Instance.CharacterData;
        var reward = Pool.Get<MainQuestManager>().CurrentMainQuest.rewardRef;
        Debug.Log("Success complete main quest event " + $"linh thạch = {reward.Money} , moneyvip = {reward.MoneyVip}"   );

        charData.MoneyVip += reward.MoneyVip;
        charData.Stats.LinhThach += reward.Money;
        var listItemRef = reward.ItemRefs;
        foreach (var item in listItemRef)
        {
            charData.InventoryManager.Add(item);

        }

        Pool.Get<MainQuestManager>().currentMainQuest.Value = Client.Instance.CharacterData.QuestManager.CurrentMainQuest;
        Pool.Get<MainQuestManager>().CheckSpawnQuest();
    }

    public override void ActiveEventMap(Map map)
    {
        base.ActiveEventMap(map);
        if (CanActiveEventMap())
        {
          if(Client.Instance.CharacterData.Progression.Level < tipLevel)
          {
                Pool.Get<PopupConfirmAction>().ShowPopConfirmAction($"Ngươi nên đạt tới tu vi {GameManager.levelWithText.GetValueOrDefault(tipLevel)} để tránh nhận phải thất bại. Vẫn tiếp tục chứ?", () =>
                {
                    ActiveEventMainQuest();
                });

          }   
          else
          {
                ActiveEventMainQuest();
          }    
        }
        else
        {
            Pool.Get<PopupConfirmAction>().ShowPopConfirmAction($"{RequireActive}", null);
        }
       
    }
    protected virtual void ActiveEventMainQuest()
    {
        bool isCombat = enemyNeedCombat.Length > 0;
        if(isCombat)
        {
            Pool.Get<QuestManager>().enemyCombats = new List<NPCData>(enemyNeedCombat.ToList());

        }
        GetDialogVisual(() =>
        {
            if (isCombat)
            {
                Pool.Get<GameManager>().GoToPrepareCombat(new GameManager.CharCombatInit { eTypeVisual = enemyNeedCombat[0].eTypeVisualChar, charStats = enemyNeedCombat[0].charStats }, ETypeCombat.AllMaps);

            }
            else
            {
                DOVirtual.DelayedCall(0.01f, () =>
                {
                    QuestDoneUI();
                });
            }

        },startQuest,spriteListAvatarStart,voiceStartQuest);
        if (isCombat)
        {
            Pool.Get<GameManager>().SetBGCombat(spriteCombat);

        }
    }
    private void GetDialogVisual(Action actionEnd , List<string> contentCheck , List<Sprite> spriteCheck , List<AudioClip> voices , Sprite spriteDialog = null)
    {
       if(contentCheck.Count > 0)
       {
            if (spriteCheck.Count > 0)
            {
                SayDialogManager.Instance.ShowDialogWithAvatar(nameCharDialog, spriteCheck, contentCheck, spriteDialog, voices, () =>
                {
                    actionEnd?.Invoke();

                }, true, scaleThisChar);
            }
            else if (npcVisualDialog != null)
            {
                SayDialogManager.Instance.ShowDialogWithSkin(npcVisualDialog.charStats.nameUser, npcVisualDialog.charStats.Gender, npcVisualDialog.charStats.Skins, npcVisualDialog.charStats.Colors, contentCheck, spriteDialog, voices, () =>
                {
                    actionEnd?.Invoke();

                }, true);
            }
       }   
       else
       {
            actionEnd?.Invoke();

       }
    }    

    public override void QuestDoneUI()
    {
        Pool.Get<MainQuestManager>().MainQuestDone();
    }

    public void Deserialize(IReader reader, ref MainQuestResource value)
    {
        BaseData baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
        value.levelRequire = reader.Read<int>();
        value.rewardRef = reader.Read<RewardSnapshot>();
    }
    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + levelRequire.ByteSize() + rewardRef.ByteSize();
    }
    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.Write(levelRequire);
        writer.Write(rewardRef);
    }
}
