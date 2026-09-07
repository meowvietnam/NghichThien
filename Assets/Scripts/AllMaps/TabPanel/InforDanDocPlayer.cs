using CreateCharacter;
using Network;
using UnityEngine;

public class InforDanDocPlayer : BtnInforStats
{
    protected override string Infor => GetInforDanDoc();
    [SerializeField] private string[] listTextInforDanDoc;

    protected string GetInforDanDoc()
    {
        CharacterData CharData = Client.Instance.CharacterData;

        var CharacterDatas = CharData;

        var CharacterStats = CharacterDatas.Stats;
        var getDanDoc = GameManager.GetDanDocValue(CharacterDatas.Stats.DanDoc);
        return listTextInforDanDoc[getDanDoc.index];
    }
}