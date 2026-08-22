using System.Collections.Generic;
using CreateCharacter.CharStats;
using CreateCharacter.States;
using LuyenDan;
using Network;
using Network.Callback;
using Network.Models;
using UnityEngine;
using Network.Extensions;
using Network.Extensions.MemoryPack;

namespace CreateCharacter
{
    public abstract class AbstractCreateCharacterSystem : MonoBehaviour, ICreateCharacterCallback
    {
        [field: SerializeField] public CharacterData NewCharacter { get; protected set; }
        [SerializeField] protected CreateCharacterPanel createCharacterPanel;
        [SerializeField] protected ProgressBar progressBar;
        [SerializeField] protected PopupConfirmAction popupConfirmAction;
        public abstract void BackToSelectChar();
        public abstract void OnSetNameResult(SetCharNameResult result);

        public virtual void OnSetSkinsResult(SetSkinResult result)
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
        protected void SendFinal(List<SelectionModel> selectionModels)
        {
            Client.Instance.MemSend(MessageTag.ApplyStats, selectionModels);
            progressBar.DoShow(0.5f, 0.5f, null);
        }

        public abstract void Final(List<SelectionModel> selectionModels);
        public abstract void OnApplyStatsResult(ApplyStatsResult result);
        public abstract void ToSetPlayerNamePanel(IPanel panel);

        public abstract void SelectStatsBack();
    }
}