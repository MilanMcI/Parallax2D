using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip slashSound;

    private float nextAttackTime = 0f;

    void Start()
    {
        if (GameSettings.Instance != null)
        {
            attackDamage = GameSettings.Instance.playerDamage;
            Debug.Log("Player Damage Loaded: " + attackDamage);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
{
    Debug.Log("Attack called!");
    
    if (audioSource != null && slashSound != null)
        audioSource.PlayOneShot(slashSound);

    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
    
    Debug.Log("Enemies hit: " + hitEnemies.Length);

    foreach (Collider2D enemy in hitEnemies)
    {
        enemy.GetComponent<EnemyHealth>()?.TakeDamage(attackDamage);
    }
}

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}