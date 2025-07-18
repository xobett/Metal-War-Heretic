using System.Collections;
using UnityEngine;

public class ElectricArea : MonoBehaviour
{
    [Header("--- ELECTRIC AREA SETTINGS ---")]
    [SerializeField] private float timeBetweenZaps;
    private bool ableToZap;
    private float timer;

    [SerializeField] private float damage;
    [SerializeField] private float lifetime;

    [SerializeField] private bool ableToAutoDestroy;

    private void Start()
    {
        StartCoroutine(AutoDestroy());
    }

    private void Update()
    {
        Timer();
    }

    #region SET ELECTRIC AREA SETTINGS

    public void SetElectricAreaSettings(float damage, float lifetime)
    {
        this.damage = damage;
        this.lifetime = lifetime;

        ableToAutoDestroy = true;
    }

    #endregion SET ELECTRIC AREA SETTINGS

    #region AUTO DESTROY

    private IEnumerator AutoDestroy()
    {
        yield return new WaitUntil(() => ableToAutoDestroy);

        Destroy(gameObject, lifetime);
    }

    #endregion AUTO DESTROY

    #region ELECTRIC DAMAGE

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

    #endregion ELECTRIC DAMAGE

    #region TIMER

    private void Timer()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            ableToZap = true;
        }
        else
        {
            ableToZap = false;
        }
    }

    #endregion TIMER
}
