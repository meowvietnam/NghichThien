using Base;
using UnityEngine;
using UnityEngine.UI;

namespace Events
{
    public class ItemEventMilestoneUI : BaseBehaviour
    {
        [SerializeField] private ItemEventUI itemUI;
        [SerializeField] private Text progressText;
        [SerializeField] private Color completed;
        [SerializeField] private Color incompleted;

        public void Init(ItemEvent target, int progress)
        {
            itemUI.Init(target);
            itemUI.SetCount(1);
            progressText.text = $"{Utils.FormatCount(progress)}/{Utils.FormatCount(target.Count)}";
            var completed = progress / target.Count >= 1;
            var color = completed ? this.completed : incompleted;
            progressText.color = color;
        }
    }
}