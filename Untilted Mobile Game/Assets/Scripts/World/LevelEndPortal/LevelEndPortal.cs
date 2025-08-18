using Unity.VisualScripting;
using UnityEngine;

public class LevelEndPortal : MonoBehaviour
{
    private Animator anim;
    private AudioSource mainAudioSource;
    private AudioSource secondaryAudioSource;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();

        mainAudioSource = GetComponent<AudioSource>();
        secondaryAudioSource = GetComponentInChildren<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.Instance.DisableMovement();
            anim.SetTrigger("Teleport");
        }
    }

    public void AnimEvent_PlayTeleportSound()
    {
        mainAudioSource.clip = AudioManager.Instance.GetClip("TELEPORT");
        mainAudioSource.Play();
    }

    public void AnimEvent_PlayTeleportHitSound()
    {
        secondaryAudioSource.clip = AudioManager.Instance.GetClip("TELEPORT HIT");
        secondaryAudioSource.Play();
    }
}
