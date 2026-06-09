using CreateCharacter.CharStats;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SkillTip : Tooltip
{
    [SerializeField] private Text nameTip;
    [SerializeField] private Text cooldownText;
    [SerializeField] private Text typeTipText;
    [SerializeField] private Text textKeyword;

    [SerializeField] private Transform requireLinhKhiParent; // trong này gôm ảnh => text => ảnh => text =>...
    [SerializeField] private Sprite[] spriteLinhKhiTip; // trong này gôm ảnh => text => ảnh => text =>...

    protected override void Awake()
    {
        
    }
    public void ShowSkillTip(string nameTip,string desTip,string typeTip, List<ThanThong.RequireActive> requireLinhKhi,ThanThong thanThong, int level)
    {
        for (int i = 0; i < requireLinhKhiParent.transform.childCount; i++)
        {
            requireLinhKhiParent.transform.GetChild(i).gameObject.SetActive(false);
        }
        Debug.Log("dmmm req" + requireLinhKhi.Count);
        int currentIndex = 0;
        for (int i = 0; i < requireLinhKhi.Count; i++)
        {
            var item = requireLinhKhi[i];
            Image img = requireLinhKhiParent.transform.GetChild(i+currentIndex).transform.GetComponent<Image>();
            Text textCount = requireLinhKhiParent.transform.GetChild(i+1+currentIndex).transform.GetComponent<Text>();
            img.gameObject.SetActive(true);
            textCount.gameObject.SetActive(true);
            img.sprite = spriteLinhKhiTip[(int)item.linhKhiRequire];
            textCount.text = $"x{item.count}";
            currentIndex++; 

        }
        var lines = thanThong.GetEKeywordSkillHave()
      .Select(item => BaseSkill.GetDescKeyWord(item));

        var lines2 = thanThong.GetEKeywordBuffSkillHave(level)
             .Select(item => BaseSkill.GetDescBuffOrDeBuff(item));

        // Gán danh sách 1
        textKeyword.text = string.Join("\n", lines);

        // Thêm xuống dòng và gán danh sách 2
        if (lines2.Any())
        {
            textKeyword.text += "\n" + string.Join("\n", lines2);
        }

        this.cooldownText.gameObject.SetActive(false);
        this.text.text = $"<color=#cda14c>Hiệu quả: </color> {desTip}";
        this.nameTip.text = nameTip;
        this.typeTipText.text = typeTip;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(thisTransform);
        textKeyword.gameObject.SetActive(!string.IsNullOrEmpty(textKeyword.text));
        DoShow();
    }
    public void ShowWeaponTip()
    {
       
    }    
}
