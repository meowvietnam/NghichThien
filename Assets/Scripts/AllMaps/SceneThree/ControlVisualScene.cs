using AllMaps;
using LongDH.LongDHModules;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ControlVisualScene : BaseBehaviour
{
    [Serializable]
    public class SpriteWithEMapType
    {
        public Sprite sprite;
        public EMapType eMapType;
    }
    [SerializeField] protected Image bg;
    [SerializeField] private SpriteWithEMapType[] listSprites;
    [SerializeField] private SpriteWithEMapType CurrentSpriteWithMap => listSprites.FirstOrDefault(x=>x.eMapType == Pool.Get<GameManager>().currentEMapType);

    [SerializeField] public virtual Sprite CurrentSprite => CurrentSpriteWithMap.sprite;
    private void Awake()
    {
        InitBG();
    }
    public virtual void InitBG()
    {
        bg.sprite = CurrentSprite;

    }

}
