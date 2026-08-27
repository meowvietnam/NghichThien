using ConvertElementalSkill;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Network.Models;
using Network;
using Network.Extensions.MemoryPack;
using System;

namespace DP
{
    public class ButtonGroup : BaseBehaviour
    {
        [field: SerializeField] public Button HarvestButton { get; private set; }
        [field: SerializeField] public Button WaterButton { get; private set; }
        [SerializeField] private float startPosY;
        [SerializeField] private float targetPosY;
        private Tween tween;
        private void Start()
        {
            SetEventButton(HarvestButton, HarvestClick);
            SetEventButton(WaterButton, WaterClick);
        }
        private void HarvestClick()
        {
            var slot = DongPhuPanel.Instance.CurrentSelectedSlot;
            if (slot == null)
            {
                Debug.Log("Slot null!");
                return;
            }
            if (!slot.HasSeed())
            {
                Debug.Log("Not has seed");
                return;
            }
            var slotIndex = slot.Slot.SlotIndex;
            var request = new HarvestRequest()
            {
                SlotIndex = slotIndex
            };
            Client.Instance.MemSend(MessageTag.HarvestSlot, request);
        }
        private void WaterClick()
        {
            var slot = DongPhuPanel.Instance.CurrentSelectedSlot;
            if (slot == null)
            {
                Debug.Log("Slot null!");
                return;
            }
            if (!slot.HasSeed())
            {
                Debug.Log("Not has seed");
                return;
            }
            var slotIndex = slot.Slot.SlotIndex;
            var request = new WateringHelpRequest()
            {
                SlotIndex = slotIndex
            };
            Client.Instance.MemSend(MessageTag.WateringHelp, request);
        }
        public void Init(bool isMe)
        {
            HarvestButton.gameObject.SetActive(isMe);
            WaterButton.gameObject.SetActive(!isMe);
            Hide();
        }
        [Button]
        public void Show()
        {
            UpdateUI();
            tween?.Kill();
            tween = transform.DOLocalMoveY(targetPosY, 0.3f);
        }
        [Button]
        public void Hide()
        {
            tween?.Kill();
            tween = transform.DOLocalMoveY(startPosY, 0.3f);
        }
        public void UpdateUI()
        {
            var dongPhu = DongPhuPanel.Instance;
            var slot = dongPhu.CurrentSelectedSlot;
            if (slot == null)
            {
                WaterButton.interactable = false;
                HarvestButton.interactable = false;
                return;
            }
            if (!slot.HasSeed())
            {
                WaterButton.interactable = false;
                HarvestButton.interactable = false;
                return;
            }
            var canWatering = dongPhu.CanWateringHelp();
            var canHarvest = slot.Seed.CanHarvest();
            WaterButton.interactable = canWatering;
            HarvestButton.interactable = canHarvest;
        }
    }
}
