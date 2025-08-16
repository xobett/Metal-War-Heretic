using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI
{
    public enum State
    {
        Idle,
        Chase,
        OnQueue,
        Attack
    }
    public enum EnemyType
    {
        Hammer,
        Electric,
        Shield
    }

    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Health))]
    public abstract class Enemy : MonoBehaviour, IDamageable
    {
        public State currentState;
        public EnemyType enemyType;

        #region VFX & AUDIO

        [Header("VFX & AUDIO SETTINGS")]
        [SerializeField] private GameObject onDeathVfx;
        protected AudioSource audioSource;

        #endregion VFX & AUDIO

        #region NAVIGATION

        [Header("--- ENEMY BASE SETTINGS ---\n")]
        internal NavMeshAgent agent;

        [Header("NAVIGATION SETTINGS")]
        [SerializeField] public float walkSpeed = 1.5f;
        [SerializeField] public float runSpeed = 2.5f;

        [SerializeField] public float stoppingDistance;
        [SerializeField] public float priority;

        internal Vector3 waitingPos;
        internal Vector3 attackPos;

        internal bool UpdatedPosition = false;

        public bool AttackPositionsAssigned => enemyArea.UsedAttackPosCount == 3;

        #endregion NAVIGATION

        #region ANIMATOR

        [Header("ENEMY ANIMATOR SETTINGS")]
        protected Animator animator;

        #endregion ANIMATOR

        #region PLAYER REFERENCES

        [HideInInspector] public GameObject player;

        #endregion PLAYER REFERENCES

        #region ATTACK

        [Header("ATTACK SETTINGS")]
        [SerializeField] protected float damage;
        protected int timeBeforeAttack = 5;

        internal bool isExecutingAttack;
        internal bool forcedAttackState;

        public bool attackCooldown = false;
        private bool attackStateCooldown = false;

        [SerializeField] private int score;

        #endregion ATTACK

        #region ROTATION

        public bool ableToFace = false;

        protected Quaternion currentFacePlayerRot;

        private float lastYRotation;

        #endregion ROTATION

        #region ENEMY AREA

        private EnemyArea enemyArea;
        [SerializeField] private int respawnTime;

        #endregion ENEMY AREA

        #region FSM

        private FiniteStateMachine fsm = new FiniteStateMachine();

        private IdleState idleState;
        private ChaseState chaseState;
        private OnQueueState onQueueState;
        private AttackState attackState;

        #endregion FSM

        protected virtual void Start()
        {
            Start_GetReferences();
            Start_CreateFSMStates();
            Start_InitializeFSM();
            lastYRotation = transform.rotation.eulerAngles.y;
        }

        #region START

        private void Start_GetReferences()
        {
            player = Player.Instance.gameObject;

            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            audioSource = GetComponentInChildren<AudioSource>();

            audioSource.outputAudioMixerGroup = AudioManager.Instance.sfxMixerGroup;

        }

        private void Start_CreateFSMStates()
        {
            idleState = new IdleState(this);
            chaseState = new ChaseState(this);
            onQueueState = new OnQueueState(this);
            attackState = new AttackState(this);
        }

        private void Start_InitializeFSM()
        {
            fsm.Initialize(idleState);
        }

        #endregion START

        #region FSM

        public void ChangeState(State newState)
        {
            switch (newState)
            {
                case State.Idle: fsm.ChangeState(idleState); break;
                case State.Chase: fsm.ChangeState(chaseState); break;
                case State.OnQueue: fsm.ChangeState(onQueueState); break;
                case State.Attack: fsm.ChangeState(attackState); break;
            }
        }

        public void OnRespawn()
        {
            Invoke(nameof(OnRespawnChangeToQueue), 1f);
        }

        private void OnRespawnChangeToQueue()
        {
            ChangeState(State.OnQueue);
        }

        #endregion FSM

        protected virtual void Update()
        {
            if (enemyArea.inArea)
            {
                fsm.Update();
            }
            Rotation_Update();
            Animator_Update();
        }

        #region ON DAMAGE AND DESTROY

        public virtual void OnDamage(float damage)
        {
            GetComponent<Health>().TakeDamage(damage);

            transform.rotation = currentFacePlayerRot;

            switch (currentState)
            {
                case State.Attack:
                    {
                        attackState.ResetTimer();
                        break;
                    }
                default:
                    {
                        forcedAttackState = true;
                        fsm.ChangeState(attackState);
                        enemyArea.OnForced_AddAttackingEnemy(this);
                        break;
                    }
            }
        }

        public void OnDeath()
        {
            GameManager.Instance.IncreaseScore(score);

            enemyArea.RemoveEnemyFromArea(this);
            enemyArea.RespawnEnemy(enemyType, respawnTime);
        }

        #endregion ON DAMAGE AND DESTROY

        #region ENEMY AREA

        public void AssignArea(EnemyArea area)
        {
            enemyArea = area;
        }

        public void OnForced_TransitionToQueue()
        {
            fsm.ChangeState(onQueueState);
        }

        //Used to reassign queue positions only when attacking enemies are 3
        public void ResetUpdatePositionValue()
        {
            enemyArea.ResetUpdatePositionValue();
        }

        public void QueryWaitPosition()
        {
            enemyArea.QueryWaitPosition(this);
        }

        public void SetWaitPosition(Vector3 assignedPosition)
        {
            waitingPos = assignedPosition;
        }

        public void RemoveWaitPos()
        {
            enemyArea.RemoveWaitPos(this);
        }

        public void QueryAttack()
        {
            if (attackStateCooldown) return;

            enemyArea.QueryAttackState(this);
        }

        public void QueryAttackPosition()
        {
            enemyArea.QueryAttackPos(this);
        }

        public void SetAttackPosition(Vector3 position)
        {
            attackPos = position;
        }

        public void RemoveAttackPos()
        {
            enemyArea.RemoveAttackPos(this);
        }

        public void RemoveFromAttackList()
        {
            enemyArea.RemoveAttackingEnemy(this);
        }

        public void RunAttackStateCooldown()
        {
            attackStateCooldown = true;

            Invoke(nameof(DisableAttackStateCooldown), 6f);
        }

        private void DisableAttackStateCooldown()
        {
            attackStateCooldown = false;
        }

        #endregion ENEMY AREA

        #region ATTACK

        public void RunAttackCooldown()
        {
            Invoke(nameof(DisableCooldown), timeBeforeAttack);
        }

        private void DisableCooldown()
        {
            attackCooldown = false;
        }

        public void ExecuteAttack()
        {
            attackCooldown = true;
            isExecutingAttack = true;

            Attack();
        }

        public abstract void Attack();

        public virtual void HitPlayer(Collider playerCollider)
        {
            player.GetComponent<Health>().TakeDamage(damage);
        }

        public void PushPlayer(float damageUponHit)
        {
            player.GetComponent<Health>().TakeDamage(damageUponHit);

            float randomSideValue = Random.Range(-2f, 2f);
            float randomTimePushed = Random.Range(0.7f, 0.9f);
            float randomPushedForce = Random.Range(10f, 13f);

            Vector3 pushedDirection = transform.forward + transform.right * randomSideValue;

            player.GetComponent<PlayerMovement>().SetHitMovement(pushedDirection, randomPushedForce, randomTimePushed);
        }

        #endregion ATTACK

        #region ROTATION

        protected void Rotation_Update()
        {
            GetCurrentPlayerRot();
            LookAtPlayer();
        }

        protected virtual void LookAtPlayer()
        {
            if (ableToFace)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, 0.3f);
            }
        }

        private void GetCurrentPlayerRot()
        {
            Vector3 lookDirection = player.transform.position - transform.position;
            Quaternion lookTarget = Quaternion.LookRotation(lookDirection);
            currentFacePlayerRot = Quaternion.Euler(0, lookTarget.eulerAngles.y, 0);
        }

        public void SmoothResetRotation()
        {
            StartCoroutine(CR_SmoothResetRotation());
        }

        private IEnumerator CR_SmoothResetRotation()
        {
            float angle = Quaternion.Angle(transform.rotation, currentFacePlayerRot);
            float remainingAngle = angle;

            while (remainingAngle > 110f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, 1 - (remainingAngle / angle));
                remainingAngle -= 55f * Time.deltaTime;
                yield return null;
            }

            transform.rotation = currentFacePlayerRot;
            ableToFace = true;

            Debug.Log("Reset rotation");

            yield break;
        }

        #endregion ROTATION

        #region ANIMATOR

        protected void Animator_Update()
        {
            SetWalkAnimation();
            SetRotationAnimation();
        }

        protected virtual void SetWalkAnimation()
        {
            if (agent.velocity.magnitude != 0)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }

        protected void SetRotationAnimation()
        {
            int roundedValue = 0;

            if (agent.velocity.magnitude != 0)
            {
                animator.SetInteger("yRotation", 0);
            }
            else
            {
                float currentYRotation = transform.eulerAngles.y;
                float deltaAngle = Mathf.DeltaAngle(lastYRotation, currentYRotation);

                if (Mathf.Abs(deltaAngle) > 0.1f)
                {
                    roundedValue = deltaAngle > 0 ? 1 : deltaAngle < 0 ? -1 : 0;
                }

                lastYRotation = transform.eulerAngles.y;

                animator.SetInteger("yRotation", roundedValue);
            }
        }

        #region ANIMATION EVENT METHDOS

        public void AnimEvent_FinishAttack()
        {
            isExecutingAttack = false;
            RunAttackCooldown();
        }

        public void AnimEvent_StopFacingAtPlayer()
        {
            ableToFace = false;
        }

        public void AnimEvent_SmoothResetRotation()
        {
            ableToFace = true;
        }

        public void AnimEvent_OnDeathStart()
        {
            gameObject.tag = "Dead";

            GameObject vfx = Instantiate(onDeathVfx, transform.position, onDeathVfx.transform.rotation);
            Destroy(vfx, 3);
        }

        public void AnimEvent_OnDeath()
        {
            Destroy(gameObject);
        }

        #endregion ANIMATION EVENT METHDOS

        #endregion ANIMATOR
    }
}
