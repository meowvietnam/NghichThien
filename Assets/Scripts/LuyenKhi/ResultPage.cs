using System.Linq;
using DG.Tweening;
using Network;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Network.Extensions;

namespace LuyenKhi
{
    public class ResultPage : BaseBehaviour
    {
        [SerializeField] private GameObject selfObject;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Image icon;
        [SerializeField] private InputField nameField;
        [SerializeField] private RectTransform myTransform;
        [SerializeField] private EffectDetailManager effectDetailManager;
        private CreateCharacter.Equipment equipment;

        private void Start()
        {
            SetEventButton(confirmButton, OnConfirm);
        }

        private void OnConfirm()
        {
            var name = nameField.text;
            var setName = new SetEquipmentName()
            {
                Name = name,
                Id = equipment.Id,
                SelfId = equipment.SelfId
            };
            Client.Instance.Send(MessageTag.SetEquipmentName, setName);
        }

        public void Show(CreateCharacter.Equipment equipment)
        {
            this.equipment = equipment;
            nameField.text = equipment.Name;
            var itemData = equipment.Item;
            icon.sprite = itemData.Icon;
            icon.SetNativeSize();
            selfObject.SetActive(true);
            myTransform.DOKill();
            myTransform.localScale = Vector3.zero;
            myTransform.DOScale(Vector3.one, 0.25f);
            var res = LuyenKhiPanel.Instance.Resources;
            var details = equipment.Effects.Select(x =>
            {
                var name = res.EffectTypeConfigs.First(y => y.Type == x.Type).Name;
                return $"{name} ({x.Value})";
            }).ToArray();
            effectDetailManager.SetDetails(details);
        }

        public void Hide()
        {
            myTransform.DOKill();
            myTransform.DOScale(Vector3.zero, 0.25f).OnComplete(() => { selfObject.SetActive(false); });
        }
    }
}