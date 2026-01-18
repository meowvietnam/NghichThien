using System;
using System.Collections.Generic;
using Base;
using DG.Tweening;
using UnityEngine;

namespace CreateCharacter
{
    public class NotifyGroup : Pool<NotifySlot>
    {
        private List<NotifySlot> showings = new List<NotifySlot>();
        public static NotifyGroup Instance { get; private set; }
        private void Start()
        {
            Instance = this;
            Init();
        }
        public void Show(string text)
        {
            var slot = Get();
            slot.transform.SetAsLastSibling();
            slot.SetText(text);
            var sequence = DOTween.Sequence();
            sequence.Append(slot.Show(1));
            sequence.AppendInterval(2f);
            sequence.Append(slot.Hide(1));
            sequence.OnComplete(() =>
            {
                showings.Remove(slot);
                Release(slot);
            });
            showings.Add(slot);
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
