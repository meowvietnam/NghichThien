using System;
using CreateCharacter;
using LongDH.LongDHModules;
using Network;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps.TongMonChien
{
    /// <summary>Gắn vào nút ở AllMaps/giao diện tông môn để vào scene Tông môn chiến.</summary>
    [RequireComponent(typeof(Button))]
    public class TongMonChienEntryButton : MonoBehaviour
    {
        public const string SceneName = "TongMonChien";

        // Khớp WarConfig phía server (server vẫn là nơi quyết định, đây chỉ là chặn sớm để báo lỗi ngay).
        public const int MinMaxLevel = 4;
        public static readonly TimeSpan OpenTime = new TimeSpan(19, 0, 0);
        public static readonly TimeSpan CloseTime = new TimeSpan(23, 0, 0);
        private static readonly TimeSpan Utc7 = TimeSpan.FromHours(7);

        public static bool IsOpen(DateTime utcNow)
        {
            var time = (utcNow + Utc7).TimeOfDay;
            return OpenTime <= CloseTime
                ? time >= OpenTime && time < CloseTime
                : time >= OpenTime || time < CloseTime;
        }

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Enter);
        }

        public static void Enter()
        {
            // GameManager.OnLoadScene gửi ChangeScene("TongMonChien"); server kiểm tra tông/thành rồi mới cho vào.
            Observer.Notify(EventList.GamePlay.LOAD_SCENE, SceneName);
        }
    }
}
