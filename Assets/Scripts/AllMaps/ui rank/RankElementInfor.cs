using Base;
using CreateCharacter;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum RankType
{
    RankLevel, // cấp bậc (luyện khí sơ trung hậu ... )
    RankTongNap, // giá trị (200, 300, 400 ... )
    RankTongTieu,
    RankArena,
}
public class RankElementInfor : BaseBehaviour
{
    [SerializeField] private CharacterGraphic graphicChar; // tuổi
    [SerializeField] private Text txtValue; // tuổi
    [SerializeField] private Text txtRank;
    [SerializeField] private Text txtName;
    [SerializeField] private Text txtValueRank; // level (luyện khí sơ trung hậu ... ) // 200
    [SerializeField] private List<Sprite> spriteLevel;
    [SerializeField] private List<Sprite> spriteRank;
    [SerializeField] private Sprite spriteTienNgoc;
    public void Init(int tuoitac , int sttRank , string nameUser,int valueRank , Gender gender, Dictionary<string, string> skin, Dictionary<string, Color> colors, RankType rankType)
    {
        graphicChar.SetGender(gender);
        graphicChar.SetSkin(skin, colors);
        txtValue.text = tuoitac.ToString();
        if(txtRank != null)
        {
            txtRank.text = sttRank.ToString();
        }
        txtName.text = nameUser;
        var imageIcon = txtValueRank.transform.GetChild(0).GetComponent<Image>();
        if (rankType == RankType.RankLevel)
        {
            GameManager.levelWithText.TryGetValue(valueRank, out string textLevel);
            txtValueRank.text = textLevel;
            imageIcon.sprite = spriteLevel[valueRank - 1];
            txtValue.transform.parent.gameObject.SetActive(true);
            imageIcon.transform.localScale = Vector3.one * 0.5f;

        }
        else if(rankType == RankType.RankTongNap || rankType == RankType.RankTongTieu)
        {
            txtValueRank.text = valueRank.ToString(); 
            imageIcon.sprite = spriteTienNgoc;
            txtValue.transform.parent.gameObject.SetActive(false);
            imageIcon.transform.localScale = Vector3.one * 0.5f;

        }
        else if (rankType == RankType.RankArena)
        {
            txtValueRank.text = valueRank.ToString();
            var typeRank = ArenaManager.GetArenaRankWithPoint(valueRank);
            imageIcon.sprite = spriteRank[(int)typeRank-1];
            txtValue.transform.parent.gameObject.SetActive(false);
            imageIcon.transform.localScale = Vector3.one * 0.05f;

        }
        imageIcon.SetNativeSize();
    }    
}
