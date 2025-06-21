using UnityEngine;

public class HammerEnemy : EnemyBase
{
    [SerializeField] private Animator anim_HammerEnemy;

    protected override void Attack()
    {
        base.Attack();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit player");
        }
    }
}
