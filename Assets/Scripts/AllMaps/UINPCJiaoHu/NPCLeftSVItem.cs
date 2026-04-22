using CreateCharacter;
using LongDH.LongDHModules;
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
    [SerializeField]
    private Button btn;

   public void Init(CharacterStatsSecure charStats)
   {
        // 1. Lấy giá trị thân mật ra trước, nếu không có thì mặc định là 0
        if (!Client.Instance.CharacterData.RelationShip.TryGetValue(charStats.idChar, out var relationshipValue))
        {
            relationshipValue = 0; // Hoặc một giá trị mặc định bất kỳ
        }

        // 2. Tính toán Enum quan hệ một lần duy nhất để tránh gọi hàm lặp lại
        var currentERelation = UINPCJiaoHu.GetERelationShip(relationshipValue, UINPCJiaoHu.ETypeSkillRelationShip.ThanThong);

        // 3. Gán dữ liệu lên UI
        this.textRelationship.text = UINPCJiaoHu.relationShipText[currentERelation];
        this.imageHeart.color = UINPCJiaoHu.relationShipColor[currentERelation];
        textName.text = charStats.nameUser;
        characterGraphic.SetGender(charStats.Gender);
        characterGraphic.SetSkin(charStats.Skins, charStats.Colors);
        textMonPhai.text = GameManager.GetChucVuText(charStats.monPhai, charStats.level);
        SetEventButton(btn, () =>
        {

            Pool.Get<UINPCJiaoHu>().ShowNpcPop(charStats);

        });
   }    
}
