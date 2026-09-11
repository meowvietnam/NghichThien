using UnityEngine;

namespace ConvertDanLu
{
    /// <summary>
    /// Page containing the 4 input slots and conversion info display.
    /// Mirrors ConvertElementalSkill.ConvertElementalSkillPage.
    /// </summary>
    public class ConvertDanLuPage : BaseBehaviour
    {
        [SerializeField] private GameObject selfObject;
        [field: SerializeField] public PutItems PutItems { get; private set; }
        [field: SerializeField] public ConvertInfoManager InfoManager { get; private set; }

        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
        }

        public void Init()
        {
            PutItems.Init();
            InfoManager.Init();
        }
    }
}
