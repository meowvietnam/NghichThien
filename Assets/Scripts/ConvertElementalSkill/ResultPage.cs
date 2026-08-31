using DG.Tweening;
using LongDHModules;
using LuyenDan;
using UnityEngine;
using UnityEngine.UI;

namespace ConvertElementalSkill
{
    public class ResultPage : BaseBehaviour
    {
        [SerializeField] private GameObject selfObject;
        [SerializeField] private RectTransform selfTransform;
        [SerializeField] private BaseItemUI itemUI;
        [SerializeField] private Button confirmButton;
        private Tween tween;

        private void Start()
        {
            SetEventButton(confirmButton, OnConfirm);
        }

        private void OnConfirm()
        {
            Hide();
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
            tween.OnComplete(() => { selfObject.SetActive(false); });
        }

        public void SetItem(ItemRef item)
        {
            var ui = Pool.Get<UIManager>();
            var gameManager = Pool.Get<GameManager>();
            ISkillItemData itemData = item.Type == ItemType.ThanThong
                ? new ThanThongItemData(item, gameManager, ui)
                : new CongPhapItemData(item, gameManager, ui);
            var skillData = new SkillData(item, itemData);
            itemUI.SetData(skillData);
        }
        private void OnDestroy()
        {
            tween?.Kill();
            confirmButton.onClick.RemoveAllListeners();
        }
    }
}