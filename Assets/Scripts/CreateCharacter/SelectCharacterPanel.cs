using System;
using System.Collections.Generic;
using Base;
using DG.Tweening;
using Network;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class SelectCharacterPanel : BaseBehaviour, IPanel
    {
        [SerializeField] private GameObject thisObject;
        [SerializeField] private Transform content;
        [SerializeField] private CreateAvatar createAvatarPrefab;
        [SerializeField] private Button backBtn;
        [SerializeField] private CanvasGroup canvasGroup;
        public CanvasGroup CanvasGroup => canvasGroup;
        public Tween Tween { get; set; }
        [SerializeField] private CreateCharacterSystem system;

        public void Init(List<CharacterData> characterDatas, int totalSlots, SkinResources skinResources)
        {
            for (int i = 0; i < totalSlots; i++)
            {
                var charData = i < characterDatas.Count ? characterDatas[i] : null;
                var createAvatar = Instantiate(createAvatarPrefab, content);
                createAvatar.Init(charData, skinResources, system);
            }

            SetEventButton(backBtn, Back);
        }

        private async void Back()
        {
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.BackToMainMenuRequest);
            var result = reader.MemRead<Result>();
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    SceneManager.LoadScene("MainMenu");
                }
                else
                {
                    NotifyGroup.Instance.Show(result.Message);
                }
            });
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