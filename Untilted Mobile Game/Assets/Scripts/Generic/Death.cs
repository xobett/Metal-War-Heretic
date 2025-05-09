using UnityEngine;

public class Death : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<CharacterController>().enabled = false;

        other.transform.position = other.gameObject.GetComponent<CheckpointSaver>().lastCheckpoint;

        other.gameObject.GetComponent<CharacterController>().enabled = true;
    }
}
