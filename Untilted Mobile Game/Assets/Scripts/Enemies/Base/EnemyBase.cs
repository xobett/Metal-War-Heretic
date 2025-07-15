using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Health))]
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        #region AI NAVIGATION

        [Header("--- ENEMY BASE SETTINGS ---\n")]
        protected NavMeshAgent agent;

        [Header("AI NAVIGATION SETTINGS")]
        [SerializeField] protected float walkSpeed = 1.5f;
        [SerializeField] protected float stoppingDistance;

        [SerializeField] private LayerMask whatAreObstacles;
        private Vector3 nextPos;

        private bool isMoving;

        private float navigationTimer;
        private float maxNavigationTime = 12f;

        private Coroutine navigationActiveCoroutine;

        #endregion AI NAVIGATION

        #region ANIMATOR

        [Header("ENEMY ANIMATOR SETTINGS")]
        [SerializeField] protected Animator animator;

        #endregion ANIMATOR

        #region PLAYER AND CAMERA REFERENCES

        [Header("PLAYER REFERENCES")]
        [SerializeField] protected LayerMask whatIsPlayer;
        [SerializeField] private float playerDetectionRadius;
        protected GameObject player;

        protected PlayerCamera playerCam;

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

        protected bool ableToFace;

        #endregion ROTATION

        protected void Awake()
        {
            GetReferences();
            SetEnemySettings();
        }

        protected virtual void Start()
        {
            isAttacking = true;
            lastYRotation = transform.rotation.eulerAngles.y;
        }

        protected virtual void Update()
        {
            Animator_SetAnimations();
            Rotation_SetLookRotation();
            Attack_SetTriggerCheck();
            AINavigation_SetNavigation();
        }

        #region BEHAVIOUR CHECK

        private void GetBehaviour()
        {
            if (!isMoving && EnemyManager.instance.ActiveAttackingEnemiesCount < 3 && !isAttacking)
            {
                EnemyManager.instance.AddAttackingEnemy(this);
                SetAttackBehaviourSettings();
                isAttacking = true;
            }
        }

        #endregion BEHAVIOUR CHECK

        #region ON DAMAGE AND DESTROY

        public void OnDamage(float damage)
        {
            GetComponent<Health>().TakeDamage(damage);
        }

        private void OnDestroy()
        {
            if (isAttacking && EnemyManager.instance != null)
            {
                EnemyManager.instance.RemoveAttackingEnemy(this);
            }
        }

        #endregion ON DAMAGE AND DESTROY

        #region ATTACK

        private void Attack_SetTriggerCheck()
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

        protected virtual void Attack()
        {
            //Default behaviour for melee attack type enemies
            if (Physics.Raycast(transform.position, transform.forward * playerDetection_Range, playerDetection_Range, whatIsPlayer))
            {
                playerCam.CameraShake();
                player.GetComponent<Health>().TakeDamage(damage);
            }

            isExecutingAttack = false;
        }

        public virtual void HitPlayer(Collider playerCollider)
        {
            playerCam.CameraShake();
            player.GetComponent<Health>().TakeDamage(damage);
        }

        public void PushPlayer(float damageUponHit)
        {
            //playerCam.CameraShake();

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

        private void Rotation_SetLookRotation()
        {
            GetCurrentPlayerRot();
            LookAtPlayer();
        }

        protected virtual void LookAtPlayer()
        {
            if (!isMoving || ableToFace)
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

        protected void SmoothResetRotation()
        {
            StartCoroutine(CR_SmoothResetRotation());
        }

        private IEnumerator CR_SmoothResetRotation()
        {
            float time = 0f;
            while (time < 1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, time);
                time += Time.deltaTime * 0.6f;
                yield return null;
            }

            transform.rotation = currentFacePlayerRot;
            ableToFace = true;

            yield break;
        }

        #endregion ROTATION

        #region AI NAVIGATION

        private void AINavigation_SetNavigation()
        {
            FollowPlayer();
        }

        protected virtual void FollowPlayer()
        {
            if (!isExecutingAttack && !isMoving)
            {
                agent.destination = player.transform.position;
            }
        }
        
        private void RunNavigationTimer()
        {
            navigationTimer -= Time.deltaTime;
        }

        private IEnumerator CR_AssignWaitPosition()
        {
            navigationTimer = maxNavigationTime;
            isMoving = true;

            Vector3 calculatedPos = EnemyManager.instance.AssignMovingPosition();

            int attemptsDone = 0;
            while (Physics.CheckSphere(calculatedPos, agent.radius, whatAreObstacles))
            {
                if (attemptsDone >= 2)
                {
                    EnemyManager.instance.IncreaseAreaRadius();
                }

                calculatedPos = EnemyManager.instance.AssignMovingPosition();
                attemptsDone++;
                Debug.Log("New position was generated");
                yield return null;
            }

            nextPos = calculatedPos;
            agent.destination = nextPos;
            //Waits until remaining distance is called and until enemy arrives to its destination
            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

            float time = 0f;
            while (time < 1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, time);
                time += Time.deltaTime * 0.6f;
                yield return null;
            }
            transform.rotation = currentFacePlayerRot;

            //Stops the enemy upon arrival
            agent.speed = 0;
            isMoving = false;
            Debug.Log("Set to false");

            GetBehaviour();

            yield break;
        }

        private void HandleStuckNavigation()
        {
            if (navigationTimer < 0 && isMoving)
            {
                StopCoroutine(navigationActiveCoroutine);
                agent.speed += 1.5f;
                navigationActiveCoroutine = StartCoroutine(CR_AssignWaitPosition());
            }
        }

        private void HandleEnemyExitingArea()
        {
            if (!isMoving && Vector3.Distance(transform.position, player.transform.position) > EnemyManager.instance.AreaRadius)
            {
                agent.speed = walkSpeed * 2;
                StartCoroutine(CR_AssignWaitPosition());
            }
        }

        #endregion AI NAVIGATION

        #region ANIMATOR

        private void Animator_SetAnimations()
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

        public void AnimEvent_FaceAtPlayer()
        {
            ableToFace = true;
        }

        #endregion ANIMATION EVENT METHDOS

        #endregion ANIMATOR

        #region START

        private void GetReferences()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerCam = Camera.main.GetComponent<PlayerCamera>();
            agent = GetComponent<NavMeshAgent>();
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
