using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BLUE_SLIME_DAMAGE : MonoBehaviour
{
    public int damage;
    public HealthManager healthManager;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            healthManager.TakeDamage(damage);
        }
    }
}
