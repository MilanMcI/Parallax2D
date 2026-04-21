using UnityEngine;

public class SpellCast : MonoBehaviour
{
    [SerializeField] private GameObject snowballPrefab;
    [SerializeField] private Transform castPoint;
    [SerializeField] private float spellCooldown = 0.5f;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private SpellCooldownUI cooldownUI; // add this

    private float nextSpellTime = 0f;

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
        Vector2 direction = playerMovement.IsFacingRight ? Vector2.right : Vector2.left;

        GameObject snowball = Instantiate(snowballPrefab, castPoint.position, Quaternion.identity);
        snowball.GetComponent<Snowball>().Init(direction);

        cooldownUI.TriggerCooldown(); // trigger the UI cooldown
    }
}