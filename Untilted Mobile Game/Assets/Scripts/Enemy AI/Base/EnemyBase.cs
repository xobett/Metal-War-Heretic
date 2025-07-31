using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Health))]
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        #region ENEMY BASE

        private EnemyArea enemyArea;
        [SerializeField] private EnemyState enemyState;

        #endregion ENEMY BASE

        #region NAVIGATION

        [Header("--- ENEMY BASE SETTINGS ---\n")]
        public NavMeshAgent agent;

        [Header("NAVIGATION SETTINGS")]
        [SerializeField] public float walkSpeed = 1.5f;
        [SerializeField] public float runSpeed = 2.5f;

        [SerializeField] public float stoppingDistance;

        public LayerMask whatIsEnemy;

        public Vector3 waitingPos;

        #endregion NAVIGATION

        #region ANIMATOR

        [Header("ENEMY ANIMATOR SETTINGS")]
        protected Animator animator;

        #endregion ANIMATOR

        #region PLAYER AND CAMERA REFERENCES

        [Header("PLAYER REFERENCES")]
        [SerializeField] protected LayerMask whatIsPlayer;
        [SerializeField] private float playerDetectionRadius;
        public GameObject player;

        #endregion PLAYER AND CAMERA REFERENCES

        #region ATTACK

        [Header("ATTACK SETTINGS")]
        [SerializeField] protected float damage;
        [SerializeField] protected int timeBeforeAttack;

        protected bool isExecutingAttack;
        protected bool isAttacking;

        #endregion ATTACK

        #region ROTATION

        protected Quaternion currentFacePlayerRot;

        private float lastYRotation;

        [SerializeField] protected bool ableToFace = true;

        #endregion ROTATION

        private FiniteStateMachine fsm = new FiniteStateMachine();

        protected void Awake()
        {
            Awake_GetReferences();

            fsm.Initialize(new IdleState(this));
        }

        #region AWAKE

        private void Awake_GetReferences()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();

            player = GameObject.FindGameObjectWithTag("Player");
        }

        #endregion AWAKE

        protected virtual void Start()
        {
            whatIsEnemy = LayerMask.GetMask("Enemy");
            lastYRotation = transform.rotation.eulerAngles.y;

            QueryWaitPosition();
        }

        private void ChangeState()
        {
            fsm.ChangeState(new ChaseState(this));
        }

        protected virtual void Update()
        {
            fsm.Update();
        }

        #region ON DAMAGE AND DESTROY

        private void OnDestroy()
        {
            if (enemyArea != null)
            {
                if (isAttacking)
                {
                    enemyArea.RemoveAttackingEnemy(this);
                }

                enemyArea.RemoveEnemyFromArea(this);
            }
            else
            {
                Debug.LogWarning($"{name} does not have an Enemy Area assigned!");
            }
        }

        public void OnDamage(float damage)
        {
            GetComponent<Health>().TakeDamage(damage);
        }

        #endregion ON DAMAGE AND DESTROY

        #region ENEMY AREA

        public void AssignArea(EnemyArea area)
        {
            enemyArea = area;
        }

        #endregion ENEMY AREA

        #region ATTACK

        public void TriggerAttack()
        {
            isExecutingAttack = true;
            Invoke(nameof(Attack), timeBeforeAttack);
        }

        protected abstract void Attack();

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

        #region AI NAVIGATION
        public void QueryWaitPosition()
        {
            enemyArea.QueryWaitPosition(this);
        }

        public void SetWaitPosition(Vector3 waitPosition)
        {
            waitingPos = waitPosition;
            fsm.ChangeState(new ChaseState(this));
        }

        #endregion AI NAVIGATION

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
