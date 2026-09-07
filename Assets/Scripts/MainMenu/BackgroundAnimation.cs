using System;
using DG.Tweening;
using UnityEngine;

namespace MainMenu
{
    public class BackgroundAnimation : MonoBehaviour
    {
        [SerializeField] private Transform selfTransform;
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 endPosition;
        [SerializeField] private float time;

        private void OnValidate()
        {
            if (selfTransform == null)
            {
                selfTransform = transform;
                endPosition = selfTransform.position;
            }
        }

        private void Start()
        {
            selfTransform.position = startPosition;
            selfTransform.DOMove(endPosition, time);
        }
    }
}