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
        
        private int rmpTriggerDistance;
        private float playerDistance;

        [Header("TIMER SETTINGS")]
        [SerializeField, Range(2f, 6f)] private int rmpRunningTime;
        [SerializeField, Range(10f, 15f)] private int rmpCooldownTime;

        private bool rmpIsRunning; 
        private bool rmpCoolingDown;

        protected override void Start()
        {
            base.Start();
            RampageRun_Start();
        }

        protected override void Update()
        {
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
            RunCooldown();
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
            if (!isExecutingAttack)
            {
                if (!rmpCoolingDown && playerDistance > rmpTriggerDistance)
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

        private void RunCooldown()
        {
            Invoke(nameof(DisableCooldown), rmpCooldownTime);
        }

        private void DisableCooldown()
        {
            rmpCoolingDown = false;
        }

        #endregion RAMPAGE RUN

        #region ANIMATION EVENT METHODS

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
            agent.speed = walkSpeed;
            RunCooldown();
        }

        #endregion ANIMATION EVENT METHODS
    }
}

