using UnityEngine;

public class SliceAttackCollider : MonoBehaviour
{
    [Header("SLICE ATTACK SETTINGS")]
    [SerializeField] private float sliceDamage;

    private PlayerCamera playerCam;

    private void Start()
    {
        playerCam = Camera.main.GetComponent<PlayerCamera>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && isDashing)
        {
            playerCam.CameraShake();
            other.GetComponent<IDamageable>().OnDamage(sliceDamage);
        }
    }
    private bool isDashing => GetComponentInParent<SliceAttack>().isDashing;
}
