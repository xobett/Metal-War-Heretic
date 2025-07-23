using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Camera Shake/Create Shake Data", fileName = "New Shake Data")]
public class SOShakeData : ScriptableObject
{
    /// <summary>
    /// Creates Scriptable Objects that contain the data required to create ShakeEvent.
    /// </summary>

    public enum ShakeType
    {
        Position,
        Rotation
    }

    public ShakeType shakeType = ShakeType.Position;

    public float amplitude = 1.0f;
    public float frequency = 1.0f;
    public float duration = 1.0f;

    public AnimationCurve lifetimeCurve = new AnimationCurve(
        new Keyframe(0.0f, 0.0f, Mathf.Deg2Rad * 0.0f, Mathf.Deg2Rad * 720.0f),
        new Keyframe(0.2f, 1.0f),
        new Keyframe(1.0f, 0.0f));
}
