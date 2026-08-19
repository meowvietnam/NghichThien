using System.Collections.Generic;
using Base;
using CreateCharacter.CharStats;
using CreateCharacter.States;
using Network;
using Network.Callback;
using Network.ClientState;
using Network.Extensions;
using Network.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CreateCharacter
{
    public class CreateCharacterSystem : AbstractCreateCharacterSystem, ISelectCharacterCallback
    {
        [SerializeField] private int totalSlots = 3;
        [SerializeField] private SkinResources skinResources;
        [SerializeField] private SelectCharacterPanel selectCharacterPanel;
        [SerializeField] private SetPlayerNamePanel setPlayerNamePanel;
        [SerializeField] private string createCharacterSuccess;
        [SerializeField] private string selecting;
        [SerializeField] private string confirmText;
        [SerializeField] private int originalMoney;
        private UserData UserData => Client.Instance.UserData;
        
        private void Start()
        {
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

        public override void ToSetPlayerNamePanel(IPanel panel)
        {
            panel.DoFade(1, 0, 0.5f, false, () =>
            {
                panel.SetActive(false);
            });
            setPlayerNamePanel.SetActive(true);
            setPlayerNamePanel.DoFade(0, 1, 0.5f, true);
        }

        public override void SelectStatsBack()
        {
            createCharacterPanel.SetState(new CreateSkin(createCharacterPanel, this));
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
            createCharacterPanel.Init(new CreateSkin(createCharacterPanel, this),skinResources, originalMoney);
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
        public override void Final(List<SelectionModel> selectionModels)
        {
            var money = createCharacterPanel.SelectStats.Money;
            if (money > 0)
            {
                var text = string.Format(confirmText, money);
                popupConfirmAction.ShowPopConfirmAction(text, () => SendFinal(selectionModels));
            }
            else
            {
                SendFinal(selectionModels);
            }
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
            progressBar.DoShow(0.5f, 0.15f, null);
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
                    Client.Instance.OnSelectedCharacter();
                    var op = SceneManager.LoadSceneAsync("AllMaps");
                    if (op != null)
                    {
                        op.completed += (o) => { progressBar.DoKill(() => {}); };
                    }
                }
                else
                {
                    progressBar.DoKill(() => { NotifyGroup.Instance.Show(result.Message); });
                }
            });
        }

        public override void BackToSelectChar()
        {
            NetworkManager.Instance.Invoke(() =>
            {
                BackToSelectCharacter();
                Client.Instance.ChangeState(new SelectCharacter(this));
            });
        }

        public override void OnSetNameResult(SetCharNameResult result)
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
        public override void OnApplyStatsResult(ApplyStatsResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    NotifyGroup.Instance.Show(createCharacterSuccess);
                    progressBar.DoKill(() =>
                    {
                        SceneManager.LoadSceneAsync("CreateCharacter");
                    });
                }
                else
                {
                    NotifyGroup.Instance.Show(result.Message);
                    progressBar.DoKill(() => { });
                }
            });
        }
    }
}