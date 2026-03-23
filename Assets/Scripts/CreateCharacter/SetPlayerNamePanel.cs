using System;
using Base;
using DG.Tweening;
using Network;
using UnityEngine;
using UnityEngine.UI;
using Network.Extensions;
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
            var name = nameInput.text;
            Client.Instance.Send(MessageTag.SetCharName, name);
        }

        public void NextFailed(string message)
        {
            NotifyGroup.Instance.Show(message);
        }
        public void NextSuccess()
        {
            CreateCharacterSystem.Instance.NewCharacter.Name = nameInput.text;
            CreateCharacterSystem.Instance.NextToCreateCharacterPanel();
        }
        private void Back()
        {
            Client.Instance.Send(MessageTag.BackToSelectChar);
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
