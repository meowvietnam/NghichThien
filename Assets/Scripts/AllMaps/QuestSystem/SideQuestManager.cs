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
using UnityEngine;
using UnityEngine.UI;

public class SideQuestManager : MonoBehaviour , INetworkEvent.ICompleteSideQuest
{
   
    public int timeMonthRandomSideQuest => 5;
    public List<SideQuestResourceSO> actionsSideQuestMap;
    public static string SaveOldTimeRandomSideQuest => "OldTimeRandomSideQuest";
    [SerializeField] public List<Map> currentMapSideQuest;
    [SerializeField] public Dictionary<string, string> skins;
    [SerializeField] public Dictionary<string, Color> color;
    [SerializeField] public Gender gender;

    public int countSideQuest => 3 - Client.Instance.CharacterData.QuestManager.CurrentCountSideQuest;
    [Header("Quiz Quest Visual")]
    [SerializeField] private AnswerQuizQuest answerPrefab;
    [SerializeField] private SideQuizQuestResource.Quiz currentQuiz;
    // Thay vì khai báo biến List, hãy viết thành hàm trả về List
    public List<string> GetQuizDialogs(string quizQuestion)
    {
        return new List<string>
    {
        "Đạo hữu dừng bước! Ta thấy ngươi căn cốt kinh kỳ, nhưng đường tu tiên cốt ở ngộ tính, chứ không chỉ có man lực.",
        "Ta có vài câu hỏi về thiên địa quy luật. Trả lời đúng, ta sẽ tặng ngươi tiên ngọc.",
        "Ngươi đã sẵn sàng chưa? Hãy nghe kỹ câu hỏi đầu tiên...",
        $"{quizQuestion}" // Truyền thẳng câu hỏi vào đây, không gọi qua Pool nữa
    };
    }
    List<string> correctDialog = new List<string>
    {
            "Ha ha ha! Khá khen cho câu trả lời của ngươi! Không hổ danh là đứa con của trời.",
            "Đây là phần thưởng xứng đáng dành cho ngộ tính đỉnh cao của ngươi. Cầm lấy đi!"
    };
    List<string> wrongDialog = new List<string>
    {
            "Sai rồi! Tiếc thay cho một tâm trí còn quá nhiều tạp niệm, chưa thấu hiểu được chân lý.",
    };
    [SerializeField] private Popup quizQuestParent;
  

