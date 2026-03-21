using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class RefineButton : MonoBehaviour
    {
        [SerializeField] public Button refine;

        private void Start()
        {
            refine.onClick.AddListener(OnClick);
        }
        public void UpdateCanRefine()
        {
            var canRefine = LuyenDanPanel.Instance.Alchermy.CanRefine();
            refine.interactable = canRefine;
        }
        private void OnClick()
        {
            LuyenDanPanel.Instance.Alchermy.Refine();
        }

        private void OnDestroy()
        {
            refine.onClick.RemoveAllListeners();
        }
    }
}