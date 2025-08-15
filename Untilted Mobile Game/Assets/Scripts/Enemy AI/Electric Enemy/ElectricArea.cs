using System.Collections;
using UnityEngine;

public class ElectricArea : MonoBehaviour
{
    [Header("--- ELECTRIC AREA SETTINGS ---")]
    [SerializeField] private float timeBetweenZaps;
    private bool ableToZap;
    private float timer;

    private bool timerEnabled = false;

    [SerializeField] private float damage;
    [SerializeField] private float lifetime;

    [SerializeField] private bool ableToAutoDestroy;

    [SerializeField] private GameObject hitPlayerVfx;

    private void Start()
    {
        StartCoroutine(AutoDestroy());
        Invoke(nameof(Start_EnableTimer), 3f);
    }

    #region START

    private void Start_EnableTimer()
    {
        timerEnabled = true;
    }

    #endregion START

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
            Vector3 hitPos = other.transform.position + other.transform.forward * 0.4f;

            GameObject vfx = Instantiate(hitPlayerVfx, hitPos, hitPlayerVfx.transform.rotation);
            other.GetComponent<Health>().TakeDamage(damage);
            Destroy(vfx, 1);

            timer = timeBetweenZaps;
        }
    }

    #endregion ELECTRIC DAMAGE

    #region TIMER

    private void Timer()
    {
        if (!timerEnabled) return;

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
