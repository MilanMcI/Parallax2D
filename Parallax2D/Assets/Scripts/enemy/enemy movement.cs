using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolDistance = 3f;  // how far it walks each way

    private Vector2 startPosition;
    private int direction = 1;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        // Move in current direction
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        // Flip sprite to face direction
        spriteRenderer.flipX = direction == -1;

        // Check if reached patrol boundary
        float distanceFromStart = transform.position.x - startPosition.x;

        if (distanceFromStart >= patrolDistance)
            direction = -1;   // turn left
        else if (distanceFromStart <= -patrolDistance)
            direction = 1;    // turn right
    }
}

