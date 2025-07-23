using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI
{
    public enum EnemyState
    {
        Idle,
        Attack
    }

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
        protected NavMeshAgent agent;

        [Header("NAVIGATION SETTINGS")]
        [SerializeField] protected float walkSpeed = 1.5f;
        [SerializeField] protected float stoppingDistance;

        #endregion NAVIGATION

        #region ANIMATOR

        [Header("ENEMY ANIMATOR SETTINGS")]
        protected Animator animator;

        #endregion ANIMATOR

        #region PLAYER AND CAMERA REFERENCES

        [Header("PLAYER REFERENCES")]
        [SerializeField] protected LayerMask whatIsPlayer;
        [SerializeField] private float playerDetectionRadius;
        protected GameObject player;

        protected CameraFollow playerCam;

        #endregion PLAYER AND CAMERA REFERENCES

        #region ATTACK

        [Header("ATTACK SETTINGS")]
        [SerializeField] protected float damage;
        [SerializeField] protected int timeBeforeAttack;

        protected bool isExecutingAttack;
        protected bool isAttacking;

        private const float playerDetection_Range = 3f;

        #endregion ATTACK

        #region ROTATION

        protected Quaternion currentFacePlayerRot;

        private float lastYRotation;

        [SerializeField] protected bool ableToFace = true;

        #endregion ROTATION

        protected void Awake()
        {
            GetReferences();
            SetEnemySettings();
        }

        protected virtual void Start()
        {
            lastYRotation = transform.rotation.eulerAngles.y;
            enemyState = EnemyState.Idle;
        }

        protected virtual void Update()
        {
            BehaviorCheck_Update();

        }

        #region BEHAVIOR CHECK

        private void BehaviorCheck_Update()
        {
            if (enemyState == EnemyState.Attack)
            {
                Rotation_Update();
                Attack_Update();
                Navigation_Update();
            }

            Animator_Update();
        }

        public void GetBehavior()
        {
            if (enemyArea.GetTotalAttackingEnemies() < 4)
            {
                enemyArea.AddAttackingEnemy(this);
                enemyState = EnemyState.Attack;
                isAttacking = true;
            }
            else
            {
                NavigateToRandomPoint();
                Debug.Log("Too much enemies were attacking, will check again in 10 seconds");
                Invoke(nameof(GetBehavior), 10f);
            }
        }

        #endregion BEHAVIOR CHECK

        #region ON DAMAGE AND DESTROY

        public void OnDamage(float damage)
        {
            GetComponent<Health>().TakeDamage(damage);
            agent.speed = 0;
            StartCoroutine(CR_SmoothResetRotation());
            Invoke(nameof(RegainMovement), 5f);
        }

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

        private void RegainMovement()
        {
            agent.speed = walkSpeed;
        }

        #endregion ON DAMAGE AND DESTROY

        #region ENEMY AREA

        public void AssignArea(EnemyArea area)
        {
            enemyArea = area;
        }

        #endregion ENEMY ARAE

        #region ATTACK

        private void Attack_Update()
        {
            AttackTriggerCheck();
        }

        private void AttackTriggerCheck()
        {
            if (isAttacking && agent.remainingDistance <= stoppingDistance && CheckPlayerIsNear() && !isExecutingAttack)
            {
                TriggerAttack();
            }
        }

        private void TriggerAttack()
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

        private void SetAttackBehaviourSettings()
        {
            agent.speed = walkSpeed;
            agent.stoppingDistance = stoppingDistance;
        }

        private void SetNonAttackBehaviourSettings()
        {
            agent.speed = walkSpeed * 2;
            agent.stoppingDistance = 0;
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

        private void Navigation_Update()
        {
            FollowPlayer();
        }

        private void NavigateToRandomPoint()
        {
            agent.destination = GetPosAroundPlayer();
        }

        private Vector3 GetPosAroundPlayer()
        {
            float randomAngle = Random.Range(0f, Mathf.PI * 2);
            float radius = Random.Range(2f, 5f);

            float offsetX = Mathf.Cos(randomAngle) * radius;
            float offsetZ = Mathf.Sin(randomAngle) * radius;

            Vector3 randomPos = new Vector3(player.transform.position.x + offsetX, transform.position.y, player.transform.position.z + offsetZ);

            return randomPos;
        }

        protected virtual void FollowPlayer()
        {
            if (!isExecutingAttack)
            {
                agent.destination = player.transform.position;
            }
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

        #region START

        private void GetReferences()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();

            player = GameObject.FindGameObjectWithTag("Player");
            playerCam = Camera.main.GetComponent<CameraFollow>();
        }

        private void SetEnemySettings()
        {
            agent.speed = walkSpeed;
            agent.stoppingDistance = stoppingDistance;
        }

        #endregion START

        #region PLAYER DETECTION
        private bool CheckPlayerIsNear()
        {
            return Physics.CheckSphere(transform.position, playerDetectionRadius, whatIsPlayer);
        }

        #endregion PLAYER DETECTION 
    }
}
