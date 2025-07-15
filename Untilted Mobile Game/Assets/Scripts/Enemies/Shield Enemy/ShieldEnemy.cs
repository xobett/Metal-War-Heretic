using System.Collections;
using UnityEngine;

namespace EnemyAI.ShieldEnemy
{
    public class ShieldEnemy : EnemyBase
    {
        [Header("--- SHIELD ENEMY SETTINGS --- \n")]

        [Header("ROYAL GUARD TIME SETTINGS")]
        [SerializeField, Range(1f, 10f)] private int rgGuardingTime;
        [SerializeField, Range(1f, 10f)] private int rgAfterPushTime;
        [SerializeField, Range(5f, 10f)] private int rgCooldownTime;

        [Header("ROYAL GUARD ROTATION SPEED")]
        [SerializeField, Tooltip("Controls how fast it will look at the player while guarding")]
        private float guardLookSpeed = 0.8f;
        [SerializeField, Tooltip("Controls how fast it will look again at the player after using its ability")]
        private float afterPushLookSpeed = 0.6f;

        private bool rgCooldownActive;
        private bool rgGuardActive;
        private bool rgPlayingAnimations;
        [HideInInspector] public bool rgIsPushing;

        #region BASE OVERRIDES

        protected override void Update()
        {
            base.Update();
            LookSlowAtPlayer();
        }

        protected override void Attack()
        {
            if (!rgCooldownActive)
            {
                StartCoroutine(StartRoyalGuard());
            }
            else
            {
                isExecutingAttack = false;
            }
        }

        public override void HitPlayer(Collider playerCollider)
        {
            PushPlayer(damage);
        }

        protected override void LookAtPlayer()
        {
            if (!rgGuardActive)
            {
                base.LookAtPlayer();
            }
        }

        #endregion BASE OVERRIDES

        #region ATTACK ABILITY

        private void LookSlowAtPlayer()
        {
            if (!rgPlayingAnimations && rgGuardActive)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, guardLookSpeed * Time.deltaTime);
            }
        }

        private IEnumerator StartRoyalGuard()
        {
            rgCooldownActive = true;

            animator.SetTrigger("StartRoyalGuard");

            rgGuardActive = true;
            rgPlayingAnimations = true;
            yield return new WaitForSeconds(5.375f); //Fixed animations duration

            rgPlayingAnimations = false;
            yield return new WaitForSeconds(rgGuardingTime);

            animator.SetTrigger("Push");
            rgPlayingAnimations = true;
            yield return new WaitForSeconds(4.208f); //Fixed animations durations

            //Rotates smoothly towards player
            float time = 0f;
            while (time < 1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, time);
                time += Time.deltaTime * afterPushLookSpeed;
                yield return null;
            }
            transform.rotation = currentFacePlayerRot;

            //Returns to its original look rotation state and waits before finalizing attack
            rgGuardActive = false;
            rgPlayingAnimations = false;
            yield return new WaitForSeconds(rgAfterPushTime);

            isExecutingAttack = false;

            StartCoroutine(RoyalGuardCooldown());
            yield return null;
        }

        private IEnumerator RoyalGuardCooldown()
        {
            yield return new WaitForSeconds(rgCooldownTime);

            rgCooldownActive = false;
            yield return null;
        }

        #endregion ATTACK ABILITY
    }
}

