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

    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Health))]
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        [SerializeField] public State currentState;
        [SerializeField] private bool attackCooldown;

        #region NAVIGATION

        [Header("--- ENEMY BASE SETTINGS ---\n")]
        public NavMeshAgent agent;

        [Header("NAVIGATION SETTINGS")]
        [SerializeField] public float walkSpeed = 1.5f;
        [SerializeField] public float runSpeed = 2.5f;

        [SerializeField] public float stoppingDistance;

        public Vector3 waitingPos;

        public Vector3 attackPos;

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
        [SerializeField] protected int timeBeforeAttack;

        protected bool isExecutingAttack;
        protected bool isAttacking;


        #endregion ATTACK

        #region ROTATION

        public bool ableToFace = false;

        protected Quaternion currentFacePlayerRot;

        private float lastYRotation;

        #endregion ROTATION

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
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();

            player = Player.Instance.gameObject;
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

        private EnemyArea enemyArea;

        private FiniteStateMachine fsm = new FiniteStateMachine();

        private IdleState idleState;
        private ChaseState chaseState;
        private OnQueueState onQueueState;
        private AttackState attackState;

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

        #endregion FSM

        protected virtual void Update()
        {
            fsm.Update();
            Rotation_Update();
            Animator_Update();
        }

        #region ON DAMAGE AND DESTROY

        private void OnDestroy()
        {
            if (enemyArea == null) return;

            switch (currentState)
            {
                case State.Attack:
                    {
                        RemoveFromAttackList();
                        break;
                    }
            }

            enemyArea.RemoveEnemyFromArea(this);
        }

        public void OnDamage(float damage)
        {
            GetComponent<Health>().TakeDamage(damage);

            switch (currentState)
            {
                case State.Attack:
                    {
                        attackState.ResetTimer();
                        break;
                    }
            }
        }

        #endregion ON DAMAGE AND DESTROY

        #region ENEMY AREA

        public void AssignArea(EnemyArea area)
        {
            enemyArea = area;
        }

        public bool AttackPositionsAssigned => enemyArea.UsedAttackPosCount == 3;
        [SerializeField] public bool UpdatedPosition = false;

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
            if (attackCooldown) return;

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
            attackCooldown = true;

            Invoke(nameof(DisableAttackCooldown), 6f);
        }

        private void DisableAttackCooldown()
        {
            attackCooldown = false;
        }

        #endregion ENEMY AREA

        #region ATTACK

        public bool attacked = false;

        public void RunAttackCooldown()
        {
            Invoke(nameof(DisableCooldown), 5f);
        }

        private void DisableCooldown()
        {
            attacked = false;
        }

        public void TriggerAttack()
        {
            isExecutingAttack = true;
            Invoke(nameof(Attack), timeBeforeAttack);
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

        private void Rotation_Update()
        {
            GetCurrentPlayerRot();
            LookAtPlayer();
        }

        protected virtual void LookAtPlayer()
        {
            if (ableToFace)
            {
                transform.rotation = currentFacePlayerRot;
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
            float time = 0f;
            while (time < 1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, time);
                time += Time.deltaTime * 0.1f;
                yield return null;
            }

            transform.rotation = currentFacePlayerRot;
            ableToFace = true;

            yield break;
        }

        #endregion ROTATION

        #region ANIMATOR

        private void Animator_Update()
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
        }

        public void AnimEvent_StopFacingAtPlayer()
        {
            ableToFace = false;
        }

        public void AnimEvent_SmoothResetRotation()
        {
            StartCoroutine(CR_SmoothResetRotation());
        }

        #endregion ANIMATION EVENT METHDOS

        #endregion ANIMATOR

    }
}
