using System.Collections.Generic;
using UnityEngine;

public class ShakeEventManager : MonoBehaviour
{
    /// <summary>
    /// Keeps track of all active Shake Events, removing them after finishing their duration
    /// </summary>

    public static ShakeEventManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private List<ShakeEvent> shakeEvents = new List<ShakeEvent>();

    public void AddShakeEvent(SOShakeData data)
    {
        shakeEvents.Add(new ShakeEvent(data));
    }

    private void LateUpdate()
    {
        Vector3 positionOffset = Vector3.zero;
        Vector3 rotationOffset = Vector3.zero;

        for (int i = shakeEvents.Count - 1; i != -1; i--)
        {
            ShakeEvent shake = shakeEvents[i];
            Vector3 shakeOffset = shake.GetShake_Update();

            switch (shake.shakeType)
            {
                case (SOShakeData.ShakeType.Position):
                    {
                        positionOffset += shakeOffset;
                        break;
                    }

                case SOShakeData.ShakeType.Rotation:
                    {
                        rotationOffset += shakeOffset;
                        break;
                    }
            }

            if (!shake.IsShaking())
            {
                shakeEvents.RemoveAt(i);
            }

        }

        transform.localPosition = positionOffset;
        transform.localEulerAngles = rotationOffset;
    }
}