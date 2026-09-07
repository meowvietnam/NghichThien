using System;
using Base;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class TierSelection : MonoBehaviour
    {
        [SerializeField] private Button btn;
        [SerializeField] private Tier tier;

        private void Start()
        {
            btn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            var recipeListUI = LuyenDanPanel.Instance.RecipeListUI;
            recipeListUI.Filter(tier);
            recipeListUI.CloseTierSelection();
        }

        private void OnDestroy()
        {
            btn.onClick.RemoveAllListeners();
        }
    }
}