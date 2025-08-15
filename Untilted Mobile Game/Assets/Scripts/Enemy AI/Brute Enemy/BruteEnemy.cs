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

        private LayerMask whatIsPlayer;

        [Header("TIMER SETTINGS")]
        [SerializeField, Range(2f, 6f)] private int rmpRunningTime;
        [SerializeField, Range(10f, 15f)] private int rmpCooldownTime;

        private bool rmpIsRunning;
        [SerializeField] private bool rmpCoolingDown;

        protected override void Start()
        {
            base.Start();
            agent.stoppingDistance = stoppingDistance;
            whatIsPlayer = LayerMask.GetMask("Player");
            RampageRun_Start();
        }

        protected override void Update()
        {
            MoveTowardsPlayer();
            FollowPlayer_Update();

            Animator_Update();
            Rotation_Update();
            RampageRun_Update();
        }

        private void MoveTowardsPlayer()
        {
            if (agent.remainingDistance <= stoppingDistance && Physics.CheckSphere(transform.position, 1.5f, whatIsPlayer))
            {
                if (isExecutingAttack) return;
                isExecutingAttack = true;
                ExecuteHeavyPunch();
            }
        }

        private void FollowPlayer_Update()
        {
            if (!isExecutingAttack)
            {
                agent.destination = player.transform.position;
            }
        }

        public override void Attack()
        {
            ExecuteHeavyPunch();
        }

        public override void OnDamage(float damage)
        {
            GetComponent<Health>().TakeDamage(damage);
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
            SetRunningMovement();
            RampageRunTriggerCheck();
            GetDistanceFromPlayer();
        }

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
            if (other.CompareTag("Player") && (rmpIsRunning && !rmpPlayerHit))
            {
                rmpPlayerHit = true;

                PushPlayer(rmpDamage);
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

        public void AnimEvent_PlayStepSound()
        {
            audioSrc.clip = AudioManager.Instance.GetClip("BRUTO PISADAS");
            audioSrc.Play();
        }

        public void AnimEvent_PlayRunStepSound()
        {
            audioSrc.clip = AudioManager.Instance.GetClip("BRUTO CORRER");
            audioSrc.Play();
        }

        #endregion ANIMATION EVENT METHODS
    }
}

