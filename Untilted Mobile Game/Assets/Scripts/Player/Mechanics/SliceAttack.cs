using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SliceAttack : MonoBehaviour
{
    [Header("SLICE ATTACK SETTINGS")]
    [SerializeField] private float sliceDamage;
    [SerializeField] private float sliceSpeed;
    private const float sliceDuration = 0.2f;

    private PlayerCamera cam;

    [Header("SLICE COOLDOWN SETTINGS")]
    [SerializeField, Range(1f, 5f)] private float cooldownTime;

    [SerializeField] private bool isCooling;
    private bool isAttacking;

    [Header("SLICE EFFECT SETTINGS")]
    [SerializeField] private Volume volume;

    [SerializeField] private float lensIntensity = -0.4f;
    [SerializeField] private float chromaticIntensity = 0.4f;

    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;

    //Reference to the player's character controller
    private CharacterController charCtrlr;

    void Start()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        charCtrlr = GetComponent<CharacterController>();

        cam = Camera.main.GetComponent<PlayerCamera>();

        volume.profile.TryGet<LensDistortion>(out lensDistortion);
        volume.profile.TryGet<ChromaticAberration>(out chromaticAberration);
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

        yield return new WaitForSeconds(sliceDuration);

        isAttacking = false;

        yield return new WaitForSeconds(cooldownTime);

        isCooling = false;

        StopCoroutine(Slice());
    }

    private IEnumerator DashEffect(float lensValue, float chromaticValue)
    {
        float targetLens = lensDistortion.intensity.value + lensValue;
        float targetChromatic = chromaticAberration.intensity.value + chromaticValue;

        float time = 0;

        while (time < 1)
        {
            lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, lensValue, time);
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, chromaticValue, time);
            time += Time.deltaTime * (sliceDuration / 2);
            yield return null;
        }

        lensDistortion.intensity.value = targetLens;
        chromaticAberration.intensity.value = targetChromatic;

        yield return null;
        
    }

    private bool IsSlicing()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && isAttacking)
        {
            cam.CameraShake();
            other.GetComponent<IDamageable>().OnDamage(sliceDamage);
        }
    }
}
