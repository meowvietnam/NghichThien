using System;
using Base;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class SetPlayerNamePanel : MonoBehaviour, IPanel
    {
        [SerializeField] private GameObject thisObject;
        [SerializeField] private InputField nameInput;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button backBtn;
        [SerializeField] private Button nextBtn;
        public CanvasGroup CanvasGroup => canvasGroup;
        public Tween Tween { get; set; }
        private void Start()
        {
            backBtn.onClick.AddListener(Back);
            nextBtn.onClick.AddListener(Next);
        }

        private void Next()
        {
            CreateCharacterSystem.Instance.NewCharacter.Name = nameInput.text;
            CreateCharacterSystem.Instance.NextToCreateCharacterPanel();
        }
        private void Back()
        {
            CreateCharacterSystem.Instance.BackToSelectCharacter();
        }
        public void SetActive(bool active)
        {
            thisObject.SetActive(active);
        }

        private void OnDestroy()
        {
            backBtn.onClick.RemoveAllListeners();
            nextBtn.onClick.RemoveAllListeners();
        }
    }
}
