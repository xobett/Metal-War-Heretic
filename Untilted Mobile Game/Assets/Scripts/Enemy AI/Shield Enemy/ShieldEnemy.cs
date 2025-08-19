using UnityEngine;

namespace EnemyAI.ShieldEnemy
{
    public class ShieldEnemy : Enemy
    {
        [Header("--- SHIELD ENEMY SETTINGS --- \n")]

        [Header("ROYAL GUARD\n")]

        [Header("COLLIDERS")]
        [SerializeField] private Collider[] colliders;
        private bool ableToPush;

        [Header("TIMER SETTINGS")]
        [SerializeField, Range(1f, 10f)] private int rgGuardingTime;
        [SerializeField, Range(5f, 10f)] private int rgCooldownTime;

        private bool rgGuardActive;
        private bool rgCooldownActive;

        [Header("GUARDING ROTATION SPEED")]
        [SerializeField] private float guardLookSpeed = 0.8f;

        protected override void Start()
        {
            base.Start();
            agent.stoppingDistance = stoppingDistance;
        }

        protected override void Update()
        {
            SelfBehavior_Update();

            Animator_Update();
            Rotation_Update();
            RoyalGuard_Update();
        }

        public override void Attack()
        {
            if (!rgCooldownActive)
            {
                ExecuteRoyalGuard();
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

        #region ROYAL GUARD

        private void RoyalGuard_Update()
        {
            SetSlowRotation();
        }

        private void ExecuteRoyalGuard()
        {
            rgCooldownActive = true;
            animator.SetTrigger("RoyalGuard");
        }

        private void SetSlowRotation()
        {
            if (rgGuardActive)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, guardLookSpeed * Time.deltaTime);
            }
        }

        private void RG_RunCooldown()
        {
            Debug.Log("Called cooldown");
            Invoke(nameof(RG_DisableCooldown), rgCooldownTime);
        }

        private void RG_DisableCooldown()
        {
            rgCooldownActive = false;
        }

        #endregion ROYAL GUARD

        #region ANIMATION EVENT METHODS

        public void AnimEvent_PlayStepSound()
        {
            audioSource.clip = AudioManager.Instance.GetClip("CAMINATA ESCUDOS");
            audioSource.Play();
        }

        public void AnimEvent_PlayShieldsStompSound()
        {
            audioSource.clip = AudioManager.Instance.GetClip("ESCUDOS ATAQUE");
            audioSource.Play();
        }

        public void AnimEvent_PlayPushShieldsSound()
        {
            audioSource.clip = AudioManager.Instance.GetClip("ESCUDOS ATAQUE 2");
            audioSource.Play();
        }

        public void AnimEvent_EnablePush()
        {
            foreach (Collider collider in colliders)
            {
                collider.isTrigger = true;
            }

            ableToPush = true;
        }

        public void AnimEvent_DisablePush()
        {
            foreach (Collider collider in colliders)
            {
                collider.isTrigger = false;
            }

            ableToPush = false;
        }

        public void AnimEvent_RunGuardTime()
        {
            Invoke(nameof(PushForward), rgGuardingTime);
            rgGuardActive = true;
        }

        private void PushForward()
        {
            rgGuardActive = false;
            animator.SetTrigger("Push");
        }

        public void AnimEvent_RunCooldown()
        {
            RG_RunCooldown();
        }

        #endregion ANIMATION EVENT METHODS
    }
}

