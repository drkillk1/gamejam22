using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 25f; // Amount of health to restore
    public GameObject pickupEffect; // Optional particle effect or visual effect upon pickup

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player collided with the health pickup
        if (collision.CompareTag("Player"))
        {
            // Access the player's health script
            PlayerCtrl playerCtrl = collision.GetComponent<PlayerCtrl>();
            if (playerCtrl != null)
            {
                // Heal the player
                playerCtrl.Heal(healAmount);
                Debug.Log($"Player healed by {healAmount} points.");

                // Play the pickup effect (optional)
                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }

                // Destroy the health pickup item
                Destroy(gameObject);
            }
        }
    }
}
