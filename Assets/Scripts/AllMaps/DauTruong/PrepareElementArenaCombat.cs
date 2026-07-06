using CreateCharacter;
using Network.Models;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using ArenaManager = AllMaps.DauTruong.ArenaManager;

public class PrepareElementArenaCombat : MonoBehaviour
{
    [SerializeField] private Text txtName;
    [SerializeField] private Text txtLevel;
    [SerializeField] private Image iconLevel;

    [SerializeField] private Image iconRank;
    [SerializeField] private Image iconRankName;
    [SerializeField] private Sprite[] spriteRefRank => ArenaManager.Instance.spriteRefRank;
    [SerializeField] private Sprite[] spriteRefRankName => ArenaManager.Instance.spriteRefRankName;
    [SerializeField] private Sprite[] spriteRefLevel;
    [SerializeField] private CharacterGraphic graphicChar; // tuổi

    public void InitVisual(CharacterStatsSecure stats)
    {
        graphicChar.SetGender(stats.Gender);
        graphicChar.SetSkin(stats.Skins, stats.Colors);
        GameManager.levelWithText.TryGetValue(stats.level, out string textLevel);
        txtLevel.text = textLevel;
        txtName.text = stats.nameUser;
        iconLevel.sprite = spriteRefLevel[stats.level - 1];

        //var currentRank = ArenaManager.GetArenaRankWithPoint(characterSnapshot.PointArena);

        //iconRank.sprite = spriteRefRank[(int)currentRank - 1];
        //iconRank.SetNativeSize();
        //iconRankName.sprite = spriteRefRankName[(int)currentRank - 1];
        //iconRankName.SetNativeSize();
    }

}
