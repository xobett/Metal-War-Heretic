using System.Collections;
using UnityEngine;

public class ShieldEnemy : EnemyBase
{
    //SHIELD ENEMY

    //WAIT 3 SECONDS BEFORE ATTACK
    //SMACKS SHIELDS ONTO THE FORWARD GROUND DEALING DAMAGE
    //STAYS COVERED WHILE ROTATING SLOW TO THE PLAYER
    //AFTER A PERIOD OF TIME
    //MOVE BACK FOR QUARTER OF A SECOND
    //LEAN FORWARD AT A FAST SPEED PUSHING THE PLAYER
    //RETURN TO ITS ORIGINAL POSITION
    //WAIT 2 SECONDS BEFORE DISENGAGING GUARD BEHAVIOUR
    //BEGIN COOLDOWN
    //FOLLOW PLAYER AGAIN

    [Header("--- SHIELD ENEMY SETTINGS --- \n")]

    [Header("ROYAL GUARD TIME SETTINGS")]
    [SerializeField, Range(1f, 10f)] private int rgBeforeSmackTime;
    [SerializeField, Range(1f, 10f)] private int rgGuardingTime;
    [SerializeField, Range(1f, 10f)] private int rgBeforePushTime;
    [SerializeField, Range(1f, 10f)] private int rgAfterPushTime;
    [SerializeField, Range(5f, 10f)] private int rgCooldownTime;

    [Header("ROYAL GUARD DAMAGE SETTINGS")]
    [SerializeField] private float rgDamage;
    [SerializeField] private float rgSmackRadius;

    [SerializeField] private GameObject rgShields;

    private bool rgIsCoolingDown;
    private bool isGuarding;
    private bool isPushing;

    #region BASE OVERRIDES

    protected override void Update()
    {
        base.Update();
        LookSlowAtPlayer();
    }

    #region BASE ATTACK

    protected override void Attack()
    {
        GetAbility();
    }

    #endregion BASE ATTACK

    #region BASE MOVEMENT AND ROTATION

    protected override void LookAtPlayer()
    {
        if (!isGuarding)
        {
            base.LookAtPlayer(); 
        }
    }

    #endregion BASE MOVEMENT AND ROTATION

    #endregion BASE OVERRIDES

    #region ATTACK ABILITY

    private void LookSlowAtPlayer()
    {
        if (!isPushing && isGuarding)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, 0.28f * Time.deltaTime); 
        }
    }

    private void GetAbility()
    {
        StartCoroutine(CrStartPhaseOneRG());
    }

    private IEnumerator CrStartPhaseOneRG()
    {
        rgIsCoolingDown = true;
        yield return new WaitForSeconds(rgBeforeSmackTime);

        //Damages player if is near
        if (Physics.CheckSphere(transform.position, rgSmackRadius, whatIsPlayer))
        {
            playerCam.CameraShake();
            player.GetComponent<Health>().TakeDamage(rgDamage);
        }

        rgShields.SetActive(true);
        isGuarding = true;
        yield return new WaitForSeconds(rgGuardingTime);

        isPushing = true;
        agent.updateRotation = false;
        yield return new WaitForSeconds(rgBeforePushTime);

        Vector3 startingPos = transform.position;
        Vector3 anticipationPos = transform.position - transform.forward * 5f;
        Vector3 pushPos = transform.position + transform.forward * 5f;

        agent.acceleration = 2000;
        agent.speed = 2;
        agent.destination = anticipationPos;
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => agent.velocity.magnitude == 0);

        //Waits half a second before pushing forward movement
        yield return new WaitForSeconds(1f);

        agent.speed = 30f;
        agent.destination = pushPos;
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => agent.velocity.magnitude == 0);

        yield return new WaitForSeconds(rgAfterPushTime);

        isAttacking = false;
        yield return new WaitForSeconds(rgCooldownTime);

        rgIsCoolingDown = false;
        yield return null;
    }

    /// <summary>
    /// ADD PUSH ONLY EFFECTIVE WHEN PUSHING
    /// ADD SMOTH ROTATION AFTER HITTING
    /// CHECK COLLIDERS AVOIDING DAMAGE
    /// </summary>
    /// <param name="other"></param>

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PushPlayer(rgDamage);
        }
    }

    #endregion ATTACK ABILITY

    #region DEBUG VISUAL GIZMOS

    #endregion DEBUG VISUAL GIZMOS
}
