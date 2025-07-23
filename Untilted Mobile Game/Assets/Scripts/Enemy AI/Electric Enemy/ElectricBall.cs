using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class ElectricBall : MonoBehaviour
{
    [Header("--- ELECTRIC BALL SETTINGS---")]
    [SerializeField] private float damage;
    [SerializeField] private float lifetime;
    [SerializeField] private float speed;

    private bool ableToAutoDestoy;

    private Rigidbody rb;
    [SerializeField] private Vector3 moveDirection;

    void Start()
    {
        SetSettings();
    }

    #region AUTO DESTROY

    private IEnumerator AutoDestroy()
    {
        yield return new WaitUntil(() => ableToAutoDestoy);

        Destroy(gameObject, lifetime);
    }

    #endregion AUTO DESTROY

    #region SET ELECTRIC BALL SETTINGS

    public void SetElectricBallSettings(Vector3 moveDirection, float damage, float speed, float lifetime)
    {
        this.moveDirection = moveDirection;

        this.damage = damage;
        this.speed = speed;
        this.lifetime = lifetime;

        ableToAutoDestoy = true;
    }

    #endregion SET ELECTRIC BALL SETTINGS

    #region BALL MOVEMENT

    private void FixedUpdate()
    {
        BallMovement();
    }

    private void BallMovement()
    {
        rb.MovePosition(transform.position + moveDirection * speed * Time.deltaTime);
    }

    #endregion BALL MOVEMENT

    #region START

    private void SetSettings()
    {
        rb = GetComponent<Rigidbody>();

        StartCoroutine(AutoDestroy());
    }

    #endregion START

    #region BALL DAMAGE

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var cam = Camera.main;

            other.GetComponent<Health>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    #endregion BALL DAMAGE
}
