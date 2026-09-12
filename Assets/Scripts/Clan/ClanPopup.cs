using System;
using DG.Tweening;
using Network;
using Network.Core;
using UnityEngine;

namespace Clan
{
    public class ClanPopup : BaseBehaviour
    {
        [SerializeField] protected Transform myTf;
        [SerializeField] protected GameObject selfObject;
        [SerializeField] private UnityEngine.UI.Button closeButton;
        private Tween tween;
        private ClanHandler clanHandler;
        private bool isListening;

        protected ClanHandler Handler => clanHandler;

        protected virtual void Awake()
        {
            EnsureReferences();
        }

        private void OnValidate()
        {
            EnsureReferences();
        }

        private void EnsureReferences()
        {
            if (myTf == null) myTf = transform;
            if (selfObject == null) selfObject = gameObject;
        }

        protected virtual void Start()
        {
            SetEventButton(closeButton, Close);
            InitializeHandler();
        }

        private void Close()
        {
            Hide();
        }

        public void Show(float time = 0.25f)
        {
            EnsureReferences();
            InitializeHandler();
            SetListening(true);
            tween?.Kill();
            selfObject.SetActive(true);
            tween = myTf.DOScale(Vector3.one, time);
            OnShown();
        }

        public void Hide(float time = 0.25f)
        {
            EnsureReferences();
            tween?.Kill();
            tween = myTf.DOScale(Vector3.one, time).OnComplete(() =>
            {
                SetListening(false);
                selfObject.SetActive(false);
            });
        }

        protected bool InitializeHandler()
        {
            var nextHandler = Client.Instance?.CoreModule?.GetHandler<ClanHandler>();
            if (ReferenceEquals(clanHandler, nextHandler)) return clanHandler != null;

            if (clanHandler != null && isListening) UnbindClanEvents(clanHandler);
            clanHandler = nextHandler;
            if (clanHandler == null)
            {
                Debug.LogWarning($"{GetType().Name}: ClanHandler chưa được khởi tạo", this);
                return false;
            }

            if (isListening) BindClanEvents(clanHandler);
            return true;
        }

        private void SetListening(bool value)
        {
            if (isListening == value || clanHandler == null) return;
            isListening = value;
            if (isListening) BindClanEvents(clanHandler);
            else UnbindClanEvents(clanHandler);
        }

        protected virtual void BindClanEvents(ClanHandler handler)
        {
        }

        protected virtual void UnbindClanEvents(ClanHandler handler)
        {
        }

        protected virtual void OnShown()
        {
        }

        protected virtual void OnDestroy()
        {
            tween?.Kill();
            if (clanHandler != null && isListening) UnbindClanEvents(clanHandler);
            isListening = false;
            clanHandler = null;
        }
    }
}
