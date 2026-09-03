using ConvertElementalSkill;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps.UI_Gacha__NTK_OS
{
    public class GachaSkillType : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private GachaPanel gachaPanel;
        [SerializeField] private GameObject active;
        [SerializeField] private GameObject inactive;
        [field: SerializeField] public BaseSkill.ETypeSkill Type { get; private set; }
        private void OnEnable()
        {
            OnToggle(toggle.isOn);
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(OnToggle);
        }
        private void OnToggle(bool isOn)
        {
            active.SetActive(isOn);
            inactive.SetActive(!isOn);
            if (isOn)
            {
                gachaPanel.SetTypeSkill(Type);
            }
        }
    }
}
