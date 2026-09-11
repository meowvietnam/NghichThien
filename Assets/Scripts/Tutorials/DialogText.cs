using System;
using System.Collections;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorials
{
    public class DialogText : BaseBehaviour
    {
        [SerializeField] private Text text;

        [SerializeField] private float scaleDuration = 0.2f;
        [SerializeField] private float typingDelay = 0.03f;

        private Tween tween;
        private Coroutine typingCoroutine;
        private string currentContent;
        public bool IsTyping { get; private set; }

        public void ShowText(string content)
        {
            IsTyping = true;
            currentContent = content;

            gameObject.SetActive(true);

            tween?.Kill();

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            transform.localScale = Vector3.zero;
            text.text = string.Empty;

            tween = transform
                .DOScale(Vector3.one, scaleDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    typingCoroutine = StartCoroutine(
                        TypeTextCoroutine(currentContent, typingDelay)
                    );
                });
        }

        private IEnumerator TypeTextCoroutine(string content, float delay)
        {
            text.text = string.Empty;

            for (int i = 0; i < content.Length; i++)
            {
                text.text = content.Substring(0, i + 1);
                yield return new WaitForSeconds(delay);
            }

            typingCoroutine = null;
            IsTyping = false;
        }

        public void Complete()
        {
            IsTyping = false;
            tween?.Kill();
            tween = null;

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            transform.localScale = Vector3.one;
            text.text = currentContent;
        }

        public void Close()
        {
            IsTyping = false;
            tween?.Kill();

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            tween = transform
                .DOScale(Vector3.zero, scaleDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => { gameObject.SetActive(false); });
        }

        private void OnDestroy()
        {
            tween?.Kill();

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
        }

        private void OnDisable()
        {
            IsTyping = false;
        }
    }
}