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

    [Header("ROYAL GUARD SETTINGS")]
    [SerializeField] private int beforeShieldSmashTime;
    [SerializeField] private float royalDamage;
    [SerializeField] private float royalDamageRadius;
    [SerializeField] private int guardingTime;

    #region BASE OVERRIDES

    #region BASE ATTACK

    protected override void Attack()
    {
        GetAbility();
    }

    #endregion BASE ATTACK

    #region BASE MOVEMENT AND ROTATION

    protected override void LookAtPlayer()
    {
        base.LookAtPlayer();
    }

    #endregion BASE MOVEMENT AND ROTATION

    #endregion BASE OVERRIDES

    #region ATTACK ABILITY

    private void GetAbility()
    {

    }

    private IEnumerator CrStartPhaseOneRG()
    {
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
        yield return new WaitForSeconds(beforeShieldSmashTime);

        if (Physics.CheckSphere(transform.position, royalDamageRadius))

        yield return null;
    }

    #endregion ATTACK ABILITY

    #region DEBUG VISUAL GIZMOS

    

    #endregion DEBUG VISUAL GIZMOS
}
