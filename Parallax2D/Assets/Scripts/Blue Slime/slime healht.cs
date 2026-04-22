using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 30;
    private int currentHealth;

    void Start()
    {
        if (GameSettings.Instance != null)
        {
            maxHealth = GameSettings.Instance.enemyMaxHealth;
            Debug.Log(gameObject.name + " Enemy Health Loaded: " + maxHealth);
        }

        currentHealth = maxHealth;
        Debug.Log(gameObject.name + " spawned with HP: " + currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage! HP left: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died!");
        Destroy(gameObject);
    }
}