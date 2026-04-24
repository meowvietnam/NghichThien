using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image progressBar;
        [SerializeField] private Text text;
        [SerializeField] private GameObject selfObject;
        private bool isCompleted;
        private Action completed;
        public void SetText(string text)
        {
            this.text.text = text;
        }
        public void SetProgress(float progress)
        {
            progressBar.fillAmount = progress;
        }
        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
        }
        public void DoShow(float minTime,float completedTime,Action<float> progressChanged)
        {
            isCompleted = false;
            SetActive(true);
            StartCoroutine(Show(minTime,completedTime,progressChanged));
        }
        private IEnumerator Show(float minTime,float completedTime,Action<float> progressChanged)
        {
            float timer = 0f;
            float currentProgress = 0f;
            while (!isCompleted || timer < minTime)
            {
                timer += Time.deltaTime;
                if (timer <= minTime)
                {
                    currentProgress = Mathf.Lerp(0f, 0.9f, timer / minTime);
                }
                else
                {
                    currentProgress = Mathf.Lerp(0.9f, 0.99f, (timer - minTime) / (minTime + 1f));
                }
                SetProgress(currentProgress);
                progressChanged?.Invoke(currentProgress);
                yield return null;
            }
            if (completedTime > 0)
            {
                float finishTimer = 0f;
                float startProgress = currentProgress;

                while (finishTimer < completedTime)
                {
                    finishTimer += Time.deltaTime;
                    currentProgress = Mathf.Lerp(startProgress, 1f, finishTimer / completedTime);
                    SetProgress(currentProgress);
                    progressChanged?.Invoke(currentProgress);
                    yield return null;
                }
            }
            SetProgress(1f);
            progressChanged?.Invoke(1f);
            completed?.Invoke();
            SetActive(false);
        }
       
        public void DoKill(Action completed)
        {
            isCompleted = true;
            this.completed = completed;
        }
    }
}
