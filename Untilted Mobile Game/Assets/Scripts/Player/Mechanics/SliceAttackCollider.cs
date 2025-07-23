using UnityEngine;

public class SliceAttackCollider : MonoBehaviour
{
    [Header("SLICE ATTACK SETTINGS")]
    [SerializeField] private float sliceDamage;

    private CameraFollow playerCam;

    private void Start()
    {
        playerCam = Camera.main.GetComponent<CameraFollow>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && isDashing)
        {
            other.GetComponent<IDamageable>().OnDamage(sliceDamage);
            GetComponentInParent<SliceAttack>().CancelSliceMovement();
        }
    }

    private bool isDashing => GetComponentInParent<SliceAttack>().IsDashing;
}
