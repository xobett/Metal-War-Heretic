using System.Collections;
using UnityEngine;

public class ElectricEnemy : EnemyBase
{
    [Header("--- ELECTRIC ENEMY ---")]

    [SerializeField] private float evokerAttackDistance = 5f;

    [Header("-- MAIN ABILITY -- ELECTRIC AREA")]

    [SerializeField, Range(1f, 10f)] private int electricAreaLifetime;

    private bool electricAttackActive;

    [Header("ELECTRIC ATTACK CUE VFX")]
    [SerializeField] private GameObject cueElectricVfx;
    [SerializeField] private GameObject electricAreaPf;

    protected override void FollowPlayer()
    {
        if (!electricAttackActive)
        {
            base.FollowPlayer(); 
        }
    }

    protected override void Attack()
    {
        StartCoroutine(ElectricAttack());

        //Random behaviour, either attack on area attack or throw something
    }

    private IEnumerator ElectricAttack()
    {

        Vector3 spawnPos = player.transform.position;
        spawnPos.y = 1;

        GameObject vfx = Instantiate(cueElectricVfx, spawnPos, cueElectricVfx.transform.rotation);
        Destroy(vfx, 3);
        yield return new WaitForSeconds(3);

        GameObject electricArea = Instantiate(electricAreaPf, spawnPos, electricAreaPf.transform.rotation);
    }
}
