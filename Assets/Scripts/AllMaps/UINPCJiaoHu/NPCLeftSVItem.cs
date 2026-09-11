using System.Linq;
using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using UnityEngine;
using UnityEngine.UI;

public class NPCLeftSVItem : BaseBehaviour
{
    [SerializeField] private Image imageHeart;
    [SerializeField] private CharacterGraphic characterGraphic;
    [SerializeField] private Text textName;
    [SerializeField] private Text textRelationship;
    [SerializeField] private Text textMonPhai;
    [SerializeField] private Button btn;
    public CharacterStatsSecure characterStats;

    public void Init(CharacterStatsSecure charStats)
    {
        this.characterStats = charStats;
        var social = Client.Instance.CharacterData.Social;
        var npcId = charStats.idChar;
        var relationshipValue = social.GetRelationshipWithNPC(npcId);
        var currentERelation =
            UINPCJiaoHu.GetERelationShip(relationshipValue, UINPCJiaoHu.ETypeSkillRelationShip.ThanThong);
        this.textRelationship.text = UINPCJiaoHu.relationShipText[currentERelation];
        this.imageHeart.color = UINPCJiaoHu.relationShipColor[currentERelation];
        textName.text = charStats.nameUser;
        characterGraphic.SetGender(charStats.Gender);
        characterGraphic.SetSkin(charStats.Skins, charStats.Colors);
        textMonPhai.text = GameManager.GetChucVuText(charStats.monPhai, charStats.level);
        SetEventButton(btn, () => { Pool.Get<UINPCJiaoHu>().ShowNpcPop(charStats); });
    }
}