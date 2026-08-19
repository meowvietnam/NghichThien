using System;
using Base;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using CreateCharacter.CharStats;
using CreateCharacter.States;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class CreateCharacterPanel : MonoBehaviour, IPanel
    {
        [SerializeField] private Gender initializeGender;
        [SerializeField] private PlayerAvatar avatar;
        [SerializeField] private SetPlayerAvatar setPlayerAvatar;
        [SerializeField] private SelectStats selectStats;
        [SerializeField] private List<SelectGender> selectGenders;
        [SerializeField] private GameObject thisObject;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button backBtn;
        [SerializeField] private Button nextBtn;
        public CanvasGroup CanvasGroup => canvasGroup;
        public SelectStats SelectStats => selectStats;
        public Tween Tween { get; set; }
        private bool isInit;
        private ICreateCharacterState state;

        public void Init(ICreateCharacterState initState, SkinResources skinResources, int money,
            CharacterData charData = null, CharacterStats stats = null)
        {
            if (isInit)
            {
                SetState(initState);
                return;
            }

            isInit = true;
            avatar.Init(skinResources, charData);
            setPlayerAvatar?.Init(skinResources);
            selectStats.Init(this, money, stats);
            foreach (var item in selectGenders)
            {
                item?.Init();
            }

            var selectGender = selectGenders.FirstOrDefault(s => s != null && s.Gender == initializeGender);
            selectGender?.ToggleOn();
            backBtn.onClick.AddListener(Back);
            nextBtn.onClick.AddListener(Next);
            SetState(initState);
        }

        public void SetState(ICreateCharacterState newState)
        {
            state?.OnExit();
            state = newState;
            state?.OnEnter();
        }

        public void SetActive(bool active)
        {
            thisObject.SetActive(active);
        }

        private void Next()
        {
            state?.Next();
        }

        private void Back()
        {
            state?.Back();
        }

        private void OnDestroy()
        {
            backBtn.onClick.RemoveAllListeners();
            nextBtn.onClick.RemoveAllListeners();
        }
    }
}