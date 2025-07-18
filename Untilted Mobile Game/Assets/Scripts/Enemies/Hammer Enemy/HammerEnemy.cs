using UnityEngine;

namespace EnemyAI.HammerEnemy
{
    public class HammerEnemy : EnemyBase
    {
        protected override void Update()
        {
            base.Update();
        }

        protected override void Attack()
        {
            ExecuteHammerPunch();
        }

        #region HAMMER PUNCH

        private void ExecuteHammerPunch()
        {
            animator.SetTrigger("HammerPunch");
            animator.SetInteger("ComboIndex", Random.Range(1, 5));
        }

        #endregion HAMMER PUNCH
    }
}
