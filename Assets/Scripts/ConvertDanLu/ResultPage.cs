using DG.Tweening;
using LuyenDan;
using UnityEngine;
using UnityEngine.UI;

namespace ConvertDanLu
{
    /// <summary>
    /// Shows the resulting đan lô (furnace) after a successful conversion.
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
        /// Displays the received đan lô (furnace) in the result slot.
        /// The ItemRef uses SelfId as Id and furnace type Id as TempId.
        /// </summary>
        public void SetItem(FurnaceData furnace)
        {
            var itemRef  = furnace.CreateRefSelfID(); // Id=SelfId, TempId=Id, Type=Furnace
            IDanItemData itemData = new FurnaceItemData(furnace);
            var danData  = new DanItemData(itemRef, itemData);
            itemUI.SetData(danData);
        }

        private void OnDestroy()
        {
            tween?.Kill();
            confirmButton.onClick.RemoveAllListeners();
        }
    }
}
