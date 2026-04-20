using UnityEngine;

public class Snowball : MonoBehaviour
{
    private int damage = 30;
    private float speed = 10f;
    private Vector2 direction;
    private Animation anim;

    void Start()
    {
        anim = GetComponent<Animation>();
        if (anim != null)
            anim.Play("snowballan"); // make sure this matches your clip name exactly
    }

    public void Init(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            other.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}