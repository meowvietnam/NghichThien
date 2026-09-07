using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class AlchermyResult : MonoBehaviour
    {
        [SerializeField] private GameObject selfObject;
        [SerializeField] private ItemResult itemPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Button okBtn;
        [SerializeField] private RectTransform result;
        private List<ItemResult> items = new();

        private void Start()
        {
            okBtn.onClick.AddListener(OnOk);
        }

        private void OnOk()
        {
            Hide(0.15f);
        }

        private void OnDestroy()
        {
            okBtn.onClick.RemoveAllListeners();
        }

        public void SetDescription(string description)
        {
            descriptionText.text = description;
        }

        public void SetData(List<ItemRef> itemRefs)
        {
            foreach (var item in items)
            {
                item.DestroySelf();
            }

            items = new List<ItemResult>();
            foreach (var itemRef in itemRefs)
            {
                var item = Instantiate(itemPrefab, content);
                item.Init(itemRef);
                items.Add(item);
            }
        }

        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
        }

        public void Show(float time)
        {
            transform.DOKill();
            SetActive(true);
            result.localScale = Vector3.zero;
            result.DOScale(Vector3.one, time);
        }

        public void Hide(float time)
        {
            transform.DOKill();
            result.DOScale(Vector3.zero, time).OnComplete(() => { SetActive(false); });
        }
    }
}