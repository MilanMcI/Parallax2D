using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;

// Just a simple health manager to show how you can controll the HealthBar UI
public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public GameObject DeathScreen; 


    public HealthBar healthBar;
    private Rigidbody2D rb;
    private SpriteRenderer SpriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Damage player when we press the G key
        if (Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(20);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetCurrentHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            TakeDamage(25);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 25);
            StartCoroutine(BlinkRed());
        }

    }

    private IEnumerator BlinkRed()
    {
        SpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        SpriteRenderer.color = Color.white;
    }

    private void Die()
    {
        DeathScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartButton()
        {
            SceneManager.LoadScene("Level1");
            Time.timeScale = 1;
        }   

    public void MainMenuButton()
        {
            SceneManager.LoadScene("TitleScreen");
            Time.timeScale = 1;
        }
}

