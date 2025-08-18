using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float CurrentHealth { get; private set; }
    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private Slider lifebar;

    [SerializeField] private GameObject onDeathVfx;
    private bool triggeredDeath;

    private void Awake()
    {
        Start_GetReferences();

    }

    void Start()
    {
        SetHealth(maxHealth);
    }

    private void Start_GetReferences()
    {
        if (gameObject.CompareTag("Player"))
        {
            lifebar = GameObject.FindGameObjectWithTag("Player Lifebar").GetComponentInChildren<Slider>();
        }
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        if (lifebar != null)
        {
            SetLifebarValue();
        }

        if (CurrentHealth <= 0)
        {
            Death();
        }
    }

    public void SetHealth(float health)
    {
        CurrentHealth += health;

        SetLifebarValue();
    }

    private void SetLifebarValue()
    {
        lifebar.value = CurrentHealth / maxHealth;
    }

    private void Death()
    {
        if (gameObject.CompareTag("Player"))
        {
            PlayerDeath();
        }
        else
        {
            if (triggeredDeath) return;
            triggeredDeath = true;

            AudioManager.Instance.PlaySFX("MUERTE ENEMIGOS");

            Animator animator = GetComponentInChildren<Animator>();
            animator.SetTrigger("Death");
        }
    }

    public void PlayerDeath()
    {
        gameObject.GetComponent<CharacterController>().enabled = false;
        transform.position = gameObject.GetComponent<CheckpointSaver>().lastCheckpoint;
        gameObject.GetComponent<CharacterController>().enabled = true;

        SetHealth(maxHealth);
    }
}
