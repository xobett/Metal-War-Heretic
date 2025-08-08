using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TrainWagon : MonoBehaviour
{
    internal float speed;
    private Rigidbody rb;

    private TrainManager manager;

    void Start()
    {
        Start_GetReferences();
    }

    #region START

    private void Start_GetReferences()
    {
        manager = GetComponentInParent<TrainManager>();
        rb = GetComponent<Rigidbody>();
    }

    #endregion START

    private void FixedUpdate()
    {
        Move();
    }

    #region MOVEMENT

    private void Move()
    {
        rb.MovePosition(transform.position + -transform.right * Time.fixedDeltaTime * speed);
    }

    #endregion MOVEMENT

    #region TRIGGER

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("End Point"))
        {
            manager.ResetWagon(this.gameObject);

            if (gameObject.name == "End Wagon") manager.SpawnTrain();
        }

        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(100);
        }
    }

    #endregion TRIGGER


}
