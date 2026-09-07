using System.Collections;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Auction
{
    public class InfoPanel : BasePanel
    {
        [SerializeField] private Text content;
        [SerializeField] private RectTransform historyContainer;
        [SerializeField] private HistoryCell historyCellPrefab;
        private Coroutine refreshLayoutCoroutine;

        public virtual void Init(AuctionInfo auctionInfo)
        {
            content.text = auctionInfo.Content;
            var sessions = auctionInfo.AuctionSessions;
            var cells = historyContainer.GetComponentsInChildren<HistoryCell>();
            foreach (var cell in cells)
            {
                cell.DestroySelf();
            }

            if (sessions == null)
            {
                RefreshLayout();
                return;
            }

            foreach (var session in sessions)
            {
                var cell = Instantiate(historyCellPrefab, historyContainer);
                cell.Init(session);
            }

            RefreshLayout();
        }

        private void RefreshLayout()
        {
            RebuildLayout();
            if (refreshLayoutCoroutine != null)
            {
                StopCoroutine(refreshLayoutCoroutine);
            }

            refreshLayoutCoroutine = StartCoroutine(RefreshLayoutNextFrame());
        }

        private IEnumerator RefreshLayoutNextFrame()
        {
            yield return null;
            RebuildLayout();
            refreshLayoutCoroutine = null;
        }

        private void RebuildLayout()
        {
            Canvas.ForceUpdateCanvases();
            Rebuild(content.rectTransform);
            Rebuild(historyContainer);
            RebuildParents(content.rectTransform);
            RebuildParents(historyContainer);
            Canvas.ForceUpdateCanvases();
        }

        private static void RebuildParents(RectTransform rectTransform)
        {
            var parents = rectTransform.GetComponentsInParent<RectTransform>(true);
            for (var i = parents.Length - 1; i >= 0; i--)
            {
                Rebuild(parents[i]);
            }
        }

        private static void Rebuild(RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                return;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }
}