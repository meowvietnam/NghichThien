using UnityEngine;

namespace AllMaps
{
    public class CombatVisual : BaseBehaviour
    {
        [SerializeField] private PrepareElementArenaCombat me;
        [SerializeField] private PrepareElementArenaCombat enemy;

        public void Show(GameManager.CharCombatInit me, GameManager.CharCombatInit enemy)
        {
            gameObject.SetActive(true);
            this.me.InitVisual(me.charStats, null);
            this.enemy.InitVisual(enemy.charStats, null);
        }
    }
}