    private void Awake()
    {
        Pool.Set(this);
        Observer.AddObserver(EventList.GamePlay.TIME_ADJUST, OnTimeAdjust);

    }
    private void Start()
    {
        LoadSideQuestMap();
        OnTimeAdjust();
    }
    public void OnTimeAdjust()
    {
        // 1. Lấy tổng số tháng hiện tại của thế giới
        int timeWordCurrent = GameManager.GetTimeWorldInt();

        // 2. Lấy thời điểm thực hiện random lần cuối từ PlayerPrefs
        // Nếu là lần đầu tiên (giá trị -1), ta coi như cần random ngay lập tức
        int oldTimeRandomSideQuest = PlayerPrefs.GetInt(SaveOldTimeRandomSideQuest, -1);

        // --- XỬ LÝ RANDOM NPC (Mỗi 5 tháng) ---
    
        // --- XỬ LÝ RANDOM ENEMY (Mỗi 3 tháng) ---
        if (oldTimeRandomSideQuest == -1 || (timeWordCurrent - oldTimeRandomSideQuest) >= timeMonthRandomSideQuest)
        {
            SideQuestRandomOnMap(); // Truyền maxCount ví dụ là 5
            Debug.Log($"<color=red>Enemy Spawned</color> at month: {timeWordCurrent}");
        }


    }
    private void LoadSideQuestMap()
    {

        string path = Application.persistentDataPath + "/SaveSideQuestMap.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            // Tiếp tục xử lý dữ liệu (JsonUtility.FromJson...)
            var data = JsonConvert.DeserializeObject<List<int>>(json);
            List<int> sideQuestExcept = new List<int>();
            foreach (var item in sideQuestExcept)
            {
                SetSideQuestOnMap(item);

            }
            Debug.Log("Đã load dữ liệu thành công!");
        }
        else
        {
            SideQuestRandomOnMap();
            Debug.LogWarning("Không tìm thấy file save. Khởi tạo dữ liệu mới.");
            return;
        }
    }
    private void SetSideQuestOnMap(int id)
    {
        var mapManager = Pool.Get<MapManager>();
        var map = mapManager.FindMap(id);
        mapManager.SetDataOnMap(actionsSideQuestMap[UnityEngine.Random.Range(0, actionsSideQuestMap.Count)], id);
        currentMapSideQuest.Add(map);

    }
    private void SaveSideQuestMap()
    {
        // Lọc trùng và chỉ lấy ra danh sách các Id (int)
        List<int> dataSaveNode = currentMapSideQuest
            .GroupBy(x => x.Id)
            .Select(group => group.Key) // Lấy ra cái Id đại diện của nhóm
            .ToList();

        string json = JsonConvert.SerializeObject(dataSaveNode);
        File.WriteAllText(Application.persistentDataPath + "/SaveSideQuestMap.json", json);
    }
    private void SideQuestRandomOnMap()
    {
        var mapManager = Pool.Get<MapManager>();
        var mapExcept = mapManager.maps.Where(x => x.IsEmptyAction && !(x is Stronghold)).ToList();
        for (int i = 0; i < countSideQuest; i++)
        {
            int randomMapId = mapExcept[UnityEngine.Random.Range(0, mapExcept.Count)].Id;
            var map = mapManager.FindMap(randomMapId);
            mapExcept.Remove(map);
            SetSideQuestOnMap(randomMapId);
        }
        
        SaveSideQuestMap();

        // Lưu thời gian random
        PlayerPrefs.SetInt(SaveOldTimeRandomSideQuest, GameManager.GetTimeWorldInt());
        PlayerPrefs.Save();

    }
    public void SetSkinSideQuestCurrent(Gender gender, Dictionary<string, string> skin, Dictionary<string,Color>color)
    {
        this.gender = gender;
        this.skins = skin;
        this.color = color;
    }
    public void QuizQuestInitShow(SideQuizQuestResource.Quiz quiz)
    {

        quizQuestParent.Show();
        currentQuiz = quiz;
        List<AnswerQuizQuest> listBtnAnswer = new List<AnswerQuizQuest>();
        var skinRandom = SayDialogManager.Instance.ShowDialogRandomSkin("Người thần bí", GetQuizDialogs(currentQuiz.quiz), null, null, () =>
        {
            for (int i = 0; i < listBtnAnswer.Count; i++)
            {
                var answerBtn = listBtnAnswer[i];
                answerBtn.transform.DOScale(Vector3.one, 0.5f);

            }
        },false);
        SetSkinSideQuestCurrent(skinRandom.gender,skinRandom.skin,skinRandom.color);
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
    public void AnswerQuest(AnswerQuizQuest answer)
    {
        answerPrefab.RecycleAll();
        quizQuestParent.Hide();

        if (answer.currentIndexAnswer == currentQuiz.rightAnswerIndex)
        {
            Observer.Notify(EventList.Network.COMPLETE_SIDEQUEST,this);
            // Fake Reward
            SayDialogManager.Instance.ShowDialog("Người thần bí", gender, skins, color, correctDialog,null,null,() =>{

                Pool.Get<EventRewardPanel>().InitShowEventReward(new RewardSnapshot { MoneyVip = 50 }, "Đây là phần thưởng xứng đáng dành cho ngộ tính đỉnh cao của ngươi!");


            });

        }
        else
        {
            SayDialogManager.Instance.ShowDialog("Người thần bí",gender,skins,color, wrongDialog);

        }

        
    }

    public void SuccessCompleteSideQuest(int moneyVip)
    {
        SayDialogManager.Instance.ShowDialog("Người thần bí", gender, skins, color, correctDialog, null, null, () => {

            Pool.Get<EventRewardPanel>().InitShowEventReward(new RewardSnapshot { MoneyVip = moneyVip }, "Đây là phần thưởng xứng đáng dành cho ngộ tính đỉnh cao của ngươi!");


        });
    }

    public void FailCompleteSideQuest(string message)
    {
    }
}
