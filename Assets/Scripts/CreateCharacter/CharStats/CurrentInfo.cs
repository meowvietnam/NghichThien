using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CreateCharacter.CharStats
{
    public class CurrentInfo : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private StatType statType;
        [SerializeField] private Text valueText;
        [SerializeField] private string infoName;
        [SerializeField,TextArea] private string info;
        protected SelectStats selectStats;
        public void Init(SelectStats selectStats)
        {
            this.selectStats = selectStats;
            var stats = selectStats.CharacterStats;
            stats.RegisterEvent(statType,OnValueChanged);
            var value = stats.GetValue(statType);
            OnValueChanged(value);
        }
        public virtual string GetValue(object value)
        {
            return value.ToString();
        }

        protected virtual Color GetColor(object value, object originalValue)
        {
            int t = 0;
            if (value is int v)
            {
                t = v.CompareTo(originalValue);
            }
            else if (value is float f)
            {
                t = f.CompareTo(originalValue);
            }
            if (t == 0)
            {
                return Color.white;
            }
            if (t > 0)
            {
                return Color.chartreuse;
            }
            return Color.crimson;
        }
        private void OnValueChanged(object value)
        {
            valueText.text = GetValue(value);
            var originalValue = selectStats.GetOrginalValue(statType);
            var color = GetColor(value, originalValue);
            valueText.color = color;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            var tooltip = Tooltip.Instance;
            tooltip.SetActive(true);
            tooltip.SetTextInfo(infoName,info);
            tooltip.SetScale(Vector2.one);
            tooltip.SetPosition(eventData.position - new Vector2(100,-40));
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            var tooltip = Tooltip.Instance;
            tooltip.SetActive(false);
        }
    }
}