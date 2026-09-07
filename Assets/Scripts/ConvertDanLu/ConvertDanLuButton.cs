using System;
using LongDH.LongDHModules;
using UnityEngine;
using UnityEngine.UI;

namespace ConvertDanLu
{
    public class ConvertDanLuButton : BaseBehaviour
    {
        [SerializeField] private Button button;
        private void Start()
        {
            SetEventButton(button, OnClick);
        }

        private void OnClick()
        {
            Observer.Notify(EventList.GamePlay.LOAD_SCENE,"ConvertDanLu");
        }
    }
}
