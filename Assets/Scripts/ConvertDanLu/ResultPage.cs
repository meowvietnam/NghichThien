using DG.Tweening;
using LongDHModules;
using LuyenDan;
using UnityEngine;
using UnityEngine.UI;

namespace ConvertDanLu
{
    /// <summary>
    /// Shows the resulting đan phương (recipe) after a successful conversion.
    /// Mirrors ConvertElementalSkill.ResultPage adapted for RecipeData.
    /// </summary>
    public class ResultPage : BaseBehaviour
    {
        [SerializeField] private GameObject selfObject;
        [SerializeField] private RectTransform selfTransform;
        [SerializeField] private ResultItemUI itemUI;
        [SerializeField] private Button confirmButton;
        private Tween tween;

        private void Start()
        {
            SetEventButton(confirmButton, Hide);
        }

        public void Show()
        {
            selfObject.SetActive(true);
            tween?.Kill();
            selfTransform.localScale = Vector3.zero;
            tween = selfTransform.DOScale(Vector3.one, 0.25f);
        }

        public void Hide()
        {
            tween?.Kill();
            tween = selfTransform.DOScale(Vector3.zero, 0.25f);
            tween.OnComplete(() => selfObject.SetActive(false));
        }

        /// <summary>
        /// Displays the received đan phương (recipe) ItemRef in the result slot.
        /// </summary>
        public void SetItem(ItemRef item)
        {
            IDanItemData itemData = new RecipeItemData(item);
            var danData = new DanItemData(item, itemData);
            itemUI.SetData(danData);
        }

        private void OnDestroy()
        {
            tween?.Kill();
            confirmButton.onClick.RemoveAllListeners();
        }
    }
}
