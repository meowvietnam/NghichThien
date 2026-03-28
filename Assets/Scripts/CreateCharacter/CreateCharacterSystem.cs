using System;
using System.Collections.Generic;
using System.IO;
using Base;
using CreateCharacter.CharStats;
using CreateCharacter.States;
using DG.Tweening;
using LuyenDan;
using Network;
using Network.Callback;
using Network.ClientState;
using Network.Extensions;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CreateCharacter
{
    public class CreateCharacterSystem : MonoBehaviour, ISelectCharacterCallback, ICreateCharacterCallback
    {
        [SerializeField] private int totalSlots = 3;
        [SerializeField] private SkinResources skinResources;
        [SerializeField] private SelectCharacterPanel selectCharacterPanel;
        [SerializeField] private SetPlayerNamePanel setPlayerNamePanel;
        [SerializeField] private CreateCharacterPanel createCharacterPanel;
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private string createCharacterSuccess;
        [SerializeField] private string selecting;
        private UserData UserData => Client.Instance.UserData;
        public static CreateCharacterSystem Instance { get; private set; }
        [field: SerializeField] public CharacterData NewCharacter { get; private set; }
        public SkinResources SkinResources => skinResources;

        private void Start()
        {
            Instance = this;
            var state = new SelectCharacter(this);
            Client.Instance.ChangeState(state);
            state.GetCharacters();
        }

        public void AddCharacterData(CharacterData characterData)
        {
            if (UserData.CharacterDatas.Count >= totalSlots) return;
            UserData.CharacterDatas.Add(characterData);
        }

        public void BeginCreateCharacter()
        {
            NewCharacter = new CharacterData();
            ToSetPlayerNamePanel(selectCharacterPanel);
        }

        public void ToSetPlayerNamePanel(IPanel panel)
        {
            panel.DoFade(1, 0, 0.5f, false, () => { panel.SetActive(false); });
            setPlayerNamePanel.SetActive(true);
            setPlayerNamePanel.DoFade(0, 1, 0.5f, true);
        }

        public void BackToSelectCharacter()
        {
            selectCharacterPanel.SetActive(true);
            selectCharacterPanel.DoFade(0, 1, 0.5f, true);
            setPlayerNamePanel.DoFade(1, 0, 0.5f, false, () => { setPlayerNamePanel.SetActive(false); });
        }

        public void NextToCreateCharacterPanel()
        {
            setPlayerNamePanel.DoFade(1, 0, 0.5f, false, () => { setPlayerNamePanel.SetActive(false); });
            createCharacterPanel.SetActive(true);
            createCharacterPanel.Init(skinResources);
            createCharacterPanel.DoFade(0, 1, 0.5f, true);
        }

        public void OnGetCharactes(List<CharacterData> characterDatas)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                selectCharacterPanel.Init(characterDatas, totalSlots, skinResources);
                selectCharacterPanel.SetActive(true);
                selectCharacterPanel.DoFade(0, 1, 0.5f, true);
            });
        }

        public void OnCanCreateCharacter(CanCreateChar result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                if (!result.CanCreate)
                {
                    NotifyGroup.Instance.Show(result.Message);
                    return;
                }

                Client.Instance.ChangeState(new Network.ClientState.CreateCharacter(this));
                BeginCreateCharacter();
            });
        }

        public void SelectCharacter(int id)
        {
            progressBar.SetText(selecting);
            progressBar.DoShow(0.15f, 0.15f, null);
            Client.Instance.Send(MessageTag.SelectCharacter, id);
        }

        public void OnSelectCharacter(SelectCharacterResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    var characterData = result.CharacterData;
                    Client.Instance.SetChar(characterData);
                    progressBar.DoKill(() => {  });
                    SceneManager.LoadSceneAsync("AllMaps");
                }
                else
                {
                    progressBar.DoKill(() => { NotifyGroup.Instance.Show(result.Message); });
                }
            });
        }

        public void BackToSelectChar()
        {
            NetworkManager.Instance.Invoke(() =>
            {
                BackToSelectCharacter();
                Client.Instance.ChangeState(new SelectCharacter(this));
            });
        }

        public void OnSetNameResult(SetCharNameResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    setPlayerNamePanel.NextSuccess();
                }
                else
                {
                    setPlayerNamePanel.NextFailed(result.Message);
                }
            });
        }

        public void OnSetSkinsResult(SetSkinResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    createCharacterPanel.SetState(new SelectStatsState(createCharacterPanel));
                }
                else
                {
                    NotifyGroup.Instance.Show(result.Message);
                }
            });
        }

        public void OnApplyStatsResult(ApplyStatsResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    NotifyGroup.Instance.Show(createCharacterSuccess);
                    progressBar.DoKill(() => { SceneManager.LoadSceneAsync("CreateCharacter"); });
                }
                else
                {
                    NotifyGroup.Instance.Show(result.Message);
                    progressBar.DoKill(() => { });
                }
            });
        }

        public void Final(List<SelectionModel> selectionModels)
        {
            Client.Instance.Send(MessageTag.ApplyStats, selectionModels);
            progressBar.DoShow(0.5f, 0.5f, null);
        }
    }
}