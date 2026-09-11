using LongDH.LongDHModules;
using LongDHModules;
using UnityEngine;

public class QuangTruongControl : ControlVisualScene
{
    [SerializeField] private Sprite baseQuangTruongSprite;

    public override Sprite CurrentSprite => Pool.Get<GameManager>().CurrentEMonPhai == Base.EMonPhai.None
        ? baseQuangTruongSprite
        : base.CurrentSprite;
}