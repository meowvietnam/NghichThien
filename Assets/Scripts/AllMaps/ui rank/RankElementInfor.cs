using System.Collections.Generic;
using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;
using ArenaManager = AllMaps.DauTruong.ArenaManager;

namespace AllMaps.ui_rank
{
    public enum RankType
    {
        RankLevel,
        RankTongNap,
        RankTongTieu,
        RankArena,
        RankMonster
    }

    public class RankElementInfor : BaseBehaviour
    {
        [SerializeField] private CharacterGraphic graphicChar;
        [SerializeField] private Text txtValue;
        [SerializeField] private Text txtRank;
        [SerializeField] private Text txtName;
        [SerializeField] private Text txtValueRank;
        [SerializeField] private List<Sprite> spriteLevel;
        [SerializeField] private List<Sprite> spriteRank;
        [SerializeField] private Sprite spriteTienNgoc;
        [SerializeField] private Sprite spriteDietQuai;
        [SerializeField] private Button infoButton;
        private CharacterStatsSecure charData;
        public void Init(CharacterData charData, int rank, RankType rankType, int valueRank)
        {
            this.charData = CharacterStatsSecureBuilder.ArenaFromCharacterSnapshot(new CharacterSnapshot(charData));
            SetEventButton(infoButton, ShowPopupInfo);
            var tuoiTac = GameManager.GetIntTuoiTac(charData.Stats.TuoiTac);
            var name = charData.Name;
            var gender = charData.Gender;
            var skin = charData.Appearance.Skins;
            var color = charData.Appearance.Colors;
            Init(tuoiTac, rank, name, valueRank, gender, skin, color, rankType);
        }

        private void ShowPopupInfo()
        {
            Observer.Notify(EventList.GamePlay.SHOW_RANK_INFO_POPUP, charData);
        }
        public void Init(int tuoitac, int sttRank, string nameUser, int valueRank, Gender gender,
            Dictionary<string, string> skin, Dictionary<string, Color> colors, RankType rankType)
        {
            graphicChar.SetGender(gender);
            graphicChar.SetSkin(skin, colors);
            txtValue.text = tuoitac.ToString();
            if (txtRank != null)
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
            else if (rankType == RankType.RankTongNap || rankType == RankType.RankTongTieu)
            {
                txtValueRank.text = Utils.FormatCount(valueRank);
                imageIcon.sprite = spriteTienNgoc;
                txtValue.transform.parent.gameObject.SetActive(false);
                imageIcon.transform.localScale = Vector3.one * 0.4f;
            }
            else if (rankType == RankType.RankArena)
            {
                txtValueRank.text = Utils.FormatCount(valueRank);
                var typeRank = ArenaManager.GetArenaRankWithPoint(valueRank);
                imageIcon.sprite = typeRank == ETypeArenaRank.None ? null : spriteRank[(int)typeRank - 1];
                txtValue.transform.parent.gameObject.SetActive(false);
                imageIcon.transform.localScale = Vector3.one * 0.05f;
            }
            else if (rankType == RankType.RankMonster)
            {
                txtValueRank.text = Utils.FormatCount(valueRank);
                imageIcon.sprite = spriteDietQuai;
                txtValue.transform.parent.gameObject.SetActive(false);
                imageIcon.transform.localScale = Vector3.one * 0.4f;
            }
            imageIcon.SetNativeSize();
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}