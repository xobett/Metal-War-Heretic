using EnemyAI;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

public class LightningCollider : MonoBehaviour
{
    [Header("--- LIGHTNING COLLIDER SETTINGS---")]
    [SerializeField] private float damage;
    [SerializeField] private float lifetime;
    [SerializeField] private float speed;

    private Rigidbody rb;
    private Vector3 moveDirection;

    [SerializeField] private GameObject hitPlayerVfx;
    [SerializeField] private SOShakeData enemyHitShake;

    Vector3 debugCubeSize;

    void Start()
    {
        Start_SetSettings();
    }

    #region START

    private void Start_SetSettings()
    {
        rb = GetComponent<Rigidbody>();
        debugCubeSize = GetComponent<BoxCollider>().size;

        Invoke(nameof(AutoDestroy), 1.8f);
    }

    private void AutoDestroy()
    {
        Destroy(gameObject);
    }

    #endregion START

    #region AUTO DESTROY

    #endregion AUTO DESTROY

    #region SET ELECTRIC BALL SETTINGS

    public void SetLightningSettings(Vector3 moveDirection, float damage, float speed)
    {
        this.moveDirection = moveDirection;

        this.damage = damage;
        this.speed = speed;
    }

    #endregion SET ELECTRIC BALL SETTINGS

    #region BALL MOVEMENT

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        rb.MovePosition(transform.position + moveDirection * speed * Time.deltaTime);
    }

    #endregion BALL MOVEMENT

    #region BALL DAMAGE

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var cam = Camera.main;

            Vector3 hitPos = other.transform.position + other.transform.forward * 0.4f;
            GameObject vfx = Instantiate(hitPlayerVfx, hitPos, Quaternion.identity);
            Destroy(vfx, 1);

            other.GetComponent<Health>().TakeDamage(damage);
            Destroy(gameObject);

            ShakeEventManager.Instance.AddShakeEvent(enemyHitShake);
        }
    }

    #endregion BALL DAMAGE

    private void OnDrawGizmos()
    {
        if (rb == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawCube(rb.position, debugCubeSize);
    }
}
