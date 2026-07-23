using UnityEngine;
using UnityEngine.UI;

namespace ConvertElementalSkill
{
    public class ConvertElementalSkillPage : BaseBehaviour
    {
        [SerializeField] private GameObject selfObject;
        [SerializeField] private Image icon;
        [SerializeField] private Text nameText;
        [field: SerializeField] public PutItems PutItems { get; private set; }
        [field: SerializeField] public ConvertRateManager RateManager { get; private set; }
        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
        }
        public void SetIcon(Sprite spriteIcon)
        {
            icon.sprite = spriteIcon;
        }
        public void SetName(string name)
        {
            nameText.text = name;
        }
        public void Init()
        {
            PutItems.Init();
            RateManager.Init();
        }
    }
}
