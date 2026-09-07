using LongDH.LongDHModules;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    public class DongPhuButton : BaseBehaviour
    {
        [SerializeField] private Button button;

        private void Start()
        {
            SetEventButton(button, OnClick);
        }

        private void OnClick()
        {
            Observer.Notify(EventList.GamePlay.LOAD_SCENE, "DongPhu");
        }
    }
}