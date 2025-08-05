using UnityEngine;

public class SliceAttackCollider : MonoBehaviour
{
    [Header("SLICE ATTACK SETTINGS")]
    [SerializeField] private float sliceDamage;

    private CameraFollowPH playerCam;

    private void Start()
    {
        playerCam = Camera.main.GetComponent<CameraFollowPH>();
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
