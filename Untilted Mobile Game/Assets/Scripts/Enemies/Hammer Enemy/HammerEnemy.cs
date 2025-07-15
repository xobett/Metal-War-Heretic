using System.Collections;
using UnityEngine;

namespace EnemyAI.HammerEnemy
{
    public class HammerEnemy : EnemyBase
    {
        [Header("HAMMER ENEMY SETTINGS")]
        [SerializeField] private Animator anim_HammerEnemy;

        private bool isPunching;

        #region ENEMY BASE OVERRIDES

        protected override void Update()
        {
            base.Update();
        }

        protected override void Attack()
        {
            StartCoroutine(ThrowHammerPunch());
        }

        protected override void LookAtPlayer()
        {
            if (!isPunching)
            {
                base.LookAtPlayer();
            }
        }

        #endregion ENEMY BASE OVERRIDES

        #region ATTACK ABILITY
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Hit player");
            }
        }

        private IEnumerator ThrowHammerPunch()
        {
            anim_HammerEnemy.SetTrigger("Attack");
            isPunching = true;

            yield return new WaitForSeconds(4);

            isPunching = false;
            isExecutingAttack = false;
            yield return null;
        }

        #endregion ATTACK ABILITY
    }
}
