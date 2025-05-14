using UnityEngine;

public class EasyEnemy : EnemyBase
{
    protected override void Attack()
    {
        Debug.Log("Attacked player");
    }
}