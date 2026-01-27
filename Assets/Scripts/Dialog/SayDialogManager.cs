using CreateCharacter;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SayDialogManager : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            characterData = new CharacterData
            {
                Skins = new Dictionary<string, string>
        {
            { "face_shape", "head_0" },
            { "eyes", "eyes_0" },
            { "mouth", "mouth_0" },
            { "nose", "nose_0" },
            { "eyebrows", "eyebrow_0" },
            { "hair", "hair_5" },
            { "face_tattoo", "feature_0" },
            { "eyes_decor", "yanying_0" },
            { "suit", "a_suit_87" }
        }, // Dùng dấu phẩy ở đây thay vì chấm phẩy

                Colors = new Dictionary<string, Color>
        {
            { "eyes", new Color(0.949f, 0.957f, 0.980f, 1.000f) },
            { "eyebrows", new Color(0.439f, 0.439f, 0.439f, 1.000f) },
            { "hair", new Color(0.392f, 0.361f, 0.361f, 1.000f) },
            { "face_tattoo", new Color(0.439f, 0.439f, 0.439f, 1.000f) }
        }
            };

    }
    [Button]
    public void ShowDialog(string content, Sprite spriteContent = null, AudioClip voice = null)
    {
        characterGraphic.SetSkin(characterData.Skins, characterData.Colors);
        imgContent.gameObject.SetActive(false);
        panel.gameObject.SetActive(true);
        if (spriteContent != null)
        {
            imgContent.gameObject.SetActive(true);
            imgContent.sprite = spriteContent;
            imgContent.SetNativeSize();
        }

        // Xử lý Voice và Chữ chạy
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        float duration = 1f; // Mặc định 1 giây nếu không có voice
        if (voice != null)
        {
            duration = voice.length; // Chạy theo độ dài của clip
            audioSource.clip = voice;
            audioSource.Play();
        }

        typingCoroutine = StartCoroutine(TypeText(content, duration));
    }
    private IEnumerator TypeText(string content, float duration)
    {
        textContent.text = "";
        float timePerChar = duration / content.Length;

        foreach (char c in content)
        {
            textContent.text += c;
            yield return new WaitForSeconds(timePerChar);
        }

        typingCoroutine = null;
    }

}
