using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BlueSlimeDamage : MonoBehaviour
{
    public int damage;
    public HealthManager currentHealth;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            currentHealth.TakeDamage(damage);
        }
    }
}
