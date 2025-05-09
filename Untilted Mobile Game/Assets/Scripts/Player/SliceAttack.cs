using System.Collections;
using UnityEngine;

public class SliceAttack : MonoBehaviour
{
    [SerializeField] private float sliceDamage;
    [SerializeField] private float sliceSpeed;

    private CharacterController charCtrlr;

    private bool isAttacking;

    void Start()
    {
        charCtrlr = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking && IsSlicing())
        {
            StartCoroutine(Slice());
        }

        Attack();
    }

    void Attack()
    {
        if (isAttacking)
        {
            Vector3 sliceMovement = transform.rotation * Vector3.forward;
            charCtrlr.Move(sliceMovement * sliceSpeed * Time.deltaTime);
        }
    }

    private IEnumerator Slice()
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.2f);

        isAttacking = false;

        StopCoroutine(Slice());
    }

    private bool IsSlicing()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<IDamageable>().OnDamage(sliceDamage);
        }
    }
}
