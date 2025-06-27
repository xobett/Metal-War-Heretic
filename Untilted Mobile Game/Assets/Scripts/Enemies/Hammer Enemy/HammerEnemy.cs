using System.Collections;
using UnityEngine;

public class HammerEnemy : EnemyBase
{
    [Header("HAMMER ENEMY SETTINGS")]
    [SerializeField] private Animator anim_HammerEnemy;

    private bool isPunching;

    #region ENEMY BASE OVERRIDES

    protected override void Update()
    {
        base.Update();

        SetWalkAnimation();
    }

    protected override void Attack()
    {
        StartCoroutine(ThrowHammerPunch());
    }

    protected override void LookAtPlayer()
    {
        if (!isPunching)
        {
            base.LookAtPlayer(); 
        }
    }

    #endregion ENEMY BASE OVERRIDES

    #region ATTACK ABILITY
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit player");
        }
    }

    private IEnumerator ThrowHammerPunch()
    {
        anim_HammerEnemy.SetTrigger("Attack");
        isPunching = true;

        yield return new WaitForSeconds(4);

        isPunching = false;
        isExecutingAttack = false;
        yield return null;
    }

    #endregion ATTACK ABILITY

    #region ANIMATIONS

    private void SetWalkAnimation()
    {
        if (agent.velocity.magnitude != 0)
        {
            anim_HammerEnemy.SetBool("isWalking", true);
        }
        else
        {
            anim_HammerEnemy.SetBool("isWalking", false);
        }
    }

    #endregion ANIMATIONS

}
