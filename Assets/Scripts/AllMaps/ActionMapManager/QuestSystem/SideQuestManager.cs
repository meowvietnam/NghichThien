using AllMaps;
using Base;
using DG.Tweening;
using LongDH.LongDHModules;
using LongDH.Script.ObjectPool;
using Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LongDHModules;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;


public class SideQuestManager : ActionEnterMapManager
{
   
   
    public override string fileSaveMapNode => "SaveSideQuestMap";

    public override int TimeMonthRandom => 5;

    public override string fileSaveTime => "OldTimeRandomSideQuest";

    public override int CountRandom => 10 - Client.Instance.CharacterData.QuestManager.CurrentCountSideQuest;
    


    [Header("Quiz Side Quest Visual")]
    [SerializeField] private AnswerQuizQuest answerPrefab;
    [SerializeField] private SideQuizQuestResource.Quiz currentQuiz;
   // [Header("Combat Side Quest Visual")]

    // Thay vì khai báo biến List, hãy viết thành hàm trả về List
   
    [SerializeField] private Popup quizQuestParent;
    [Header("Delivery Side Quest Visual")]
    public CharacterStatsSecure characterStatsTakeDeliver;
    public Map mapTakeDeliver;
    public override void Init()
    {
        base.Init();
        Observer.AddObserver(EventList.GamePlay.USER_ENTER_MAP, USER_ENTER_MAP);

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Observer.RemoveObserver(EventList.GamePlay.USER_ENTER_MAP, USER_ENTER_MAP);
    }

    public string nameChar;
    public CharacterStatsSecure characterProviderQuest;
    public void SetCharProviderQuest(CharacterStatsSecure characterStats)
    {
        this.characterProviderQuest = characterStats;
    }
    [SerializeField] public Dictionary<string, string> skins => characterProviderQuest.Skins;
    [SerializeField] public Dictionary<string, Color> color => characterProviderQuest.Colors;
    [SerializeField] public Gender gender => characterProviderQuest.Gender;
   

    public void DoneQuest()
    {
        Pool.Get<QuestManager>().QuestDone();
    }
    public void DoneQuest(CharacterStatsSecure characterShow, Action actionDoneDialog)
    {
        SayDialogManager.Instance.ShowDialogWithSkin(characterShow.nameUser, characterShow.Gender, characterShow.Skins, characterShow.Colors, Pool.Get<QuestManager>().currentQuest.CompleteQuest, null, null, () => {
            Pool.Get<QuestManager>().currentQuest.QuestDoneData();
            // Fake Reward
            Pool.Get<QuestManager>().currentQuest = null;
            actionDoneDialog?.Invoke();


        });
    }

    public void InitShowQuestHaveChar(Action actionComplete, List<string> startDialog, CharacterStatsSecure character, bool isHide = true)
    {
        nameChar = character.nameUser;
        SayDialogManager.Instance.ShowDialogWithSkin(character.nameUser, character.Gender, character.Skins, character.Colors, startDialog, null, null, () =>
        {
            actionComplete?.Invoke();
        }, isHide);

    }
    public void USER_ENTER_MAP(params object[] data)
    {
        Debug.Log("Reset");
        Map map = (Map)data[0];

        if (map.actionMap != null && map.actionMap is SideQuestResource)
        {
            Pool.Get<QuestManager>().currentQuest = null;
            characterProviderQuest = null;
           
        }
        if (mapTakeDeliver == null) return;
        if(map.actionMap != null && map.actionMap is SideQuestResource)
        {
            mapTakeDeliver.ResetDataAction();
            characterStatsTakeDeliver = null;
        }    


        Debug.Log(" vào QuestDeliveryTrigger 2");

        if (map.Id == mapTakeDeliver.Id)
        {
            Debug.Log(" vào QuestDeliveryTrigger 3");

            characterStatsTakeDeliver.idNode = mapTakeDeliver.Id;
            Pool.Get<UINPCJiaoHu>().OnReloadNpcThisNode();
            DoneQuest();
        }


    }

    public void QuizQuestInitShow(SideQuizQuestResource.Quiz quiz)
    {
        if(Pool.Get<QuestManager>().currentQuest is SideQuizQuestResource quizQuestResource)
        {
            quizQuestParent.Show();
            currentQuiz = quiz;
            List<AnswerQuizQuest> listBtnAnswer = new List<AnswerQuizQuest>();
            InitShowQuestHaveChar(() =>
            {
                for (int i = 0; i < listBtnAnswer.Count; i++)
                {
                    var answerBtn = listBtnAnswer[i];
                    answerBtn.transform.DOScale(Vector3.one, 0.5f);

                }
            }, quizQuestResource.GetQuizDialogs(currentQuiz.quiz),characterProviderQuest,false);
           
            for (int i = 0; i < currentQuiz.answer.Length; i++)
            {
                var answerBtn = answerPrefab.Spawn(answerPrefab.transform.parent);
                answerBtn.gameObject.SetActive(true);
                answerBtn.transform.localScale = Vector3.zero;
                answerBtn.Init(i, currentQuiz.answer[i]);
                listBtnAnswer.Add(answerBtn);

            }
            listBtnAnswer = listBtnAnswer.OrderBy(x => UnityEngine.Random.value).ToList();
        }    

    
        



    }
    public void AnswerQuest(AnswerQuizQuest answer)
    {
        answerPrefab.RecycleAll();
        quizQuestParent.Hide();
        if (Pool.Get<QuestManager>().currentQuest is SideQuizQuestResource quizQuestResource)
        {
            if (answer.currentIndexAnswer == currentQuiz.rightAnswerIndex)
            {
                DoneQuest();



            }
            else
            {
                SayDialogManager.Instance.ShowDialogWithSkin(nameChar, gender, skins, color, quizQuestResource.wrongDialog);

            }
        }

       


    }

    public override ActionMapResourceSO GetActionMapResourceSO()
    {
        // 1. Tạo bản sao danh sách để lọc mà không ảnh hưởng data gốc
        List<ActionMapResourceSO> resourceSideQuest = new List<ActionMapResourceSO>(actionMapResourceSOs);

        // 2. Vòng lặp chạy cho đến khi cạn kiệt danh sách tạm
        while (resourceSideQuest.Count > 0)
        {
            // Bốc ngẫu nhiên 1 phần tử theo số lượng còn lại trong list
            int randomIndex = UnityEngine.Random.Range(0, resourceSideQuest.Count);
            var resourceRandom = resourceSideQuest[randomIndex];

            // Kiểm tra xem có đúng loại SideQuestResource không
            if (resourceRandom is SideQuestResource sideQuest)
            {
                // Điều kiện ĐẠT: Trả về kết quả ngay lập tức
                if (sideQuest.CanActiveEventMap())
                {
                    return resourceRandom;
                }
            }

            // Điều kiện XẤU (Không phải SideQuest hoặc không CanActive): 
            // Xóa thẳng tay phần tử tại vị trí index này để vòng sau không bốc trùng lại nữa
            resourceSideQuest.RemoveAt(randomIndex);
        }

        // Nếu đã duyệt hết sạch sành sanh danh sách mà không ai thỏa mãn
        return null;
    }
}
