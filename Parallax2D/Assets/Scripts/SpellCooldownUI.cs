using UnityEngine;
using UnityEngine.UI;

public class SpellCooldownUI : MonoBehaviour
{
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private float cooldownTime = 2f;

    private float cooldownTimer = 0f;
    private bool onCooldown = false;

    void Start()
    {
        cooldownOverlay.fillAmount = 0f;
    }

    void Update()
    {
        if (onCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownOverlay.fillAmount = cooldownTimer / cooldownTime;

            if (cooldownTimer <= 0f)
            {
                onCooldown = false;
                cooldownOverlay.fillAmount = 0f;
            }
        }
    }

    public void TriggerCooldown()
    {
        cooldownTimer = cooldownTime;
        onCooldown = true;
        cooldownOverlay.fillAmount = 1f;
    }
}
