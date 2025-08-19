using UnityEngine;

namespace EnemyAI.BruteEnemy
{
    public class BruteEnemy : Enemy
    {
        [Header("--- BRUTE ENEMY SETTINGS ---\n")]


        [Header("RAMPAGE RUN\n")]

        [Header("RUN SPEED AND DAMAGE SETTINGS")]
        [SerializeField] private float rmpDamage;
        [SerializeField] private int rmpSpeed;

        private bool rmpPlayerHit;

        [SerializeField] private GameObject runVfx;

        private int rmpTriggerDistance;
        private float playerDistance;

        private float viewAngle;

        [Header("TIMER SETTINGS")]
        [SerializeField, Range(2f, 6f)] private int rmpRunningTime;
        [SerializeField, Range(10f, 15f)] private int rmpCooldownTime;

        private bool rmpIsRunning;
        [SerializeField] private bool rmpCoolingDown;

        protected override void Start()
        {
            base.Start();
            RampageRun_Start();
        }

        protected override void Update()
        {
            SelfBehavior_Update();

            Animator_Update();
            Rotation_Update();
            RampageRun_Update();
        }

        public override void Attack()
        {
            ExecuteHeavyPunch();
        }

        #region HEAVY PUNCH
        private void ExecuteHeavyPunch()
        {
            animator.SetTrigger("HeavyPunch");
        }

        #endregion HEAVY PUNCH

        #region RAMPAGE RUN

        private void RampageRun_Start()
        {
            rmpCoolingDown = true;
            RMPRunCooldown();
        }

        private void RampageRun_Update()
        {
            if (!enemyArea.playerIsOnArea) return;

            SetRunningMovement();
            RampageRunTriggerCheck();
            GetDistanceFromPlayer();
        }

        //private void GetViewAngle()
        //{
        //    viewAngle = Quaternion.Angle()
        //}

        private void GetDistanceFromPlayer()
        {
            playerDistance = Vector3.Distance(transform.position, player.transform.position);
        }

        private void RampageRunTriggerCheck()
        {
            if (!rmpCoolingDown)
            {
                if (!isExecutingAttack && playerDistance > rmpTriggerDistance)
                {
                    ExecuteRampageRun();
                }
            }
        }

        private void ExecuteRampageRun()
        {
            isExecutingAttack = true;
            rmpCoolingDown = true;

            animator.SetBool("isRunning", true);
            animator.SetTrigger("RampageRun");
        }

        private void SetRunningMovement()
        {
            if (rmpIsRunning)
            {
                agent.destination = agent.transform.position + agent.transform.forward * 15f;
            }
        }

        // Pushes player when running
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && rmpIsRunning)
            {
                rmpPlayerHit = true;

                PushPlayer(rmpDamage);
                Debug.Log("Hit player");
            }
        }

        private void RMPRunCooldown()
        {
            Invoke(nameof(RMPDisableCooldown), 10);
        }

        private void RMPDisableCooldown()
        {
            rmpCoolingDown = false;
        }

        #endregion RAMPAGE RUN

        #region ANIMATION EVENT METHODS

        public void AnimEvent_PlayStepSound()
        {
            audioSource.clip = AudioManager.Instance.GetClip("BRUTO PISADAS");
            audioSource.Play();
        }

        public void AnimEvent_PlayRunStepSound()
        {
            audioSource.clip = AudioManager.Instance.GetClip("BRUTO CORRER");
            audioSource.Play();
        }

        public void AnimEvent_PlayAnticipationSound()
        {
            audioSource.clip = AudioManager.Instance.GetClip("ANTICIPACION BRUTO");
            audioSource.Play();
        }

        public void AnimEvent_PlayDriftSound()
        {
            AudioManager.Instance.PlaySFX("FRENADO BRUTO");
        }

        public void AnimEvents_EnableRunVfX()
        {
            runVfx.SetActive(true);
        }

        public void AnimEvents_DisableRunVfx()
        {
            runVfx.SetActive(false);
        }

        public void AnimEvent_StopMoving()
        {
            agent.destination = transform.position;
        }

        public void AnimEvent_StartRunning()
        {
            rmpIsRunning = true;
            agent.speed = rmpSpeed;

            Invoke(nameof(SlowDown), rmpRunningTime);
        }

        private void SlowDown()
        {
            animator.SetBool("isRunning", false);

            agent.speed = 1;
        }

        public void AnimEvent_StopAcceleration()
        {
            rmpIsRunning = false;
            agent.destination = transform.position;

            if (rmpPlayerHit)
            {
                rmpPlayerHit = false;
            }
        }

        public void AnimEvent_ResetSpeed()
        {
            Debug.Log($"Cooldown queried");
            agent.speed = walkSpeed;
            RMPRunCooldown();
        }


        #endregion ANIMATION EVENT METHODS
    }
}

