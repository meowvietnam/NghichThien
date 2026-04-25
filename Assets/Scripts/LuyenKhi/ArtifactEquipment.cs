using System;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class ArtifactEquipment : BaseBehaviour
    {
        [field: SerializeField] public ArtifactType Type { get; private set; }
        [SerializeField] private SelectTypePage selectTypePage;
        [SerializeField] private Button button;
        private void Start()
        {
            SetEventButton(button,OnClick);
        }
        private void OnClick()
        {
            selectTypePage.SelectArtifactType(this);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}