using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class TierSelectionButton : MonoBehaviour
    {
        [SerializeField] private Button btn;

        private void Start()
        {
            btn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            LuyenDanPanel.Instance.RecipeListUI.OpenTierSelection();
        }

        private void OnDestroy()
        {
            btn.onClick.RemoveAllListeners();
        }
    }
}