using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float CurrentHealth { get; private set; }
    private const float maxHealth = 100f;

    [SerializeField] private Slider lifebar;

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
        lifebar = GameObject.FindGameObjectWithTag("Player Lifebar").GetComponentInChildren<Slider>();
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        Debug.Log($"Se ha recibido {damage} pts de daño. Vida actual = {CurrentHealth}");

        SetLifebarValue();

        if (CurrentHealth <= 0 )
        {
            Death();
        }
    }

    public void SetHealth(float health)
    {
        CurrentHealth += health;
        Debug.Log($"Se ha recibido {health} pts de vida. Vida actual = {CurrentHealth}");
        
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
            Debug.Log($"{name} died");
            Destroy(gameObject);
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
