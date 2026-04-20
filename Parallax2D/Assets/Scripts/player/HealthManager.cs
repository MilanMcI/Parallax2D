using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;

    private float timeSinceLastDamage = 0f;
    private float regenDelay = 10f;
    private float regenRate = 5f;
    private float regenAccumulator = 0f;

    void Start()
    {
        if (GameSettings.Instance != null)
        {
            maxHealth = GameSettings.Instance.playerMaxHealth;
            Debug.Log("Player Max Health Loaded: " + maxHealth);
        }

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {

        if (currentHealth > 0 && currentHealth < maxHealth)
        {
            timeSinceLastDamage += Time.deltaTime;

            if (timeSinceLastDamage >= regenDelay)
            {
                regenAccumulator += regenRate * Time.deltaTime;

                if (regenAccumulator >= 1f)
                {
                    int regenAmount = Mathf.FloorToInt(regenAccumulator);
                    currentHealth = Mathf.Min(currentHealth + regenAmount, maxHealth);
                    healthBar.SetCurrentHealth(currentHealth);
                    regenAccumulator -= regenAmount;
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        healthBar.SetCurrentHealth(currentHealth);

        timeSinceLastDamage = 0f;
        regenAccumulator = 0f;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Player died!");
        SceneManager.LoadScene("GameOver");
    }
}