using Base;
using CreateCharacter;
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
    public Popup panel => textContent.transform.parent.GetComponent<Popup>();
    public Button btnPanel;
    private bool isTyping = false; // Đang chạy chữ
    private bool isDone = false;   // Đã hiện full chữ
    private string fullContent; // Lưu lại nội dung đầy đủ để hiện nhanh khi cần
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Text textName;
    public static SayDialogManager Instance;
    private List<string> dialogContents = new List<string>();
    private List<AudioClip> dialogVoices = new List<AudioClip>();
    private List<Sprite> dialogAvatar = new List<Sprite>();

    private int currentDialogIndex = 0;
    private Action onDialogComplete; // Lưu lại action cuối cùng
    private bool isAutoHideAfterDoneDialog = true;
    private void Awake()
    {
        Init();
    }
    public void ShowDialogRandomSkin(string name,Gender gender, List<string> contents, Sprite spriteContent = null, List<AudioClip> voices = null, Action actionEndThisDialog = null, bool isAutoHideAfterDoneDialog = true)
    {
       
        var skinRandom = characterGraphic.RandomSkin(gender, false);
        ShowDialogWithSkin(name, gender, skinRandom.skins, skinRandom.colors, contents, spriteContent, voices, actionEndThisDialog, isAutoHideAfterDoneDialog);
    }
    // 1. Hàm ShowDialog với danh sách ngẫu nhiên Skin
    public (Gender gender,Dictionary<string,string> skin ,Dictionary<string,Color> color) ShowDialogRandomSkin(string name, List<string> contents, Sprite spriteContent = null, List<AudioClip> voices = null, Action actionEndThisDialog = null, bool isAutoHideAfterDoneDialog = true)
    {
        System.Random _random = new System.Random();
        Array values = Enum.GetValues(typeof(Gender));
        Gender randomGender = (Gender)values.GetValue(_random.Next(values.Length));
        var skinRandom = characterGraphic.RandomSkin(randomGender, false);
        ShowDialogWithSkin(name, randomGender, skinRandom.skins, skinRandom.colors, contents, spriteContent, voices, actionEndThisDialog,isAutoHideAfterDoneDialog);
        return (randomGender,skinRandom.skins,skinRandom.colors);
    }

    // 2. Hàm ShowDialog hiển thị thông tin User
    public void ShowDialogForUser(List<string> contents, Sprite spriteContent = null, List<AudioClip> voices = null, Action actionEndThisDialog = null, bool isAutoHideAfterDoneDialog = true)
    {
        var charData = Client.Instance.CharacterData;
        ShowDialogWithSkin(charData.Name, charData.Gender, charData.Appearance.Skins, charData.Appearance.Colors, contents, spriteContent, voices, actionEndThisDialog , isAutoHideAfterDoneDialog);
    }

    // 3. Hàm chạy Dialog chính (Nhận List)
    [Button]
    public void ShowDialogWithSkin(string name, Gender gender, Dictionary<string, string> skins, Dictionary<string, Color> colors, List<string> contents, Sprite spriteContent = null, List<AudioClip> voices = null, Action actionEndThisDialog = null , bool isAutoHideAfterDoneDialog = true)
    {

        imgChar.gameObject.SetActive(false);
        characterGraphic.gameObject.SetActive(true);
        characterGraphic.SetGender(gender);
        characterGraphic.SetSkin(skins, colors);

        ShowDialog(name, contents, spriteContent, voices, actionEndThisDialog, isAutoHideAfterDoneDialog);


    }
    public void ShowDialogWithAvatar(string name, List<Sprite> avatars, List<string> contents, Sprite spriteContent = null, List<AudioClip> voices = null, Action actionEndThisDialog = null, bool isAutoHideAfterDoneDialog = true , float scale =1)
    {
        characterGraphic.gameObject.SetActive(false);

        imgChar.gameObject.SetActive(true);
        imgChar.transform.localScale = Vector3.one*scale;
        dialogAvatar = avatars;
        ShowDialog(name, contents, spriteContent, voices, actionEndThisDialog, isAutoHideAfterDoneDialog);

    }
    public void ShowDialog(string name, List<string> contents, Sprite spriteContent = null, List<AudioClip> voices = null, Action actionEndThisDialog = null, bool isAutoHideAfterDoneDialog = true)
    {

        // Lưu trữ danh sách dữ liệu truyền vào
        dialogContents = contents ?? new List<string>();
        dialogVoices = voices ?? new List<AudioClip>();
        onDialogComplete = actionEndThisDialog;
        currentDialogIndex = 0; // Reset về câu đầu tiên
        this.isAutoHideAfterDoneDialog = isAutoHideAfterDoneDialog;
        // Thiết lập thông tin nhân vật
        textName.text = name;
     

        // Thiết lập Image Content (Sprite phụ nếu có)
        imgContent.gameObject.SetActive(false);
        if (!panel.gameObject.activeSelf)
        {
            panel.Show();
        }

        if (spriteContent != null)
        {
            imgContent.gameObject.SetActive(true);
            imgContent.sprite = spriteContent;
            //Color c = imgContent.color;
            //c.a = 0;
            //imgContent.color = c;
            //imgContent.DOFade(1, 1.5f);
        }

        // Đăng ký sự kiện Click cho Button Panel (Chỉ đăng ký 1 lần duy nhất ở đây)
        SetEventButton(btnPanel, HandlePanelClick);

        // Bắt đầu hiển thị câu đầu tiên
        DisplayCurrentSubDialog();

    }
    // Hàm bổ trợ: Xử lý hiển thị từng câu thoại dựa vào currentDialogIndex
    private void DisplayCurrentSubDialog()
    {
        if (currentDialogIndex >= dialogContents.Count)
        {
            FinishAllDialogs();
            return;
        }

        fullContent = dialogContents[currentDialogIndex]; // Lưu text của câu hiện tại
        isDone = false;
        isTyping = false;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        // Kiểm tra xem câu hiện tại có Voice đi kèm không
        AudioClip currentVoice = null;
        if (dialogVoices != null && currentDialogIndex < dialogVoices.Count)
        {
            currentVoice = dialogVoices[currentDialogIndex];
        }
        if(dialogAvatar != null && currentDialogIndex < dialogAvatar.Count)
        {
            imgChar.sprite = dialogAvatar[currentDialogIndex];
            imgChar.SetNativeSize();
        }    
        float duration = (currentVoice != null) ? currentVoice.length : 1.5f;
        if (currentVoice != null)
        {
            audioSource.clip = currentVoice;
            audioSource.Play();
        }

        typingCoroutine = StartCoroutine(TypeText(fullContent, duration));
    }

    // Hàm bổ trợ: Xử lý Logic khi click vào Panel
    private void HandlePanelClick()
    {
        // Kiểm tra xem câu hiện tại có kèm Voice không
        AudioClip currentVoice = (dialogVoices != null && currentDialogIndex < dialogVoices.Count) ? dialogVoices[currentDialogIndex] : null;

        // 1. Nếu ĐANG CÓ VOICE: Bắt buộc phải đợi chữ chạy xong hoàn toàn (isDone = true) mới cho Next
        if (currentVoice != null)
        {
            if (isDone)
            {
                NextOrFinish();
            }
            return;
        }

        // 2. Nếu KHÔNG CÓ VOICE: Cho phép người chơi Skip text nhanh
        if (isTyping)
        {
            // Click lần 1: Ngắt hiệu ứng chạy, hiện toàn bộ text ngay lập tức
            StopCoroutine(typingCoroutine);
            textContent.text = fullContent;
            isTyping = false;
            isDone = true;
        }
        else if (isDone)
        {
            // Click lần 2: Đã hiện full chữ xong, bấm cái nữa để qua câu tiếp theo
            NextOrFinish();
        }
    }

    // Hàm điều hướng: Sang câu tiếp theo hoặc kết thúc
    private void NextOrFinish()
    {
        currentDialogIndex++; // Tăng chỉ số câu thoại lên 1

        if (currentDialogIndex < dialogContents.Count)
        {
            // Nếu vẫn còn câu thoại tiếp theo -> Chạy câu tiếp theo
            DisplayCurrentSubDialog();
        }
        else
        {
            // Nếu đã hết danh sách -> Tắt UI và gọi callback kết thúc
            FinishAllDialogs();
        }
    }

    // Hàm bọc xử lý kết thúc toàn bộ hội thoại
    private void FinishAllDialogs()
    {
        onDialogComplete?.Invoke();
        if (audioSource.isPlaying) audioSource.Stop(); // Dừng âm thanh nếu có
        if (!isAutoHideAfterDoneDialog) return;
        HideDialog();

    }
    public void HideDialog()
    {
        panel.Hide();
        imgContent.gameObject.SetActive(false);
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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu đã có một bản Instance rồi, thì xóa bản mới tạo này đi
            Destroy(gameObject);
        }
    }
}
