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
    private AudioSource audioSource;

    private void Start()
    {
        Start_GetReferences();
        Start_EnableTimer();
        Start_SetAutoDestroy();
    }

    #region START

    private void Start_GetReferences()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        audioSource.outputAudioMixerGroup = AudioManager.Instance.sfxMixerGroup;
    }

    private void Start_EnableTimer()
    {
        Invoke(nameof(EnableTimer), 3f);

        audioSource.clip = AudioManager.Instance.GetClip("ELECTRICIDAD SUELO");
        audioSource.Play();
    }

    private void Start_SetAutoDestroy()
    {
        Invoke(nameof(AutoDestroy), 6.7f);
    }

    private void AutoDestroy()
    {
        Destroy(gameObject);
    }

    private void EnableTimer()
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
