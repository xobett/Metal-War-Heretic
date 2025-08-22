using UnityEngine;

[System.Serializable]
public class ShakeEvent
{
    private Vector3 noiseCoordinates;
    public Vector3 noise;

    private const float randomOffset = 32.0f;

    private float duration;
    private float timeRemaining;

    private SOShakeData shakeData;
    public SOShakeData.ShakeType shakeType => shakeData.shakeType;

    public ShakeEvent(SOShakeData data)
    {
        shakeData = data;
        SetEventData();
    }

    private void SetEventData()
    {
        duration = shakeData.duration;
        timeRemaining = duration;

        noiseCoordinates.x = Random.Range(0, randomOffset);
        noiseCoordinates.y = Random.Range(0, randomOffset);
        noiseCoordinates.z = Random.Range(0, randomOffset);
    }

    public Vector3 GetShake_Update()
    {
        timeRemaining -= Time.deltaTime;

        float deltaMovement = Time.deltaTime * shakeData.frequency;

        noiseCoordinates.x += deltaMovement;
        noiseCoordinates.y += deltaMovement;
        noiseCoordinates.z += deltaMovement;

        noise.x = Mathf.PerlinNoise(noiseCoordinates.x, 0.0f);
        noise.y = Mathf.PerlinNoise(noiseCoordinates.y, 1.0f);
        noise.z = Mathf.PerlinNoise(noiseCoordinates.z, 2.0f);

        noise -= Vector3.one * 0.5f;

        noise *= shakeData.amplitude;

        float normalizedDuration = 1.0f - (timeRemaining / duration);
        noise *= shakeData.lifetimeCurve.Evaluate(normalizedDuration);

        return noise;
    }

    public bool IsShaking()
    {
        return timeRemaining > 0.0f;
    } 
}