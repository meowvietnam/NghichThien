using LongDH.LongDHModules;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace AllMaps
{
    public class Stronghold : Map
    {
        [SerializeField] private GameObject enter;
        [SerializeField] private Button btnEnter;
        [SerializeField] private string nameSceneLoad;

        private void Awake()
        {
            SetEventButton(btnEnter, () =>
            {
                Observer.Notify(EventList.GamePlay.LOAD_SCENE, nameSceneLoad);
            });
        }
        public override void OnPlayerEnter()
        {
            enter.SetActive(true);
        }
        public override void OnPlayerExit()
        {
            enter.SetActive(false);
        }
    }
}
