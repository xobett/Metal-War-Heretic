using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float health;
    private const float maxHealth = 100f;

    private float CurrentHealth
    {
        get { return health;}
        set
        {
            if (value > maxHealth)
            {
                health = maxHealth;
            }
            else if (value < 0)
            {
                health = 0;
            }
            else
            {
                health = value;
            }
        }
    }

    void Start()
    {
        SetHealth(maxHealth);
    }

    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        Debug.Log($"Se ha recibido {damage} pts de daño. Vida actual = {CurrentHealth}");

        if (CurrentHealth <= 0 )
        {
            Death();
        }
    }

    public void SetHealth(float health)
    {
        CurrentHealth += health;
        Debug.Log($"Se ha recibido {health} pts de vida. Vida actual = {CurrentHealth}");
    }

    private void Death()
    {
        Debug.Log("Enemy died");
        Destroy(gameObject);
    }
}
