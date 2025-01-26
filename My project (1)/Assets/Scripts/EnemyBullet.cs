using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 10f; // Damage dealt to the player
    public float lifetime = 5f; // Time before the bullet is destroyed

    private void Start()
    {
        // Destroy the bullet after its lifetime
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCtrl playerCtrl = collision.GetComponent<PlayerCtrl>();
            if (playerCtrl != null)
            {
                playerCtrl.TakeDamage(damage); // Deal damage to the player
            }

            Destroy(gameObject); // Destroy the bullet on impact
        }
        // Ignore collision with walls or other objects
        else if (collision.CompareTag("Wall"))
        {
            // Debug.Log("Bullet hit the wall and is destroyed.");
            Destroy(gameObject);
        }
    }
}
