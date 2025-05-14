using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("MELEE ATTACK SETTINGS")]
    [SerializeField, Range(10f, 50f)] private float damage;
    [SerializeField] private float hitRate;

    [SerializeField] private LayerMask whatIsMelee;

    [SerializeField] private float hitRange = 0.5f;

    private void Start()
    {
        
    }

    private void Update()
    {
        Hit();
    }

    public void Hit()
    {
        if(IsHitting())
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward * hitRange, out hit, hitRange, whatIsMelee))
            {
                hit.collider.GetComponent<IDamageable>().OnDamage(damage);
            }
        }
    }

    public bool IsHitting()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * hitRange);
    }
}