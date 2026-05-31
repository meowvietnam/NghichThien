using LongDH.LongDHModules;
using LongDHModules;
using UnityEngine;
using UnityEngine.UI;

public class AnswerQuizQuest : BaseBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] public int currentIndexAnswer;
    [SerializeField] public Text textQuiz;

    public void Init(int indexAnswer,string textQuiz)
    {
        currentIndexAnswer = indexAnswer;
        this.textQuiz.text = textQuiz;
        SetEventButton(btn, () =>
        {
            Pool.Get<SideQuestManager>().AnswerQuest(this);
        });
    }
}
