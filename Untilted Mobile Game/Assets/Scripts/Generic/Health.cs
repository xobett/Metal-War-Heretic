using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float CurrentHealth { get; private set; }
    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private Slider lifebar;

    [SerializeField] private GameObject onDeathVfx;
    private bool triggeredDeath;
    private Animator animator;

    internal bool damaged = false;
    internal bool deathByFalling = false;

    private void Awake()
    {
        Start_GetReferences();
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        AddHealth(maxHealth);
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
        

        if (CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFX("TAKE DAMAGE");
        }

        if (lifebar != null)
        {
            SetLifebarValue();
        }

        if (CurrentHealth <= 0)
        {
            Death();
        }

        if (damaged) return;
        damaged = true;
        Invoke(nameof(DisableDamagedState), 5);
    }

    private void DisableDamagedState()
    {
        damaged = false;
    }

    public void AddHealth(float health)
    {
        CurrentHealth += health;

        SetLifebarValue();
    }

    private void SetLifebarValue()
    {
        if (lifebar == null) return;

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

            animator.SetTrigger("Death");
        }
    }

    public void PlayerDeath()
    {
        if (deathByFalling)
        {
            RespawnPlayer();
        }
        else
        {
            Player.Instance.DisableMovement();
            Player.Instance.ResetOnAreaValue();
            gameObject.tag = "Dead";

            animator.CrossFade("Base Layer.Muerte", 0);

            Invoke(nameof(RespawnPlayer), 2.8f);
        }
    }

    private void RespawnPlayer()
    {
        deathByFalling = false;
        gameObject.tag = "Player";

        animator.CrossFade("Base Layer.Idle", 0);

        GetComponent<CharacterController>().enabled = false;
        transform.position = gameObject.GetComponent<CheckpointSaver>().lastCheckpoint;
        GetComponent<CharacterController>().enabled = true;

        Player.Instance.EnableMovement();
        AddHealth(maxHealth);
    }
}
