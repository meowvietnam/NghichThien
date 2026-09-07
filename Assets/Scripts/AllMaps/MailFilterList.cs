using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AllMaps
{
    public class MailFilterList : BaseBehaviour
    {
        [SerializeField] private List<MailFilter> mailFilters;
        [SerializeField] private MailPanel mailPanel;
        [SerializeField] private Button filterButton;
        [SerializeField] private Text text;
        [SerializeField] private GameObject selection;
        [SerializeField] private RectTransform direct;

        private void Start()
        {
            SetEventButton(filterButton, Open);
            foreach (var mailFilter in mailFilters)
            {
                mailFilter.OnFilter += OnFilter;
            }
        }

        private void Close()
        {
            selection.SetActive(false);
            direct.localEulerAngles = new Vector3(0, 0, 180);
        }

        private void Open()
        {
            selection.SetActive(true);
            direct.localEulerAngles = new Vector3(0, 0, 0);
        }

        private void OnFilter(MailFilter filter)
        {
            mailPanel.Filter(filter.Type);
            text.text = filter.Name;
            Close();
        }

        private void OnDestroy()
        {
            filterButton.onClick.RemoveAllListeners();
        }
    }
}