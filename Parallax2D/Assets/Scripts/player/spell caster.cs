using UnityEngine;

public class SpellCast : MonoBehaviour
{
    [SerializeField] private GameObject snowballPrefab;
    [SerializeField] private Transform castPoint;
    [SerializeField] private float spellCooldown = 0.5f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float nextSpellTime = 0f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && Time.time >= nextSpellTime)
        {
            CastSnowball();
            nextSpellTime = Time.time + spellCooldown;
        }
    }
  void CastSnowball()
 {
    // Use the direction the sprite is facing
    Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;

    GameObject snowball = Instantiate(snowballPrefab, castPoint.position, Quaternion.identity);
    snowball.GetComponent<Snowball>().Init(direction);
 }
    
}