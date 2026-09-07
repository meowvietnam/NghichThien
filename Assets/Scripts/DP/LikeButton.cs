using System;
using DG.Tweening;
using Network;
using Network.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class LikeButton : BaseBehaviour
{
    [SerializeField] private Button button;

    private void Start()
    {
        transform.DOScale(Vector3.one * 1.2f, 0.75f).SetLoops(-1, LoopType.Yoyo);
        SetEventButton(button, OnLikeClick);
    }

    private void OnLikeClick()
    {
        Client.Instance.Send(MessageTag.LikeDongPhu);
    }
}