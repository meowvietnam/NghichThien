using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class ToggleImageChanger : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image targetImage;

        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;

        private void Awake()
        {
            toggle.onValueChanged.AddListener(UpdateImage);
            UpdateImage(toggle.isOn);
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(UpdateImage);
        }

        private void UpdateImage(bool isOn)
        {
            targetImage.sprite = isOn ? onSprite : offSprite;
        }
    }
}