using System.Collections;
using UnityEngine;

public class ShieldEnemy : EnemyBase
{
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

    private bool rgCooldownActive;
    private bool rgGuardActive;
    private bool rgPushActive;
    private bool rgisPushing;

    private bool rgPlayerHit = false;

    private Coroutine rgActiveCoroutine;

    #region BASE OVERRIDES

    protected override void Update()
    {
        base.Update();
        LookSlowAtPlayer();
    }

    #region BASE ATTACK

    protected override void Attack()
    {
        if (!rgCooldownActive)
        {
            Debug.Log("Executed royal guard");
            rgActiveCoroutine = StartCoroutine(StartRoyalGuard());
        }
        else
        {
            Debug.Log("Executed normal attack");
            base.Attack();
        }
    }

    #endregion BASE ATTACK

    #region BASE MOVEMENT AND ROTATION

    protected override void LookAtPlayer()
    {
        if (!rgGuardActive)
        {
            base.LookAtPlayer();
        }
    }

    #endregion BASE MOVEMENT AND ROTATION

    #endregion BASE OVERRIDES

    #region ATTACK ABILITY

    private void LookSlowAtPlayer()
    {
        if (!rgPushActive && rgGuardActive)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, 0.28f * Time.deltaTime);
        }
    }

    private IEnumerator StartRoyalGuard()
    {
        //Starts cooldown and time before executing the attack
        rgCooldownActive = true;
        yield return new WaitForSeconds(rgBeforeSmackTime);

        //Stopms ground and activates shields
        //While guarding, enables a condition to slowly rotate towards the player
        if (Physics.CheckSphere(transform.position, rgSmackRadius, whatIsPlayer))
        {
            player.GetComponent<Health>().TakeDamage(rgDamage);
        }
        playerCam.CameraShake();

        rgGuardActive = true;
        yield return new WaitForSeconds(rgGuardingTime);

        //Stops rotating slowly and waits a time before executing its push
        rgPushActive = true;
        agent.updateRotation = false;
        yield return new WaitForSeconds(rgBeforePushTime);

        //Calculates start and end push positions,
        Vector3 startingPos = transform.position;
        Vector3 anticipationPos = transform.position - transform.forward * 5f;
        Vector3 pushPos = transform.position + transform.forward * 15f;

        //Reduces acceleration to avoid acceleration in its push and moves back to push.
        agent.acceleration = 2000;
        agent.speed = 2;
        agent.destination = anticipationPos;
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= stoppingDistance);

        //Waits half a second before pushing forward movement
        yield return new WaitForSeconds(1f);

        //Pushes forward and waits until it has arrives to its end push position
        rgisPushing = true;

        agent.speed = 30f;
        agent.destination = pushPos;
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= stoppingDistance);

        rgisPushing = false;

        //Rotates smoothly towards player
        float time = 0f;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, time);
            time += Time.deltaTime * 0.6f;
            yield return null;
        }
        transform.rotation = currentFacePlayerRot;

        //Returns to its original look rotation state and waits before finalizing attack
        rgGuardActive = false;
        rgPushActive = false;
        yield return new WaitForSeconds(rgAfterPushTime);

        agent.speed = walkSpeed;
        agent.acceleration = 12f; //Resets to its default value
        isExecutingAttack = false;

        StartCoroutine(RoyalGuardCooldown());
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (rgisPushing && !rgPlayerHit))
        {
            Debug.Log("Hit player");
            rgPlayerHit = false;

            PushPlayer(rgDamage);
            StopCoroutine(rgActiveCoroutine);
            rgActiveCoroutine = StartCoroutine(StopPostHit());
        }
    }

    private IEnumerator StopPostHit()
    {
        rgisPushing = false;

        agent.speed = 0;
        agent.destination = transform.position;

        //Rotates smoothly towards player
        float time = 0f;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, time);
            time += Time.deltaTime * 0.6f;
            yield return null;
        }
        transform.rotation = currentFacePlayerRot;

        //Returns to its original look rotation state and waits before finalizing attack
        rgGuardActive = false;
        rgPushActive = false;
        yield return new WaitForSeconds(rgAfterPushTime);

        agent.speed = walkSpeed;
        agent.acceleration = 12f; //Resets to its default value
        isExecutingAttack = false;

        StartCoroutine(RoyalGuardCooldown());
        yield return null;
    }

    private IEnumerator RoyalGuardCooldown()
    {
        yield return new WaitForSeconds(rgCooldownTime);

        rgCooldownActive = false;
        yield return null;
    }

    #endregion ATTACK ABILITY

    #region DEBUG VISUAL GIZMOS

    #endregion DEBUG VISUAL GIZMOS
}
