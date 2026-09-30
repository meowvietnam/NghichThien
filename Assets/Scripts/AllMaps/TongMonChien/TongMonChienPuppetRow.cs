using System;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps.TongMonChien
{
    /// <summary>Một hàng trong bảng Rối (prefab <c>TongMonChienPuppetRow.prefab</c>).</summary>
    public class TongMonChienPuppetRow : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text infoText;
        [Tooltip("Tuỳ chọn: thanh HP của Rối.")]
        [SerializeField] private Slider hpBar;
        [Tooltip("Nhóm nút chỉ Tộc Chủ thấy.")]
        [SerializeField] private GameObject leaderActions;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button deployButton;
        [SerializeField] private Button pillButton;

        private int puppetId;
        private TongMonChienController controller;
        private bool wired;

        public void Bind(WarUnitView puppet, DateTime now, bool isLeader, TongMonChienController owner)
        {
            puppetId = puppet.Id;
            controller = owner;
            WireOnce();
            var hp = puppet.GetHp(now);
            if (nameText != null) nameText.text = $"{puppet.Name}  {TongMonChienHud.LevelText(puppet.Level)}";
            if (infoText != null)
                infoText.text = $"HP {hp}/{puppet.HpMax}  |  Node {puppet.NodeId}  |  {TongMonChienHud.StateText(puppet.State)}";
            if (hpBar != null) hpBar.value = puppet.HpMax > 0 ? (float)hp / puppet.HpMax : 0;
            if (leaderActions != null) leaderActions.SetActive(isLeader);
            if (deployButton != null) deployButton.interactable = hp >= puppet.HpMax && puppet.State == WarUnitState.Idle;
            if (pillButton != null) pillButton.interactable = hp < puppet.HpMax && puppet.State != WarUnitState.Fighting;
            if (upgradeButton != null) upgradeButton.interactable = puppet.State != WarUnitState.Fighting;
        }

        /// <summary>Hàng được tái sử dụng cho Rối khác nên listener đọc puppetId hiện tại, chỉ gắn một lần.</summary>
        private void WireOnce()
        {
            if (wired) return;
            wired = true;
            if (upgradeButton != null) upgradeButton.onClick.AddListener(() => controller.UpgradePuppet(puppetId));
            if (deployButton != null) deployButton.onClick.AddListener(() => controller.BeginDeployPuppet(puppetId));
            if (pillButton != null) pillButton.onClick.AddListener(() => controller.OpenInventoryForPuppet(puppetId));
        }
    }
}
