using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("ENEMY ATTACK SETTINGS")]
    [SerializeField] protected float damage;
    [SerializeField] protected float attackCooldown;

    [SerializeField] protected LayerMask whatIsPlayer;

    [Header("ENEMY MOVEMENT SETTINGS")]
    [SerializeField] protected float speed;


    protected abstract void Attack();

    public void OnDamage(float damage)
    {
        GetComponent<Health>().TakeDamage(damage);
    }
}
