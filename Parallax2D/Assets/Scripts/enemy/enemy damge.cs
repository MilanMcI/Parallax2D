using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float damageCooldown = 1f; // prevents instant kill
    private float nextDamageTime = 0f;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            nextDamageTime = Time.time + damageCooldown;
        }
    }
}