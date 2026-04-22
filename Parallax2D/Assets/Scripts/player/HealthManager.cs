using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private SpriteRenderer SpriteRenderer;
    public HealthBar healthBar;
    private Rigidbody2D rb;
    public GameObject DeathScreen;
    private float timeSinceLastDamage = 0f;
    private float regenDelay = 10f;
    private float regenRate = 5f;
    private float regenAccumulator = 0f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hurtSound;

    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

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
        StartCoroutine(BlinkRed());

        if (audioSource != null && hurtSound != null)
            audioSource.PlayOneShot(hurtSound);

        timeSinceLastDamage = 0f;
        regenAccumulator = 0f;

        if (currentHealth <= 0)
            Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            TakeDamage(25);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 15);
            StartCoroutine(BlinkRed());
        }
    }

    void Die()
    {
        DeathScreen.SetActive(true);
        Time.timeScale = 0;
    }

    private IEnumerator BlinkRed()
    {
        SpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        SpriteRenderer.color = Color.white;
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("Level1");
        Time.timeScale = 1;
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("mainmenu");
        Time.timeScale = 1;
    }
}