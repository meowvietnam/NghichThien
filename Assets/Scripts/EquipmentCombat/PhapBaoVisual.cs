using LuyenKhi;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhapBaoVisual : MonoBehaviour
{
    public string ActiveTrigger => "Active";
    public ArtifactType artifactType;
    [SerializeField] private Animator anim;

    [Button]
    public void ActiveAttack()
    {
        anim.SetTrigger(ActiveTrigger);
    }

    public void Init(Sprite spriteVisual)
    {
        gameObject.SetActive(true);
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var item in spriteRenderers)
        {
            item.sprite = spriteVisual;
        }
    }
}