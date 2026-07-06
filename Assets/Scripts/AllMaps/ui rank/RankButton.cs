using UnityEngine;
using UnityEngine.UI;

namespace AllMaps.ui_rank
{
    public class RankButton : BaseBehaviour
    {

        [SerializeField] private Button btn;
        [SerializeField] private RankPanel rankPanel;
        
        private void Start()
        {
            SetEventButton(btn,ShowRankPanel);
        }

        private void ShowRankPanel()
        {
            rankPanel.DoShow();
        }
    }
}
