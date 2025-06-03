using UnityEngine;
using UnityEngine.UI;

public class MeleeAttack : MonoBehaviour
{
    [Header("ATTACK BUTTON")]
    [SerializeField] private Button attackButton;

    [Header("MELEE ATTACK SETTINGS")]
    [SerializeField, Range(10f, 50f)] private float damage;
    [SerializeField] private float hitRate;
    [SerializeField] private float hitRange = 0.5f;

    [SerializeField] private LayerMask whatIsMelee;

    private PlayerCamera playerCam;

    [Header("AIM ASSIT SETTINGS")]
    [SerializeField, Range(1f, 5f)] private float radius;
    public bool aimAssitActive;

    private void Start()
    {
        GetReferences();
        AddButtonEvents();
    }

    private void GetReferences()
    {
        playerCam = Camera.main.GetComponent<PlayerCamera>();
    }

    private void AddButtonEvents()
    {
        attackButton.onClick.AddListener(Punch);
    }

    private void Update()
    {
        HitCheck();
        AimAssit();
    }

    public void HitCheck()
    {
        if(IsHitting())
        {
            Punch();
        }
    }

    private void Punch()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward * hitRange, out hit, hitRange, whatIsMelee))
        {
            hit.collider.GetComponent<IDamageable>().OnDamage(damage);
            playerCam.CameraShake();
        }
    }

    private void AimAssit()
    {
        //Check for enemies in range

        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, radius, whatIsMelee, QueryTriggerInteraction.UseGlobal);

        if (enemyColliders.Length != 0)
        {
            //Look at enemy directly

            GameObject enemy = enemyColliders[0].gameObject; 

            Quaternion target = Quaternion.LookRotation(enemy.transform.position - transform.position);
            Quaternion lookRotation = Quaternion.Slerp(transform.rotation, target, 3.5f * Time.deltaTime);
            transform.rotation = lookRotation;

            aimAssitActive = true;
        }
        else
        {
            aimAssitActive = false;
        }
    }

    public bool IsActivatingAim() => Input.GetKeyDown(KeyCode.O);
    public bool IsHitting() => Input.GetKeyDown(KeyCode.E);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * hitRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}