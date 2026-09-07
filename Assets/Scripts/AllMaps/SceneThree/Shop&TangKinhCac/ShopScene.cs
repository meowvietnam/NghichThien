using AllMaps;
using Base;
using LongDH.LongDHModules;
using System;
using System.Linq;
using LongDHModules;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShopScene : ControlVisualScene
{
    [SerializeField] private Sprite baseShopSprite;
    [SerializeField] private Sprite tangKinhCacSprite;

    [Serializable]
    public class ShopTypeWithEMapType
    {
        public EShopType eShopType;
        public EMapType eMapType;
    }

    public override Sprite CurrentSprite =>
        Pool.Get<GameManager>().CurrentEMonPhai == EMonPhai.None ? baseShopSprite : tangKinhCacSprite;

    [SerializeField] private ShopTypeWithEMapType[] shopTypeWithEMapTypes;

    [ShowInInspector]
    private EShopType CurrentShopType => Pool.Get<GameManager>().CurrentEMonPhai == EMonPhai.None
        ? shopTypeWithEMapTypes.FirstOrDefault(x => x.eMapType == Pool.Get<GameManager>().currentEMapType).eShopType
        : EShopType.Skill;

    public override void InitBG()
    {
        base.InitBG();
        Pool.Get<ShopUI>().ShowInitShop(CurrentShopType);
    }
}