using CreateCharacter;
using Network;
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

    public void InitVisual(CharacterStatsSecure stats , CharacterSnapshot snapshot)
    {
        graphicChar.SetGender(stats.Gender);
        graphicChar.SetSkin(stats.Skins, stats.Colors);
        GameManager.levelWithText.TryGetValue(stats.level, out string textLevel);
        txtLevel.text = textLevel;
        txtName.text = stats.nameUser;
        iconLevel.sprite = spriteRefLevel[stats.level - 1];
        int valuePoint = stats.pointArena;
        if(snapshot == null)
        {
            valuePoint = Client.Instance.CharacterData.ArenaManager.Points;
        }    
        var currentRank = ArenaManager.GetArenaRankWithPoint(stats.pointArena.Value);

        iconRank.sprite = spriteRefRank[(int)currentRank - 1];
        iconRank.SetNativeSize();
        iconRankName.sprite = spriteRefRankName[(int)currentRank - 1];
        iconRankName.SetNativeSize();
    }

}
