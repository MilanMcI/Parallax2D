using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private int damage = 25;
    [SerializeField] private float damageCooldown = 1f;
    private float nextDamageTime = 0f;

    void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Colliding with: " + collision.gameObject.name + " Tag: " + collision.gameObject.tag);
        
        if (collision.gameObject.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            collision.gameObject.GetComponent<HealthManager>()?.TakeDamage(damage);
            nextDamageTime = Time.time + damageCooldown;
        }
    }
}