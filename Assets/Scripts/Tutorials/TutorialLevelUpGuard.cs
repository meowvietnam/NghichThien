using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Tutorials
{
    /// <summary>
    /// Tiện ích dùng chung cho mọi mạch hand-tut: popup thăng cấp (LevelUpPopup) bật lên qua
    /// Observer LEVEL_UP nên có thể nhảy ra BẤT CỨ LÚC NÀO sau khi thời gian trong game trôi
    /// (di chuyển trên bản đồ, tu luyện, lĩnh ngộ, đột phá...) và nó che mất bước đang hướng dẫn.
    ///
    /// Gọi <see cref="Guide"/> trước khi hiện một bước mới: nếu popup đang bật thì chờ hiệu ứng
    /// phóng to xong rồi khoanh sáng nút đóng của nó, xong mới trả quyền lại cho mạch chính.
    /// Không có popup thì thoát ngay, gần như không tốn gì.
    /// </summary>
    public static class TutorialLevelUpGuard
    {
        private static LevelUpPopup cached;

        public static Popup GetPopup()
        {
            if (cached == null)
            {
                cached = Object.FindObjectsByType<LevelUpPopup>(
                        FindObjectsInactive.Include,
                        FindObjectsSortMode.None)
                    .FirstOrDefault();
            }

            return cached != null ? cached.MainPopup : null;
        }

        public static bool IsOpened()
        {
            var popup = GetPopup();
            return popup != null && popup.gameObject.activeInHierarchy;
        }

        public static IEnumerator Guide(Tutorial tutorial, float appearTimeout = 0.75f)
        {
            if (tutorial == null) yield break;

            // Popup có thể chưa kịp bật (server trả LEVEL_UP trễ hơn một chút) -> chờ ngắn.
            float timeLeft = appearTimeout;
            while (timeLeft > 0f && !IsOpened())
            {
                timeLeft -= Time.deltaTime;
                yield return null;
            }

            if (!IsOpened()) yield break;

            var popup = GetPopup();

            // Chờ DOScale của Popup.Show() xong rồi mới tính khung sáng, tránh lệch vị trí.
            float animLeft = 2f;
            while (animLeft > 0f && DOTween.IsTweening(popup.transform))
            {
                animLeft -= Time.deltaTime;
                yield return null;
            }

            yield return null;

            var btnClose = popup.BtnClose;
            if (btnClose == null)
            {
                // Không tìm được nút đóng thì cứ chờ người chơi tự tắt, không hiện gì cả.
                yield return new WaitWhile(IsOpened);
                yield break;
            }

            tutorial.ShowTutorial(new[]
            {
                new TutorialStep
                {
                    TargetRect = btnClose.GetComponent<RectTransform>(),
                    Text = "Chúc mừng đạo hữu thăng cấp! Chạm vào đây để đóng.",
                    RequireRealAction = true
                }
            });

            yield return new WaitWhile(IsOpened);
            tutorial.CompleteInteractiveStep();
        }
    }
}
