using LongDHModules;
using LuyenDan;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private Button btnShowFullSkill;
        [SerializeField] private Bag bagShowFullSkill;

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
            SetEventButton(btnShowFullSkill, () =>
            {
                var itemSet = ConvertElementalSkillPanel.Instance.CurrentSelectedSkill.SkillType == SkillType.ThanThong
                    ? Pool.Get<GameManager>().thanThongs
                    : Pool.Get<GameManager>().congPhaps;
                var skills = new List<ItemRef>();
                foreach (var item in itemSet)
                {
                    skills.Add(new ItemRef
                    {
                        Id = (ushort)item,
                        Type = ConvertElementalSkillPanel.Instance.CurrentSelectedSkill.SkillType == SkillType.ThanThong
                            ? ItemType.ThanThong
                            : ItemType.CongPhap,
                        Count = 1,
                    });
                }

                bagShowFullSkill.Init(skills);
                bagShowFullSkill.SetActive(true);
            });
        }
    }
}