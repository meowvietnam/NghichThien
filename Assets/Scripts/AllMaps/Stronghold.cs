using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace AllMaps
{
    public class Stronghold : Map
    {
        [SerializeField] private GameObject enter;
        [SerializeField] private Button btnEnter;

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
