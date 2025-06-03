using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor.Embree;

public class SliceAttack : MonoBehaviour
{
    [Header("SLICE BUTTON")]
    [SerializeField] private Button sliceButton;

    [Header("SLICE ATTACK SETTINGS")]
    [SerializeField] private float sliceDamage;
    [SerializeField] private float sliceSpeed;
    private const float sliceDuration = 0.2f;

    private PlayerCamera cam;

    [Header("SLICE ATTACK ASSIST SETTINGS")]
    [SerializeField, Range(1f, 5f)] private float assistRadius;
    [SerializeField] private LayerMask whatIsEnemy;

    private bool isPressingSlice;

    [Header("SLICE COOLDOWN SETTINGS")]
    [SerializeField, Range(1f, 5f)] private float cooldownTime;

    [SerializeField] private bool isCooling;
    private bool isDashing;

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
        AddButtonEvents();
    }

    private void AddButtonEvents()
    {
        sliceButton.onClick.AddListener(PressSlice);
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
        SliceCheck();
        SnapToEnemy();
    }

    void SliceMovement()
    {
        if (isDashing)
        {
            Vector3 dashMovement = Vector3.zero;

            if (aimAssistActive && (JoystickManager.Instance.HorizontalInput() != 0 || JoystickManager.Instance.ForwardInput() != 0))
            {
                dashMovement = Vector3.right * JoystickManager.Instance.HorizontalInput() + Vector3.forward * JoystickManager.Instance.ForwardInput();
            }
            else
            {
                dashMovement = transform.rotation * Vector3.forward;
            }

            charCtrlr.Move(dashMovement * sliceSpeed * Time.deltaTime);
        }

        SliceCheck();
    }

    private void SliceCheck()
    {
        if (!isCooling && !isDashing && (IsSlicing() || isPressingSlice))
        {
            StartCoroutine(StartSlice());
        }
    }


    //Add a method that detects enemy on a dash and snaps to it!
    private void SnapToEnemy()
    {
        //Add logic that will snap to enemy on dash

        if (isDashing)
        {
            Collider[] enemyColliders = Physics.OverlapSphere(transform.position, assistRadius, whatIsEnemy, QueryTriggerInteraction.UseGlobal);

            if (enemyColliders.Length != 0)
            {
                GameObject lockedEnemy = enemyColliders[0].gameObject;
                transform.position = Vector3.MoveTowards(transform.position, lockedEnemy.transform.position, Time.deltaTime * sliceSpeed * 2f);
            }
        }
    }

    private IEnumerator StartSlice()
    {
        isPressingSlice = false;

        isDashing = true;
        isCooling = true;

        yield return new WaitForSeconds(sliceDuration);

        isDashing = false;

        yield return new WaitForSeconds(cooldownTime);

        isCooling = false;

        StopCoroutine(StartSlice());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && isDashing)
        {
            cam.CameraShake();
            other.GetComponent<IDamageable>().OnDamage(sliceDamage);
        }
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

    private bool aimAssistActive => gameObject.GetComponent<MeleeAttack>().aimAssitActive;

    private void PressSlice()
    {
        isPressingSlice = true;
    }

    private bool IsSlicing()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, assistRadius);
    }
}
