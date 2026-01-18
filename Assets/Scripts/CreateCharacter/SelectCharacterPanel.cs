using System;
using System.Collections.Generic;
using Base;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class SelectCharacterPanel : MonoBehaviour, IPanel
    {
        [SerializeField] private GameObject thisObject;
        [SerializeField] private Transform content;
        [SerializeField] private CreateAvatar createAvatarPrefab;
        [SerializeField] private Button backBtn;
        [SerializeField] private CanvasGroup canvasGroup;
        public CanvasGroup CanvasGroup => canvasGroup;
        public Tween Tween { get; set; }
        public void Init(List<CharacterData> characterDatas, int totalSlots, SkinResources skinResources)
        {
            for (int i = 0; i < totalSlots; i++)
            {
                var charData = i < characterDatas.Count ? characterDatas[i] : null;
                var createAvatar = Instantiate(createAvatarPrefab, content);
                createAvatar.Init(charData, skinResources);
            }
            backBtn.onClick.AddListener(Back);
        }
        private void Back()
        {
            Debug.Log("Not implemented yet");
        }
        public void SetActive(bool active)
        {
            thisObject.SetActive(active);
        }

        private void OnDestroy()
        {
            backBtn.onClick.RemoveAllListeners();
        }
    }
}
