using Base;
using CreateCharacter;
using DG.Tweening;
using LongDH.LongDHModules;
using Network;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SayDialogManager : BaseBehaviour
{
    [SerializeField]
    private CharacterGraphic characterGraphic;
    [SerializeField]
    private Image imgChar;
    [SerializeField]
    private Image imgContent;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Text textContent;
    private Coroutine typingCoroutine;
    private Transform panel => textContent.transform.parent;
    public Button btnPanel;
    private bool isTyping = false; // Đang chạy chữ
    private bool isDone = false;   // Đã hiện full chữ
    private string fullContent; // Lưu lại nội dung đầy đủ để hiện nhanh khi cần
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Text textName;
    private void Awake()
    {
        Init();
    }
    public void ShowDialogRandomSkin(string name,Gender gender, string content, Sprite spriteContent = null, AudioClip voice = null, Action actionEndThisDialog = null)
    {
        var skinRandom = characterGraphic.RandomSkin(gender,false);
        ShowDialog(name, gender,skinRandom.skins,skinRandom.colors, content, spriteContent, voice, actionEndThisDialog);
    }
    public void ShowDialogForUser(string content, Sprite spriteContent = null, AudioClip voice = null, Action actionEndThisDialog = null)
    {
        var charData = Client.Instance.CharacterData;
        ShowDialog(charData.Name,charData.Gender,charData.Appearance.Skins,charData.Appearance.Colors,content,spriteContent,voice,actionEndThisDialog);
    }    
    [Button]
    public void ShowDialog(string name,Gender gender,Dictionary<string,string>skins,Dictionary<string,Color>colors,string content, Sprite spriteContent = null, AudioClip voice = null, Action actionEndThisDialog = null)
    {
        textName.text = name;
        characterGraphic.SetGender(gender);
        characterGraphic.SetSkin(skins, colors);
        fullContent = content; // Lưu trữ text đầy đủ
        isDone = false;
        isTyping = false;

        imgContent.gameObject.SetActive(false);
        panel.gameObject.SetActive(true);

        if (spriteContent != null)
        {
            imgContent.gameObject.SetActive(true);
            imgContent.sprite = spriteContent;
            Color c = imgContent.color;
            c.a = 0;
            imgContent.color = c;
            imgContent.DOFade(1, 1.5f);
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        float duration = (voice != null) ? voice.length : 1.5f; // Không voice thì mặc định 1.5s cho dễ đọc
        if (voice != null)
        {
            audioSource.clip = voice;
            audioSource.Play();
        }

        typingCoroutine = StartCoroutine(TypeText(content, duration));

        // Xử lý sự kiện click
        SetEventButton(btnPanel, () =>
        {
            // 1. Nếu đang có voice: Bắt buộc phải chờ xong (theo yêu cầu của bạn)
            if (voice != null)
            {
                if (isDone)
                {
                    DoneAction();

                }
                return;
            }

            // 2. Nếu KHÔNG có voice: Cho phép skip text
            if (isTyping)
            {
                // Bấm lần 1: Ngắt chạy chữ, hiện full ngay
                StopCoroutine(typingCoroutine);
                textContent.text = fullContent;
                isTyping = false;
                isDone = true;
            }
            else if (isDone)
            {
                // Bấm lần 2: Đã xong rồi thì kết thúc
                DoneAction();
            }
        });

        void DoneAction()
        {
            panel.gameObject.SetActive(false);
            imgContent.gameObject.SetActive(false);
            actionEndThisDialog?.Invoke();
        }
    }

    private IEnumerator TypeText(string content, float duration)
    {
        isTyping = true;
        isDone = false;
        textContent.text = "";

        // Tránh chia cho 0 nếu chuỗi rỗng
        float timePerChar = content.Length > 0 ? duration / content.Length : 0.05f;

        foreach (char c in content)
        {
            textContent.text += c;
            yield return new WaitForSeconds(timePerChar);
        }

        isTyping = false;
        isDone = true;
        typingCoroutine = null;
    }

    public void Init()
    {
        DontDestroyOnLoad(this);
        Pool.Set(this);
    }
}
