using UnityEngine;

public class ElectricArea : MonoBehaviour
{
    [Header("--- ELECTRIC AREA SETTINGS ---")]
    [SerializeField] private float damage;
    [SerializeField, Range(1f, 3f)] private float timeBetweenZaps;
    [SerializeField, Range(2f, 6f)] private float lifetime; 
    private bool ableToZap;

    private float timer;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (timer < 0)
        {
            ableToZap = true;
        }
        else
        {
            ableToZap = false;
        }

        Timer();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && ableToZap)
        {
            var cam = Camera.main;
            cam.GetComponent<PlayerCamera>().CameraShake();
            other.GetComponent<Health>().TakeDamage(damage);

            timer = timeBetweenZaps;
        }
    }

    private void Timer()
    {
        timer -= Time.deltaTime;
    }
}
