using System.Collections;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    //Follow settings
    [SerializeField] private Transform target;

    //Camera settings
    [SerializeField] private Vector3 offsetPosition;
    [SerializeField] private Quaternion offsetRotation;

    [SerializeField, Range(0f, 1f)] private float followSpeed;

    private Vector3 velocity;

    //Camera shake settings
    [SerializeField, Range(0.2f, 1f)] private float duration;
    [SerializeField] private AnimationCurve intensityCurve;
    public bool beginShake;


    private void Update()
    {
        if (beginShake)
        {
            beginShake = false;
            CameraShake();
        }
    }

    private void LateUpdate()
    {
        PlayerFollowing();
    }

    private void PlayerFollowing()
    {
        Vector3 offset = offsetRotation * offsetPosition;

        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, followSpeed * Time.deltaTime);
    }

    public void CameraShake()
    {
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        Vector3 startPos = target.localPosition;

        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float strength = intensityCurve.Evaluate(time / duration);
            target.localPosition = target.localPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        target.localPosition = startPos;

        yield return null;
    }
}
