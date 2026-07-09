using System;
using Base;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using LongDH.LongDHModules;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ComboSkillResource.NPCData;
using LongDHModules;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : BaseBehaviour , IUISingleton
{
    public List<NPCData> enemyCombats;
    [SerializeField] public QuestResource currentQuest;
   
   
    public virtual void InitQuest(QuestResource quest)
    {
        this.currentQuest = quest;
    }
    private void OnCheckSceneToTrigger(Scene scene, LoadSceneMode mode)
    {
        // Kiểm tra xem ĐÂY CÓ PHẢI là scene bạn muốn hay không (Ví dụ tên scene là "MainMap")
        if (scene.name == "AllMaps")
        {
            Debug.Log($"Đã vào đúng scene: {scene.name}. Tiến hành kích hoạt logic!");

            // Thực hiện logic của bạn tại đây
            // Pool.Get<QuestManager>().QuestCombatTrigger(...);

            // Hủy đăng ký ngay sau khi xử lý xong (nếu chỉ muốn chạy 1 lần)
            LoadAfterBackAllMaps();
        }
        else
        {
            Debug.Log($"Vào scene {scene.name}, không phải scene MainMap nên bỏ qua.");
        }
    }
    public virtual void LoadAfterBackAllMaps()
    {
        StartCoroutine(DelayTime(0.01f));

    }
    private IEnumerator DelayTime(float time)
    {
        yield return new WaitForSeconds(time);
        var gameManager = Pool.Get<GameManager>();
        QuestCombatTrigger(gameManager.enemyCombat, gameManager.currentETypeCombat, gameManager.currentECombatResult);
    }    
    
    public void QuestCombatTrigger(GameManager.CharCombatInit charCombat, ETypeCombat typeCombat, ETypeResultCombat result)
    {
        Debug.Log(" vào QuestCombatTrigger");
        if (typeCombat == ETypeCombat.AllMaps && result == ETypeResultCombat.Win)
        {
            if (enemyCombats.Any(x => x.charStats.idChar == charCombat.charStats.idChar))
            {
                enemyCombats.Remove(enemyCombats.FirstOrDefault(x => x.charStats.idChar == charCombat.charStats.idChar));
                if (enemyCombats.Count <= 0)
                {
                    QuestDone();

                    enemyCombats.Clear();

                }
                else
                {
                    Pool.Get<GameManager>().GoToPrepareCombat(new GameManager.CharCombatInit { eTypeVisual = enemyCombats[0].eTypeVisualChar, charStats = enemyCombats[0].charStats }, ETypeCombat.AllMaps);


                }
            }
               
        }
        
    }
    public void QuestDone()
    {
        if (currentQuest != null)
        {
            currentQuest.QuestDoneUI();
        }
    }

    public void Init()
    {
        Pool.Set(this);
        SceneManager.sceneLoaded += OnCheckSceneToTrigger;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnCheckSceneToTrigger;
    }
}
