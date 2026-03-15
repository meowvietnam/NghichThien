using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class AllTierSelection : MonoBehaviour
    {
        [SerializeField] private Button btn;
        private void Start()
        {
            btn.onClick.AddListener(OnClick);
        }
        private void OnClick()
        {
            var recipeListUI = LuyenDanPanel.Instance.RecipeListUI;
            recipeListUI.ShowAll();
            recipeListUI.CloseTierSelection();
        }
        private void OnDestroy()
        {
            btn.onClick.RemoveAllListeners();
        }
    }
}