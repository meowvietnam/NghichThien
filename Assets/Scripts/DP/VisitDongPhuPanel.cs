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
        private const string LegacyKey = "visit_history";
        private const int MaxHistories = 15;
        private bool isInit;

        /// <summary>Lịch sử lưu riêng theo nhân vật (trước đây dùng chung một khoá cho mọi tài khoản trên máy).</summary>
        private static string Key => $"visit_history_{Network.Client.Instance?.CharacterData?.Id ?? 0}";

        private void Start()
        {
            SetEventButton(visitButton, Visit);
            SetEventButton(closeButton, Hide);
        }

        private void Hide()
        {
            tween?.Kill();
            tween = panel.DOScale(Vector3.zero, 0.25f).OnComplete(() => { gameObject.SetActive(false); });
        }

        private void Visit()
        {
            var name = nameField.text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            DongPhuPanel.Instance.Visit(name);
        }

        public void Show()
        {
            tween?.Kill();
            if (!isInit)
            {
                var histories = GetHistories();
                foreach (var history in histories)
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

        /// <summary>Danh sách tên đã thăm, mới nhất đứng đầu, không trùng (không phân biệt hoa thường).</summary>
        private List<string> GetHistories()
        {
            var key = Key;
            // Chuyển lịch sử cũ (khoá dùng chung) sang nhân vật đầu tiên mở bảng, rồi bỏ khoá cũ.
            if (!PlayerPrefs.HasKey(key) && PlayerPrefs.HasKey(LegacyKey))
            {
                var legacy = Parse(PlayerPrefs.GetString(LegacyKey));
                legacy.Reverse();
                PlayerPrefs.DeleteKey(LegacyKey);
                Save(legacy);
                return legacy;
            }

            return PlayerPrefs.HasKey(key) ? Parse(PlayerPrefs.GetString(key)) : new List<string>();
        }

        private static List<string> Parse(string json)
        {
            try
            {
                var list = JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
                var seen = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
                return list.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Where(seen.Add).ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        private static void Save(List<string> histories)
        {
            PlayerPrefs.SetString(Key, JsonConvert.SerializeObject(histories));
            PlayerPrefs.Save();
        }

        /// <summary>Ghi nhận vừa thăm: đưa tên lên đầu (thăm lại cũng lên đầu), giữ tối đa 15 tên gần nhất.</summary>
        public void AddHistory(string name)
        {
            name = name?.Trim();
            if (string.IsNullOrEmpty(name)) return;
            var histories = GetHistories();
            histories.RemoveAll(x => string.Equals(x, name, System.StringComparison.OrdinalIgnoreCase));
            histories.Insert(0, name);
            if (histories.Count > MaxHistories) histories.RemoveRange(MaxHistories, histories.Count - MaxHistories);
            Save(histories);
        }
    }
}