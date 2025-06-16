using System.Collections;
using UnityEngine;

public class ElectricEnemy : EnemyBase
{
    [Header("--- ELECTRIC ENEMY SETTINGS ---\n")]

    [Header("MAIN ABILITY - DISTANCE ATTACK\n")]

    [Header("DISTANCE ATTACK PREFAB")]
    [SerializeField] private GameObject distanceAttackPf;

    [Header("DISTANCE ATTACK SETTINGS")]
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private float beforeDistanceAttackTime;
    [SerializeField] private float afterDistanceAttackTime;

    [SerializeField] private float distanceAttackSpeed;
    [SerializeField] private float distanceAttackDamage;
    [SerializeField] private float distanceAttackLifetime;

    [Header("SECONDARY ABILITY - ELECTRIC ATTACK\n")]

    [Header("ELECTRIC ATTACK PREFAB | CUE VFX")]
    [SerializeField] private GameObject electricAreaPf;
    [SerializeField] private GameObject cueElectricVfx;

    [Header("ELECTRIC ATTACK SETTINGS")]
    [SerializeField] private int electricAreaLifetime; //
    [SerializeField] private int electricAreaDamage; //

    [SerializeField, Range(1f, 10f)] private int electricAttackCooldownTime;
    private bool electricAttackCoolingDown;

    private bool electricAttackActive;

    #region BASE OVERRIDES

    #region BASE MOVEMENT AND ROTATION

    protected override void LookAtPlayer()
    {
        if (!electricAttackActive)
        {
            base.LookAtPlayer(); 
        }
    }

    #endregion BASE MOVEMENT AND ROTATION

    #region BASE ATTACK
    protected override void Attack()
    {
        GetAttackAbility();
    }

    #endregion BASE ATTACK

    #endregion BASE OVERRIDES

    #region ATTACK ABILITIES

    private void GetAttackAbility()
    {
        if (!electricAttackCoolingDown)
        {
            StartCoroutine(ElectricAttack());
        }
        else
        {
            StartCoroutine(DistanceAttack());
        }
    }

    #region MAIN ABILITY - DISTANCE ATTACK

    private IEnumerator DistanceAttack()
    {
        Debug.Log("Entered distance attack");
        yield return new WaitForSeconds(beforeDistanceAttackTime);

        Debug.Log("Threw ball");
        distanceAttackPf.GetComponent<ElectricBall>().SetElectricBallSettings(transform.forward, distanceAttackDamage, distanceAttackSpeed, distanceAttackLifetime);
        Instantiate(distanceAttackPf, spawnPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(afterDistanceAttackTime);

        isExecutingAttack = false;

        yield return null;
    }

    #endregion MAIN ABILITY - DISTANCE ATTACK

    #region SECONDARY ABILITY - ELECTRIC ATTACK

    private IEnumerator ElectricAttack()
    {
        Debug.Log("Entered electric attack");
        electricAttackActive = true;
        electricAttackCoolingDown = true;

        Vector3 spawnPos = player.transform.position;
        spawnPos.y = 1.2f;

        GameObject vfx = Instantiate(cueElectricVfx, spawnPos, cueElectricVfx.transform.rotation);
        Destroy(vfx, 3);
        yield return new WaitForSeconds(3);

        electricAreaPf.GetComponent<ElectricArea>().SetElectricAreaSettings(electricAreaDamage, electricAreaLifetime);
        Instantiate(electricAreaPf, spawnPos, electricAreaPf.transform.rotation);
        yield return new WaitForSeconds(5);

        //Looks smoothly at player before returning to its default state
        float time = 0f;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, currentFacePlayerRot, time);
            time += Time.deltaTime * 0.6f;
            yield return null;
        }
        transform.rotation = currentFacePlayerRot;

        electricAttackActive = false;
        isExecutingAttack = false;

        StartCoroutine(StartElectricCooldown());
        yield return null;
    }

    private IEnumerator StartElectricCooldown()
    {
        yield return new WaitForSeconds(electricAttackCooldownTime);
        
        electricAttackCoolingDown = false;
        yield return null;
    }

    #endregion SECONDARY ABILITY - ELECTRIC ATTACK

    #endregion ATTACK ABILITIES
}
