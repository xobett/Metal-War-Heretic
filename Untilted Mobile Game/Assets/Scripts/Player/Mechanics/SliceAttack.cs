using System.Collections;
using UnityEngine;

public class SliceAttack : MonoBehaviour
{
    [Header("SLICE ATTACK SETTINGS")]
    [SerializeField] private float sliceDamage;
    [SerializeField] private float sliceSpeed;

    [Header("SLICE COOLDOWN SETTINGS")]
    [SerializeField, Range(1f, 5f)] private float cooldownTime;

    [SerializeField] private bool isCooling;
    private bool isAttacking;

    //Reference to the player's character controller
    private CharacterController charCtrlr;

    void Start()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        charCtrlr = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        SliceMovement();
    }

    void SliceMovement()
    {
        if (isAttacking)
        {
            Vector3 sliceMovement = transform.rotation * Vector3.forward;
            charCtrlr.Move(sliceMovement * sliceSpeed * Time.deltaTime);
        }

        if (!isCooling && !isAttacking && IsSlicing())
        {
            StartCoroutine(Slice());
        }
    }

    private IEnumerator Slice()
    {
        isAttacking = true;
        isCooling = true;

        yield return new WaitForSeconds(0.2f);

        isAttacking = false;

        yield return new WaitForSeconds(cooldownTime);

        isCooling = false;

        StopCoroutine(Slice());
    }

    private bool IsSlicing()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && isAttacking)
        {
            other.GetComponent<IDamageable>().OnDamage(sliceDamage);
        }
    }
}
