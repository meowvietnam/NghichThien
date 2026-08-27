using System;
using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    public class VisitDongPhuButton : BaseBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private VisitDongPhuPanel panel;
        private void Start() 
        {
            SetEventButton(button, OpenPanel);
        }

        private void OpenPanel()
        {
            panel.Show();
        }
    }
}