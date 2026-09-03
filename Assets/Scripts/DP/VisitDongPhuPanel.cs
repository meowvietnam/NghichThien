using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace DP
{
    public class VisitDongPhuPanel : BaseBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private InputField nameField;
        [SerializeField] private Button visitButton;
        [SerializeField] private VisitPlayer visitPlayerPrefab;
        [SerializeField] private RectTransform content;
        [SerializeField] private RectTransform panel;
        private Tween tween;
        private string key = "visit_history";
        private bool isInit;
        private void Start()
        {
            SetEventButton(visitButton, Visit);
            SetEventButton(closeButton, Hide);
        }

        private void Hide()
        {
            tween?.Kill();
            tween = panel.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }

        private void Visit()
        {
            var name = nameField.text;
            if(string.IsNullOrWhiteSpace(name))
            {
                return;
            }
            DongPhuPanel.Instance.Visit(name);
        }
        public void Show()
        {
            tween?.Kill();
            if(!isInit)
            {
                var histories = GetHistories();
                foreach(var history in histories)
                {
                    var player = Instantiate(visitPlayerPrefab, content);
                    player.Init(history);
                }

                isInit = true;
            }
            gameObject.SetActive(true);
            panel.localScale = Vector3.zero;
            tween = panel.DOScale(Vector3.one, 0.25f);
        }
        private HashSet<string> GetHistories()
        {
            if(PlayerPrefs.HasKey(key))
            {
                var json = PlayerPrefs.GetString(key);
                try
                {
                    return JsonConvert.DeserializeObject<HashSet<string>>(json);
                }
                catch
                {
                    return new HashSet<string>();
                }
            }
            return new HashSet<string>();
        }
        public void AddHistory(string name)
        {
            var histories = GetHistories();
            histories.Add(name);
            if(histories.Count > 15)
            {
                var first = histories.First();
                histories.Remove(first);
            }
            var json = JsonConvert.SerializeObject(histories);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }
    }
}
