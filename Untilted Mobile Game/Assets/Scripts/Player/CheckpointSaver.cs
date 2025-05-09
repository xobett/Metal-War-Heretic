using UnityEngine;

public class CheckpointSaver : MonoBehaviour
{
    public Vector3 lastCheckpoint;

    private void Start()
    {
        lastCheckpoint = transform.position;
    }
}
